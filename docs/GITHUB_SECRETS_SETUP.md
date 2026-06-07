# GitHub Secrets Configuration

This document explains how to configure GitHub Actions secrets for deployment.

## Setup Instructions

### 1. Navigate to GitHub Repository Settings

1. Go to your repository on GitHub
2. Click **Settings** (top right)
3. In the left sidebar, click **Secrets and variables** → **Actions**

### 2. Create Repository Secrets

Click **New repository secret** and add the following:

#### `DEPLOY_URL`
- **Name**: `DEPLOY_URL`
- **Value**: Your hosting deployment URL (e.g., `https://monsterasp.example.com/deploy`)
- **Description**: The endpoint where your application should be deployed

#### `DEPLOY_USERNAME`
- **Name**: `DEPLOY_USERNAME`
- **Value**: Your hosting control panel username
- **Description**: Username for authentication to the hosting provider

#### `DEPLOY_PASSWORD`
- **Name**: `DEPLOY_PASSWORD`
- **Value**: Your hosting control panel password
- **Description**: Password for authentication to the hosting provider

### 3. Verify Secrets are Set

After adding the secrets:
1. Refresh the Secrets page
2. You should see all three secrets listed (values will be masked)
3. The CI/CD pipeline will now be able to access them

## How Secrets Work in GitHub Actions

- Secrets are **encrypted** and stored securely by GitHub
- They are **never displayed** in logs or workflow output
- They are only available to workflows in your repository
- Each secret is masked in logs to prevent accidental exposure

## Security Best Practices

- ✅ Use strong, unique passwords
- ✅ Rotate credentials periodically
- ✅ Use least-privilege accounts for deployment
- ✅ Never commit credentials to source code
- ✅ Use environment-specific secrets
- ❌ Never paste secrets directly in workflows
- ❌ Never log secret values

## Deployment Flow

1. Code pushed to `main` branch
2. GitHub Actions runs CI/CD pipeline
3. If tests pass, builds and pushes Docker image
4. Deployment step retrieves secrets from GitHub
5. Application is deployed to hosting provider
6. Secrets are never exposed in logs or history

## Troubleshooting

### Deployment Still Fails with "Secrets not configured"

1. Verify all three secrets are added to GitHub
2. Check that secrets are in the correct repository (not organization level)
3. Ensure the workflow file references correct secret names: `${{ secrets.DEPLOY_URL }}`
4. Re-run the workflow after adding secrets

### Secrets Not Working in Pull Requests

- Repository secrets are not available in pull request workflows for security reasons
- This prevents accidentally exposing secrets through untrusted code
- Secrets are only available for workflows on protected branches

## Additional Resources

- [GitHub Docs: Using Secrets](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions)
- [GitHub Docs: Encrypted Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
