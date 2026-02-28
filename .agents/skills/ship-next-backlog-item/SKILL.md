# ship-next-backlog-item

Implement exactly one top-priority READY backlog item with minimal blast radius.

## Scope
- Read `BACKLOG.md`.
- Select the first item marked `[READY]`.
- Implement only that item.
- Run smoke checks (delegate to `$run-smoke-test` if available).
- Update `CHANGELOG.md` under Unreleased.
- Create branch `codex/<item-slug>`.
- Commit all relevant files.

## Boundaries
- Do not pull in external dependencies.
- Do not refactor unrelated systems.
- Keep project fully offline.
- If blocked, document the blocker in `Reports/smoke_test_latest.md` and stop.
