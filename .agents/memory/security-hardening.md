---
name: Security hardening decisions
description: Key security rules applied to the FinBank API — must be preserved in future changes.
---

## JWT

- `Jwt:Key` must be set via env var — both `AuthService.cs` and `AuthenticationServiceExtensions.cs` throw `InvalidOperationException` if missing or < 32 bytes. Never add a fallback default.
- Token expiry is read from `Jwt:ExpiryMinutes` (default 60 min). Banking context — do not extend beyond 60 min without explicit product decision.
- Token validation is fully explicit: `ValidateLifetime=true`, `ClockSkew=30s`, `RequireExpirationTime=true`, issuer + audience + signing key all validated.
- Each token gets a unique `jti` (JWT ID) claim for future revocation support.

**Why:** Original code had hardcoded fallback keys (`"SUPER_SECRET_KEY"`) and 7-day expiry — both catastrophic for a banking app.

## Identity / Lockout

- `lockoutOnFailure: true` in `CheckPasswordSignInAsync` — account locks after 5 bad attempts for 15 minutes.
- Password policy: 12+ chars, uppercase + lowercase + digit + special char, 6+ unique chars.

**Why:** Original had `lockoutOnFailure: false` and min 6-char password with no uppercase or special char requirement.

## Error handling

- `GlobalExceptionMiddleware` never sends exception details, message text, or stack traces to clients — only a generic safe message. Full details logged server-side only.

**Why:** Original sent `exception.ToString()` to clients in the default (500) case.

## PII / sensitive data in logs

- **Session tokens / refresh tokens**: always pass through `MaskToken()` (first 4 + "..." + last 4) before logging.
- **Bank account numbers**: always pass through `MaskAccountNumber()` (last 4 digits only) before logging.
- **Phone numbers**: always pass through `MaskPhoneNumber()` (prefix + masked middle + last 4) before logging.
- **Credit scores**: log only the score *range* (e.g., "Good"), never the numeric value.

**How to apply:** Any new log statement involving tokens, account numbers, phone numbers, or credit data must use the appropriate masker helper. The helpers live in `SessionService`, `AccountValidationService`, and `SmsService` respectively.

## Nginx

- H2C smuggling: `Connection` header is cleared (`""`) on proxy; `Upgrade` only forwarded when value is `"websocket"`.
- Security headers (`X-Frame-Options: DENY`, `HSTS`, `CSP`, etc.) defined at server block. When a location block adds *any* `add_header`, all server-block headers must be re-declared inside that block too (nginx inheritance rule).
- `X-Frame-Options` changed from `SAMEORIGIN` to `DENY` — banking app has no legitimate iframe embedding use-case.
