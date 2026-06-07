# GitHub Workflows - Issues and Fixes

## Summary
Your GitHub workflows had path mismatches causing build failures. All workflows have been reviewed and fixed.

---

## Issues Found and Fixed

### 1. **CI/CD Pipeline** (`.github/workflows/ci-cd.yml`)
**Problem:** Workflows looked for `bank-backend/` directory which doesn't exist
```bash
# ❌ WRONG
dotnet restore bank-backend/
dotnet build bank-backend/ --no-restore
```

**Solution:** Updated to use correct path structure
```bash
# ✅ CORRECT
dotnet restore src/Bank.sln
dotnet build src/Bank.sln --no-restore
```

**Changes Made:**
- Line ~18: `bank-backend/` → `src/Bank.sln`
- Line ~21: `bank-backend/` → `src/Bank.sln`
- Line ~24: `bank-backend/` → `src/Bank.sln`
- Frontend path: `Bank.Frontend/` → `src/Bank.Frontend/`

---

### 2. **Code Quality Analysis** (`.github/workflows/code-quality.yml`)
**Problem:** Same path issues in SonarCloud and CodeQL jobs

**Fixes Applied:**
1. **SonarCloud job:**
   - `bank-backend/` → `src/Bank.sln` (3 occurrences)

2. **CodeQL job - C# path:**
   - `bank-backend/` → `src/Bank.sln` (2 occurrences)

3. **CodeQL job - JavaScript path:**
   - `Bank.Frontend/package-lock.json` → `src/Bank.Frontend/package-lock.json`
   - `cd Bank.Frontend` → `cd src/Bank.Frontend`

4. **ESLint job:**
   - `Bank.Frontend/package-lock.json` → `src/Bank.Frontend/package-lock.json`
   - `cd Bank.Frontend` → `cd src/Bank.Frontend`
   - `Bank.Frontend/eslint-results.sarif` → `src/Bank.Frontend/eslint-results.sarif`

---

### 3. **Security Scan** (`.github/workflows/security-scan.yml`)
**Status:** ✅ No changes needed
- Uses file system scan (`scan-ref: '.'`)
- Doesn't reference specific paths
- Should work as-is

---

### 4. **Performance Testing** (`.github/workflows/performance-test.yml`)
**Status:** ✅ No changes needed
- Generates its own test configurations
- Doesn't reference project paths
- Should work as-is

---

## Actual Project Structure
```
Bank-Api/
├── src/
│   ├── Bank.sln                  (Main solution)
│   ├── Bank.Api/                 (Web API project)
│   ├── Bank.Application/         (Business logic)
│   ├── Bank.Domain/              (Domain entities)
│   ├── Bank.Infrastructure/      (Data access)
│   └── Bank.Frontend/            (Angular frontend - if exists)
├── devops/
│   ├── docker/
│   │   ├── Dockerfile.backend
│   │   └── Dockerfile.frontend
│   └── ...
└── .github/
    └── workflows/
```

---

## What You Need to Do

### 1. **For Frontend Development**
If you plan to add a frontend:
- Create `src/Bank.Frontend/` directory
- Add `package.json` and `package-lock.json`
- Workflows will automatically work with the updated paths

### 2. **Environment Secrets Required**
Add these secrets to your GitHub repository:
```
SNYK_TOKEN          - For Snyk security scanning
SONAR_TOKEN         - For SonarCloud analysis
GITHUB_TOKEN        - Already provided by GitHub
```

### 3. **Docker Registries**
Ensure Dockerfiles are in `devops/docker/`:
- `Dockerfile.backend` ✅ Exists
- `Dockerfile.frontend` ⏳ Create when frontend is ready

### 4. **Test Projects**
Create test projects for better coverage:
- `Bank.Api.Tests/`
- `Bank.Application.Tests/`
- `Bank.Infrastructure.Tests/`

The workflows will automatically run tests if they exist.

---

## Workflow Triggers

| Workflow | Trigger | Runs |
|----------|---------|------|
| **CI/CD** | Push to `main`/`develop`<br>PR to `main` | Build, Test, Docker build |
| **Code Quality** | Push/PR to `main`<br>Daily 1 AM | SonarCloud, CodeQL, ESLint |
| **Security Scan** | Push to `main`<br>Daily 2 AM | Trivy, Snyk, Dependency-Check |
| **Performance** | Weekly Monday 3 AM<br>Manual dispatch | Artillery load tests |

---

## Testing Locally

Test your workflows locally before pushing:

```bash
# Restore and build
dotnet restore src/Bank.sln
dotnet build src/Bank.sln --no-restore

# Run tests
dotnet test src/Bank.sln --no-build --configuration Release

# Build Docker images (if needed)
docker build -f devops/docker/Dockerfile.backend -t bank-api:latest .
```

---

## Next Steps

1. ✅ Commit workflow fixes (Already done in CI/CD update)
2. ⏳ Add required secrets to GitHub repository settings
3. ⏳ Create test projects if they don't exist
4. ⏳ Test workflows with next push to main
5. ⏳ Monitor workflow runs in GitHub Actions tab

---

## Files Modified
- `.github/workflows/ci-cd.yml` - ✅ Fixed
- `.github/workflows/code-quality.yml` - ✅ Fixed
