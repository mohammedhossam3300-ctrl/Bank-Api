---
name: Controller cleanup pattern
description: How the try/catch removal transformation works and its edge cases
---

## Rule
Remove `try { body } catch (Exception ex) { _logger.LogError; return StatusCode(500); }` from controllers — GlobalExceptionMiddleware handles all of these centrally.

## How to apply
- Target `try` at exactly 8-space indentation (method body level); leave deeper indentation alone (lambdas, nested blocks)
- Two logger removal regex patterns needed:
  1. `_logger = logger;` (simple assignment)
  2. `_logger = logger ?? throw new ArgumentNullException(nameof(logger));` (null-check form)
- After removing logger field+param, check DI registration files for fully-qualified type names using the old namespace
- BillerManagementController special case: logger used for informational audit logging OUTSIDE catch blocks — keep the field, only remove catch blocks

**Why:** GlobalExceptionMiddleware was already handling all these exception types centrally. The per-controller catches were redundant and some leaked `ex.Message` to clients (security issue).

**How to apply:** When adding new controllers, never add try/catch for generic exceptions — let middleware handle it.
