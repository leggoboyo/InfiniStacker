# InfiniStacker

Offline, original hyper-casual lane survival shooter prototype in Unity (URP target), inspired by ad-style mechanics only.

## Constraints
- Offline-only runtime (no ads, analytics, telemetry, remote config, IAP).
- Unity 2022.3 LTS baseline.
- Input System for touch + mouse drag.
- TextMeshPro for counters/labels.
- No Asset Store or external dependencies.

## Open And Run
1. Open this folder in Unity Hub using Unity `2022.3 LTS`.
2. Let the editor finish script import (it auto-generates `Assets/Scenes/Game.unity` if missing).
3. Open scene `Assets/Scenes/Game.unity`.
4. Press Play.

If Unity CLI is unavailable, this repo still works through Unity Hub + Editor.

## How To Test (Current Milestone)
### M1
1. Open `Assets/Scenes/Game.unity` and press Play.
2. Press `Start` in the start panel.
3. Drag finger (device) or mouse (editor) left/right and confirm player movement is clamped to bridge bounds.
4. Confirm continuous forward auto-fire and enemy waves spawning from ahead.
5. Confirm enemies die on bullet hits with simple hit VFX.
6. Confirm breaching enemies reduce squad and base HP.
7. Confirm defeat triggers on squad/base depletion, or Victory triggers at 60 seconds.
8. Confirm Play Mode has no console errors.

## Milestone Notes
- M0: Repository scaffolding, agent guidance, backlog, changelog, and automation docs/skills.
- M1: Runtime bootstrap scene, bridge environment, drag movement, pooled bullets/VFX, enemy waves, base HP + timer, and start/victory/game-over UI.
