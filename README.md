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
### M2
1. Open `Assets/Scenes/Game.unity` and press Play.
2. Press `Start` in the start panel.
3. Drag finger (device) or mouse (editor) left/right and confirm player movement is clamped to bridge bounds.
4. Confirm continuous forward auto-fire and enemy waves spawning from ahead.
5. Confirm enemies die on bullet hits with simple hit VFX.
6. Confirm squad count near player updates dynamically (breaches reduce squad; every 5 enemy kills grants +1 reinforcement soldier).
7. Confirm soldier formation expands/contracts immediately as squad power changes.
8. Confirm firepower scales with squad size (more visible bullet streams with larger squad).
9. Confirm defeat triggers on squad/base depletion, or Victory triggers at 60 seconds.
10. Confirm Play Mode has no console errors.

## Milestone Notes
- M0: Repository scaffolding, agent guidance, backlog, changelog, and automation docs/skills.
- M1: Runtime bootstrap scene, bridge environment, drag movement, pooled bullets/VFX, enemy waves, base HP + timer, and start/victory/game-over UI.
- M2: Player power now maps to soldier count with live formation updates and scalable firepower; enemy kills provide reinforcements while breaches remove soldiers.
