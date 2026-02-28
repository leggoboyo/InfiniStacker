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
### M3
1. Open `Assets/Scenes/Game.unity` and press Play.
2. Press `Start` in the start panel.
3. Drag finger (device) or mouse (editor) left/right and confirm player movement is clamped to bridge bounds.
4. Confirm continuous forward auto-fire and enemy waves spawning from ahead.
5. Confirm left/right gate pairs spawn periodically and scroll toward the player.
6. Move to the left lane and confirm the left gate operation applies exactly once when the pair crosses the player line.
7. Move to the right lane and confirm the right gate operation applies exactly once.
8. Confirm gate colors and labels are clear (`+N`/`xN` positive, `-N` negative).
9. Confirm soldier formation and bullet volume update immediately after each gate operation.
10. Run EditMode tests and confirm all tests pass (`GateMathTests`, `SquadFormationTests`).
11. Confirm Play Mode has no console errors.

## Milestone Notes
- M0: Repository scaffolding, agent guidance, backlog, changelog, and automation docs/skills.
- M1: Runtime bootstrap scene, bridge environment, drag movement, pooled bullets/VFX, enemy waves, base HP + timer, and start/victory/game-over UI.
- M2: Player power now maps to soldier count with live formation updates and scalable firepower; enemy kills provide reinforcements while breaches remove soldiers.
- M3: Arithmetic lane gates added (`+`, `-`, `x`) with deterministic application, clear color/text signage, and EditMode coverage for gate math and formation sizing.
