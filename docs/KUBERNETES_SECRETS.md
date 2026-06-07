# Kubernetes Secrets Management Guide

## Overview

This guide explains how to securely manage secrets in Kubernetes for the Bank API deployment. **Never store sensitive data in ConfigMaps or commit secrets to the repository.**

## Security Principles

- ✅ Secrets should be **separate from ConfigMaps**
- ✅ Sensitive data should be **encrypted at rest**
- ✅ Secrets should be **injected at deployment time**
- ✅ Use **RBAC** to restrict secret access
- ✅ Use **audit logging** for secret access
- ❌ Never commit secrets to git
- ❌ Never store secrets in ConfigMaps
- ❌ Never expose secrets in logs or error messages

## Current Setup

### ConfigMap (Non-Sensitive Data)
Located in `configmap.yaml`:
- `ASPNETCORE_ENVIRONMENT`: Production
- `JwtSettings__Issuer`: BankAPI
- `JwtSettings__Audience`: BankClients
- `JwtSettings__ExpirationMinutes`: 60

### Kubernetes Secrets (Sensitive Data)
Located in `configmap.yaml` (Secret section):
- `ConnectionStrings__DefaultConnection`: Database connection string
- `JwtSettings__SecretKey`: JWT signing key
- `SA_PASSWORD`: SQL Server administrator password

## Deployment Methods

### Method 1: Direct kubectl (Development Only)

⚠️ **NOT recommended for production**

```bash
# Create secrets from environment file
kubectl create secret generic bank-secrets \
  --from-env-file=devops/kubernetes/secrets.env \
  --namespace=bank-app

# Or create individual secrets
kubectl create secret generic bank-secrets \
  --from-literal=ConnectionStrings__DefaultConnection="Server=..." \
  --from-literal=JwtSettings__SecretKey="..." \
  --from-literal=SA_PASSWORD="..." \
  --namespace=bank-app
```

### Method 2: Sealed Secrets (Recommended for Production)

Sealed Secrets encrypt secrets at rest in the cluster.

**Installation:**
```bash
kubectl apply -f https://github.com/bitnami-labs/sealed-secrets/releases/download/v0.18.0/controller.yaml
```

**Usage:**
```bash
# Create a secret file
cat > secret.yaml << EOF
apiVersion: v1
kind: Secret
metadata:
  name: bank-secrets
  namespace: bank-app
type: Opaque
stringData:
  ConnectionStrings__DefaultConnection: "..."
  JwtSettings__SecretKey: "..."
  SA_PASSWORD: "..."
EOF

# Seal the secret
kubeseal -f secret.yaml -w sealed-secret.yaml

# Apply the sealed secret (safe to commit)
kubectl apply -f sealed-secret.yaml
```

### Method 3: External Secrets Operator (Recommended)

External Secrets syncs secrets from external providers (AWS Secrets Manager, Azure Key Vault, etc.)

**Installation:**
```bash
helm repo add external-secrets https://external-secrets.io/charts
helm install external-secrets \
  external-secrets/external-secrets \
  -n external-secrets-system \
  --create-namespace
```

**Configuration:**
```yaml
apiVersion: external-secrets.io/v1beta1
kind: SecretStore
metadata:
  name: aws-secrets-manager
  namespace: bank-app
spec:
  provider:
    aws:
      service: SecretsManager
      region: us-east-1
      auth:
        jwt:
          serviceAccountRef:
            name: external-secrets-sa
---
apiVersion: external-secrets.io/v1beta1
kind: ExternalSecret
metadata:
  name: bank-secrets
  namespace: bank-app
spec:
  refreshInterval: 1h
  secretStoreRef:
    name: aws-secrets-manager
    kind: SecretStore
  target:
    name: bank-secrets
    creationPolicy: Owner
  data:
    - secretKey: ConnectionStrings__DefaultConnection
      remoteRef:
        key: bank-api/db-connection-string
    - secretKey: JwtSettings__SecretKey
      remoteRef:
        key: bank-api/jwt-key
    - secretKey: SA_PASSWORD
      remoteRef:
        key: bank-api/sa-password
```

### Method 4: Kustomize with Sealed Secrets

Using Kustomize to manage configurations and Sealed Secrets for sensitive data.

```bash
# Build with Kustomize
kustomize build devops/kubernetes/overlays/production | kubectl apply -f -
```

## Best Practices

### 1. Secret Rotation
- Rotate secrets regularly (e.g., every 90 days)
- Use automated tools for rotation
- Plan zero-downtime secret updates

### 2. Access Control
- Restrict who can read/write secrets using RBAC
- Use service accounts with minimal permissions
- Audit all secret access

### 3. Encryption
- Enable encryption at rest in etcd
- Use network policies to restrict secret access
- Consider using a separate secret management system

### 4. Monitoring
- Monitor secret creation and deletion
- Alert on unauthorized access attempts
- Log all secret mutations

### 5. Secret Naming
- Use descriptive, consistent names
- Group related secrets with prefixes (e.g., `bank-api-db-*`)
- Document the purpose of each secret

## GitHub Actions Integration

For CI/CD pipelines, use GitHub Secrets (not repository secrets):

```yaml
# .github/workflows/deploy.yml
name: Deploy to Kubernetes

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Create Kubernetes Secrets
        run: |
          kubectl create secret generic bank-secrets \
            --from-literal=ConnectionStrings__DefaultConnection="${{ secrets.DB_CONNECTION }}" \
            --from-literal=JwtSettings__SecretKey="${{ secrets.JWT_KEY }}" \
            --from-literal=SA_PASSWORD="${{ secrets.SA_PASSWORD }}" \
            --namespace=bank-app \
            --dry-run=client \
            -o yaml | kubectl apply -f -
```

## Troubleshooting

### Secret Not Found
```bash
# List all secrets in namespace
kubectl get secrets -n bank-app

# Describe the secret
kubectl describe secret bank-secrets -n bank-app
```

### Secret Not Updated in Pod
```bash
# Secrets are not automatically reloaded by pods
# You must restart the pods to pick up new secrets

# Rolling restart
kubectl rollout restart deployment/bank-api -n bank-app
```

### Debugging Secret Values (Development Only)
```bash
# WARNING: Never do this in production!
# View encoded secret value
kubectl get secret bank-secrets -n bank-app \
  -o jsonpath='{.data.JwtSettings__SecretKey}' | base64 -d
```

## Security Checklist

- [ ] Secrets are NOT stored in ConfigMaps
- [ ] Secrets are encrypted at rest in etcd
- [ ] RBAC policies restrict secret access
- [ ] Audit logging is enabled for secret access
- [ ] Secrets are rotated regularly
- [ ] No secrets in git history
- [ ] Sealed Secrets or External Secrets is used (production)
- [ ] Pod security policies enforce secret usage
- [ ] Network policies restrict secret access
- [ ] Monitoring and alerting for secret access

## References

- [Kubernetes Secrets Documentation](https://kubernetes.io/docs/concepts/configuration/secret/)
- [Sealed Secrets Project](https://github.com/bitnami-labs/sealed-secrets)
- [External Secrets Operator](https://external-secrets.io/)
- [Kubernetes Security Best Practices](https://kubernetes.io/docs/concepts/security/)
