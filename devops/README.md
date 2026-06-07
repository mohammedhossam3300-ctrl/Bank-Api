# DevOps Infrastructure

This directory contains all DevOps-related configurations and scripts for the Bank Management System.

## Directory Structure

```
devops/
├── docker/                 # Docker configurations
│   ├── Dockerfile.backend  # Backend container
│   ├── Dockerfile.frontend # Frontend container
│   ├── docker-compose.yml  # Production compose
│   ├── docker-compose.dev.yml # Development compose
│   └── nginx.conf          # Nginx configuration
├── kubernetes/             # Kubernetes manifests
│   ├── namespace.yaml      # Namespace definition
│   ├── configmap.yaml      # Configuration and secrets
│   ├── database.yaml       # Database deployment
│   ├── backend.yaml        # Backend deployment
│   ├── frontend.yaml       # Frontend deployment
│   └── ingress.yaml        # Ingress configuration
├── terraform/              # Infrastructure as Code
│   ├── main.tf            # Main Terraform configuration
│   ├── variables.tf       # Variable definitions
│   └── outputs.tf         # Output definitions
├── helm/                   # Helm charts
│   ├── Chart.yaml         # Chart metadata
│   └── values.yaml        # Default values
├── monitoring/             # Monitoring configurations
│   ├── prometheus.yml     # Prometheus configuration
│   └── grafana-dashboard.json # Grafana dashboard
├── scripts/               # Deployment scripts
│   ├── deploy.sh          # Main deployment script
│   ├── smoke-tests.sh     # Health check tests
│   └── backup.sh          # Database backup script
└── ansible/               # Configuration management
```

## Quick Start

### Local Development

1. **Start development environment:**
```bash
docker-compose -f devops/docker/docker-compose.dev.yml up -d
```

2. **Build and run locally:**
```bash
docker-compose -f devops/docker/docker-compose.yml up --build
```

### Production Deployment

#### Using Kubernetes

1. **Deploy to Kubernetes:**
```bash
./devops/scripts/deploy.sh production latest
```

2. **Check deployment status:**
```bash
kubectl get pods -n bank-app
kubectl get services -n bank-app
```

#### Using Terraform (Azure)

1. **Initialize Terraform:**
```bash
cd devops/terraform
terraform init
```

2. **Plan deployment:**
```bash
terraform plan -var="sql_admin_password=YourSecurePassword123!"
```

3. **Apply infrastructure:**
```bash
terraform apply -var="sql_admin_password=YourSecurePassword123!"
```

#### Using Helm

1. **Install with Helm:**
```bash
helm install bank-app devops/helm/ --namespace bank-app --create-namespace
```

2. **Upgrade deployment:**
```bash
helm upgrade bank-app devops/helm/ --namespace bank-app
```

## CI/CD Pipeline

The GitHub Actions workflow (`.github/workflows/ci-cd.yml`) provides:

- **Continuous Integration:**
  - Backend tests (.NET)
  - Frontend tests (Angular)
  - Security scanning
  - Code quality checks

- **Continuous Deployment:**
  - Docker image building
  - Container registry push
  - Automated deployment to staging
  - Manual approval for production

## Monitoring

### Prometheus Metrics

The application exposes metrics at `/metrics` endpoint:
- HTTP request duration
- Request rate
- Error rate
- Database connection pool metrics

### Grafana Dashboard

Import the dashboard from `devops/monitoring/grafana-dashboard.json` to visualize:
- API performance metrics
- System resource usage
- Error rates and alerts

## Security

### Secrets Management

- **Kubernetes:** Uses ConfigMaps and Secrets
- **Azure:** Azure Key Vault integration
- **Docker:** Environment variables (development only)

### Network Security

- **Ingress:** NGINX with SSL/TLS termination
- **Network Policies:** Pod-to-pod communication restrictions
- **Service Mesh:** Optional Istio integration

## Backup and Recovery

### Database Backups

```bash
# Manual backup
./devops/scripts/backup.sh

# Automated backups (cron job)
0 2 * * * /path/to/devops/scripts/backup.sh
```

### Disaster Recovery

1. **Database restore:**
```bash
kubectl exec -it database-pod -n bank-app -- psql -U $DB_USER -d bankdb -c "SELECT pg_restore_from_backup('/backups/backup.dump');"
```

2. **Application rollback:**
```bash
kubectl rollout undo deployment/backend -n bank-app
kubectl rollout undo deployment/frontend -n bank-app
```

## Scaling

### Horizontal Pod Autoscaler

The HPA is configured to scale based on:
- CPU utilization (70% threshold)
- Memory utilization (80% threshold)

### Manual Scaling

```bash
kubectl scale deployment backend --replicas=5 -n bank-app
kubectl scale deployment frontend --replicas=3 -n bank-app
```

## Troubleshooting

### Common Issues

1. **Pod not starting:**
```bash
kubectl describe pod <pod-name> -n bank-app
kubectl logs <pod-name> -n bank-app
```

2. **Database connection issues:**
```bash
kubectl exec -it database-pod -n bank-app -- psql -U $DB_USER -d bankdb
```

3. **Ingress not working:**
```bash
kubectl get ingress -n bank-app
kubectl describe ingress bank-ingress -n bank-app
```

### Health Checks

```bash
# Run smoke tests
./devops/scripts/smoke-tests.sh http://your-domain.com

# Check all services
kubectl get all -n bank-app
```

## Environment Variables

### Required Environment Variables

- `DB_PASSWORD`: PostgreSQL database password
- `DB_USER`: PostgreSQL database user
- `JWT_SECRET_KEY`: JWT signing key
- `ASPNETCORE_ENVIRONMENT`: Application environment

### Optional Environment Variables

- `AWS_S3_BUCKET`: S3 bucket for backups
- `AZURE_STORAGE_ACCOUNT`: Azure storage for backups
- `SMTP_SERVER`: Email server for notifications

## Support

For deployment issues:
1. Check the logs: `kubectl logs -f deployment/backend -n bank-app`
2. Run health checks: `./devops/scripts/smoke-tests.sh`
3. Review monitoring dashboards
4. Contact the DevOps team