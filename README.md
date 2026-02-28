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
### M4
1. Open `Assets/Scenes/Game.unity` and press Play.
2. Press `Start` in the start panel.
3. Drag finger (device) or mouse (editor) left/right and confirm player movement is clamped to bridge bounds.
4. Confirm continuous forward auto-fire and enemy waves spawning from ahead.
5. Confirm left/right gate pairs spawn periodically and apply arithmetic once per pair.
6. Confirm ice obstacles spawn ahead, can be shot to break, and remove squad soldiers if not cleared before contact.
7. Confirm soldier formation and bullet volume update immediately after gate/obstacle/combat squad changes.
8. Confirm Victory at 60 seconds and defeat when squad or base HP reaches 0.
9. In Editor quick-test mode, press `1` add +10 squad, `2` spawn enemy wave, `3` spawn gate pair, `4` damage base, `5` spawn obstacle.
10. Run EditMode tests and confirm all tests pass (`GateMathTests`, `SquadFormationTests`).
11. Confirm Play Mode has no console errors.

## Milestone Notes
- M0: Repository scaffolding, agent guidance, backlog, changelog, and automation docs/skills.
- M1: Runtime bootstrap scene, bridge environment, drag movement, pooled bullets/VFX, enemy waves, base HP + timer, and start/victory/game-over UI.
- M2: Player power now maps to soldier count with live formation updates and scalable firepower; enemy kills provide reinforcements while breaches remove soldiers.
- M3: Arithmetic lane gates added (`+`, `-`, `x`) with deterministic application, clear color/text signage, and EditMode coverage for gate math and formation sizing.
- M4: Destructible ice obstacles, screen-shake/haptics service hooks, editor debug hotkeys, and iOS build notes in `docs/IOS_BUILD.md`.
