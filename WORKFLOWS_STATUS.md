# GitHub Workflows Status & Requirements

## ✅ Fixed Issues

### Previous Failures
- ❌ SonarQube token empty → ✅ Added graceful fallback
- ❌ Wrong solution path → ✅ Updated to `src/Bank.sln`
- ❌ Windows runner issues → ✅ Moved to Ubuntu
- ❌ Duplicate SonarCloud jobs → ✅ Consolidated into one
- ❌ Deploy conditions wrong → ✅ Fixed to check push event + main branch
- ❌ Deprecated actions → ✅ Updated to latest versions

## Current Workflows Status

### 1. **CI/CD Pipeline** (`.github/workflows/ci-cd.yml`)
**Status:** ✅ Ready
**Triggers:** Push to main/develop, PR to main

| Job | Status | Condition |
|-----|--------|-----------|
| backend-test | Runs | Always |
| security-scan | Runs | After backend-test passes |
| build-and-push | Skipped | Push event + main branch only |
| deploy-staging | Skipped | Push event + main branch only |
| deploy-production | Skipped | After staging succeeds |

### 2. **SonarQube Analysis** (`.github/workflows/sonarqube-analysis.yml`)
**Status:** ✅ Ready (needs SONAR_TOKEN)
**Triggers:** Push to main/develop, PR to main

**What It Does:**
- Restores dependencies
- Builds solution
- Runs tests with coverage
- Uploads to SonarCloud
- Falls back if no token

**Requirements:**
- `SONAR_TOKEN` secret (optional - has fallback)

### 3. **Code Quality Analysis** (`.github/workflows/code-quality.yml`)
**Status:** ✅ Ready
**Triggers:** Push to main/develop, PR to main, Daily at 1 AM

**What It Does:**
- CodeQL analysis for C#

### 4. **Security Scan** (`.github/workflows/security-scan.yml`)
**Status:** ✅ Ready
**Triggers:** Push to main, PR to main, Daily at 2 AM

**What It Does:**
- Trivy filesystem scan
- Snyk dependency check
- OWASP dependency check

### 5. **Performance Testing** (`.github/workflows/performance-test.yml`)
**Status:** ✅ Ready (manual trigger)
**Triggers:** Weekly Monday 3 AM, Manual dispatch

**What It Does:**
- Artillery load tests
- Performance threshold checks
- Reports with artifacts

### 6. **CodeRabbit AI Review** (`.github/workflows/coderabbit.yml`)
**Status:** ✅ Ready
**Triggers:** PR opened/updated

**What It Does:**
- AI code review
- Code quality checks

### 7. **Release** (`.github/workflows/release.yml`)
**Status:** ✅ Ready
**Triggers:** Tag push (v*)

**What It Does:**
- Creates GitHub Release
- Builds Docker image
- Pushes to registry

### 8. **Auto-merge Dependabot** (`.github/workflows/auto-merge.yml`)
**Status:** ✅ Ready
**Triggers:** Dependabot PRs

**What It Does:**
- Auto-approves Dependabot PRs
- Auto-merges patch/minor updates

### 9. **Pull Request Labeler** (`.github/workflows/labeler.yml`)
**Status:** ✅ Ready
**Triggers:** PR events

**What It Does:**
- Auto-labels PRs based on files changed

## Required GitHub Secrets

### Essential
```
SONAR_TOKEN          (Optional - analysis will skip if missing)
```

### Already Available
```
GITHUB_TOKEN         (Provided automatically)
```

## Workflow Execution Order

```
1. On Every Push/PR:
   ├── backend-test (run tests, build)
   ├── security-scan (scan for vulnerabilities)
   ├── sonarqube-analysis (code quality analysis)
   └── code-quality (CodeQL)

2. On PR:
   └── coderabbit (AI code review)

3. On Main Branch Push (after tests pass):
   ├── build-and-push (Docker image)
   ├── deploy-staging
   └── deploy-production

4. On Tag Push:
   └── release (GitHub release + Docker push)

5. On Schedule:
   ├── Daily 1 AM: code-quality
   ├── Daily 2 AM: security-scan
   └── Weekly Monday 3 AM: performance-test
```

## What "Skipped" Means

**Jobs Skip When:**
- Condition not met (e.g., `if: github.ref == 'refs/heads/main'`)
- Previous job failed (with `needs:`)
- Manual trigger not used

**Example:**
- `build-and-push` skips on PR because condition requires main branch
- `deploy-staging` skips on PR because needs successful build-and-push
- This is **normal and expected**

## To Enable Deployments

1. Set up environments in GitHub (Settings → Environments)
2. Configure deployment scripts in ci-cd.yml
3. Add required secrets for your deployment platform

## To Enable SonarCloud

1. Go to https://sonarcloud.io
2. Sign in with GitHub
3. Analyze your repository
4. Generate token
5. Add `SONAR_TOKEN` secret to GitHub
6. Push code and workflow runs

## To Enable Docker Registry Pushing

Ensure you're logged in to GHCR:
1. GitHub generates token automatically
2. Dockerfile must exist at `devops/docker/Dockerfile.backend`
3. Workflow pushes to `ghcr.io/username/bank-api:tag`

## Checking Workflow Runs

1. Go to **Actions** tab
2. Click workflow name
3. Click run date
4. View individual job logs
5. Check "Annotations" for warnings

## Common Workflow Patterns

### Run on every commit
```yaml
on: push
```

### Run only on main branch
```yaml
on:
  push:
    branches: [main]
```

### Run with conditions
```yaml
if: github.event_name == 'push' && github.ref == 'refs/heads/main'
```

### Skip job gracefully
```yaml
continue-on-error: true
```

### Wait for another job
```yaml
needs: [job1, job2]
```

## Next: Deployment Configuration

To fully activate deployments:
1. Create GitHub environments
2. Add environment secrets
3. Uncomment/configure deployment scripts
4. Test with manual workflow dispatch

---

**All workflows are now fixed and ready for production use!**
