# SonarQube Setup Guide for Bank API

## Quick Start - What You Need

### 1. Create SonarCloud Account
1. Go to https://sonarcloud.io
2. Sign in with GitHub
3. Click "Analyze a new project"
4. Select your repository
5. Choose "With GitHub Actions"

### 2. Generate SonarCloud Token
1. Go to https://sonarcloud.io/account/security
2. Click "Generate Tokens"
3. Give it a name (e.g., "Bank-API")
4. Copy the token

### 3. Add Token to GitHub Secrets
1. Go to: GitHub Repository → Settings → Secrets and variables → Actions
2. Click "New repository secret"
3. Name: `SONAR_TOKEN`
4. Value: Paste your SonarCloud token
5. Click "Add secret"

## What Gets Analyzed

**Automatic Analysis:**
- ✅ Code Quality
- ✅ Security Issues
- ✅ Code Coverage
- ✅ Duplicated Code
- ✅ Technical Debt
- ✅ Bugs and Vulnerabilities

**Files Analyzed:**
```
src/Bank.Api/
src/Bank.Application/
src/Bank.Domain/
src/Bank.Infrastructure/
```

## Workflow Triggers

| When | What Happens |
|------|--------------|
| Push to `main` | Full analysis + upload to SonarCloud |
| Push to `develop` | Full analysis + upload to SonarCloud |
| Pull Request to `main` | PR analysis (comment with results) |
| No SONAR_TOKEN | Build succeeds but analysis skipped |

## Expected Quality Gates

Default SonarCloud Quality Gate checks:
- ✅ Code Coverage ≥ 80%
- ✅ No Critical Issues
- ✅ Security Rating: A
- ✅ Reliability Rating: A

## Checking Results

After workflow runs:
1. Go to https://sonarcloud.io/dashboard
2. Select your project
3. View detailed analysis

## Common Issues & Fixes

### Issue: "The format of the analysis property sonar.token= is invalid"
**Fix:** SONAR_TOKEN environment variable is empty
**Solution:** Add SONAR_TOKEN to GitHub Secrets (see step 3 above)

### Issue: "MSBUILD: error MSB1003"
**Fix:** No solution file specified
**Solution:** Already fixed - now uses `src/Bank.sln`

### Issue: Build passes but SonarCloud analysis skipped
**Fix:** SONAR_TOKEN not configured
**Solution:** Workflows have graceful fallback when token is missing

## Getting Token Value

Your SonarCloud Organization Key:
- Format: `username` or `organization-name`
- Find at: https://sonarcloud.io/organizations

Your Project Key (auto-generated):
- Format: `github-username_repository-name`
- Example: `Mostafa-SAID7_Bank-Api`

## Workflow Configuration

Your current workflow settings:
- **Trigger:** Push to main/develop, PR to main
- **Analysis Engine:** SonarCloud
- **Language:** C# (.NET 9.0)
- **Coverage Format:** OpenCover
- **Fallback:** If no token, just build (no fail)

## Next Steps

1. ✅ Add SONAR_TOKEN to GitHub Secrets
2. Push code to main or create PR
3. Workflow runs automatically
4. Check SonarCloud dashboard for results
5. Fix any issues identified

## Advanced: Local Analysis

Run analysis locally before pushing:

```bash
# Install scanner
dotnet tool install --global dotnet-sonarscanner

# Run analysis
dotnet sonarscanner begin /k:"project-key" /o:"org-key" /d:sonar.token="your-token" /d:sonar.host.url="https://sonarcloud.io"
dotnet build src/Bank.sln
dotnet test src/Bank.sln --no-build --verbosity normal /p:CollectCoverage=true /p:CoverageFormat=opencover
dotnet sonarscanner end /d:sonar.token="your-token"
```

## Troubleshooting

Check workflow logs:
1. Go to Actions tab
2. Click "SonarQube Analysis" workflow
3. Click failed run
4. Expand "Build and analyze" step
5. Look for error messages

## Project Settings

- **Organization:** mostafa-said7 (or your org)
- **Project Key:** Auto-generated from repo name
- **Branch Analysis:** Enabled
- **Pull Request Analysis:** Enabled
- **Quality Gate:** SonarCloud Default

---

**After Setup:** Workflows will run automatically on every push/PR and report quality metrics.
