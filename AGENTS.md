# AGENTS

## Guardrails
- Keep the game fully offline. Do not add ads, analytics, telemetry, remote config, IAP, or network requirements.
- Do not add Asset Store dependencies or external packages beyond Unity defaults used by this project.
- Keep scripts small, explicit, and purpose-driven.
- Avoid broad refactors unless required to complete the current milestone.
- Build in milestones; each milestone must end with:
  - Playable Play Mode result,
  - README "How to test" update,
  - No Play Mode console errors.

## Engineering Style
- Prefer reliable, simple implementations over clever ones.
- Keep runtime allocations low and use pooling for hot paths.
- Maintain clean separation for gameplay systems (state, squad, combat, enemy spawn, gates, UI).
