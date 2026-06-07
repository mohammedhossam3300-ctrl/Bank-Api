# Bank Management System Helm Chart

This Helm chart deploys the Bank Management System to Kubernetes.

## Prerequisites

- Kubernetes cluster (1.24+)
- Helm 3.0+
- kubectl configured to access your cluster

## Installation

### Step 1: Create Database Secret

Before deploying, create a Kubernetes secret containing the database password:

```bash
# Create the secret with a secure password
kubectl create secret generic db-credentials \
  --from-literal=password='YourSecurePassword123!'

# Verify the secret was created
kubectl get secrets db-credentials
```

### Step 2: Deploy the Chart

```bash
# Install the chart
helm install bank-system ./devops/helm \
  --namespace bank-system \
  --create-namespace

# Or upgrade an existing deployment
helm upgrade bank-system ./devops/helm \
  --namespace bank-system
```

### Step 3: Verify Deployment

```bash
# Check deployment status
kubectl get deployments -n bank-system

# View pods
kubectl get pods -n bank-system

# Check ingress
kubectl get ingress -n bank-system
```

## Configuration

### Database Password Management

The database password is NOT stored in `values.yaml`. Instead, it's managed via Kubernetes Secrets:

```yaml
# In values.yaml (reference only)
database:
  passwordSecretName: db-credentials
  passwordSecretKey: password
```

The actual password is stored in the Kubernetes secret `db-credentials`.

### Updating the Database Password

To update the password:

```bash
# Delete the old secret
kubectl delete secret db-credentials -n bank-system

# Create a new secret with the new password
kubectl create secret generic db-credentials \
  --from-literal=password='NewSecurePassword456!' \
  -n bank-system

# Restart the database pod to pick up the new secret
kubectl rollout restart deployment/bank-system-database -n bank-system
```

## Security Best Practices

1. **Never commit passwords** to version control
2. **Use Kubernetes Secrets** for all sensitive data
3. **Encrypt secrets at rest** in your Kubernetes cluster
4. **Use RBAC** to restrict secret access
5. **Audit secret access** with Kubernetes audit logs
6. **Rotate passwords regularly** (recommended: every 90 days)

## Troubleshooting

### Secret not found error

If you see "secret db-credentials not found":

```bash
# Verify the secret exists
kubectl get secrets -n bank-system | grep db-credentials

# If missing, recreate it
kubectl create secret generic db-credentials \
  --from-literal=password='YourSecurePassword123!' \
  -n bank-system
```

### Database connection failures

Check the database pod logs:

```bash
kubectl logs -n bank-system -l app=database
```

## Helm Values

See `values.yaml` for all configurable options including:
- Replica counts
- Resource limits
- Ingress configuration
- SSL/TLS settings
- Scaling policies

## Support

For issues or questions, please open an issue on the repository.
