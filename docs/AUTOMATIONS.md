# Codex Automations Setup

Automations run locally and each run uses a **fresh worktree**. Keep prompts self-contained and point to this repo path.

## 1) Ship Next Backlog Item
- Name: `Ship Next Backlog Item`
- Schedule: Every weekday at 09:00 local time
- Workspace: this repo root
- Prompt to paste:

```text
Open BACKLOG.md, pick the top item tagged [READY], implement it with minimal scope, run $run-smoke-test, update CHANGELOG.md, create a branch named codex/<item-slug>, and commit with a clear message.
Use $ship-next-backlog-item.
```

## 2) Daily Smoke Test
- Name: `Daily Smoke Test`
- Schedule: Every day at 12:00 local time
- Workspace: this repo root
- Prompt to paste:

```text
Run the local smoke workflow and write/update Reports/smoke_test_latest.md with checks executed, results, and follow-up actions.
Use $run-smoke-test.
```

## 3) Weekly iOS Build Notes Refresh
- Name: `Refresh iOS Build Notes`
- Schedule: Every Monday at 10:00 local time
- Workspace: this repo root
- Prompt to paste:

```text
Regenerate docs/IOS_BUILD.md from current Unity project settings and build assumptions, keeping steps concise and actionable.
Use $prep-ios-build-notes.
```

## Notes
- Skills can be invoked in automation prompts via `$skill-name`.
- If Unity CLI is not available on the host, automation should still produce docs/reports and clearly note skipped Unity CLI steps.
