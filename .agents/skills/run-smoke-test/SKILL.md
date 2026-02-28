# run-smoke-test

Execute feasible local smoke checks and publish a concise report.

## Scope
- Run available automated checks (prefer Unity EditMode tests when Unity tooling exists).
- If Unity CLI is unavailable, run static repo checks (file presence, basic lint-like sanity) and note limitations.
- Write `Reports/smoke_test_latest.md` with:
  - timestamp,
  - checks run,
  - pass/fail,
  - notable risks,
  - next actions.

## Boundaries
- Read-only with respect to gameplay behavior.
- Do not introduce new dependencies.
