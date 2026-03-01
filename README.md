# InfiniStacker

Offline, original hyper-casual lane survival shooter prototype in Unity (URP target), inspired by ad-style mechanics only.

## Constraints
- Offline-only runtime (no ads, analytics, telemetry, remote config, IAP).
- Unity baseline from project version file (`InfiniStacker/ProjectSettings/ProjectVersion.txt`).
- Input System for touch + mouse drag.
- TextMeshPro for counters/labels.
- No Asset Store or external dependencies.

## Open And Run
1. In Unity Hub, open project folder: `InfiniStacker/`.
2. Use the Unity version recorded in `InfiniStacker/ProjectSettings/ProjectVersion.txt` (currently `6000.3.9f1` on this machine).
3. Let script import finish (the bootstrap script auto-creates `Assets/Scenes/Game.unity` if missing).
4. Open scene `Assets/Scenes/Game.unity` inside the `InfiniStacker` project.
5. Press Play.

If Unity CLI is unavailable, this repo still works through Unity Hub + Editor.

## How To Test (Current Milestone)
### M4
1. Open `Assets/Scenes/Game.unity` and press Play.
2. Press `Start` in the start panel.
3. Drag finger (device) or mouse (editor) left/right and confirm free swapping across both lanes (no center blocker).
4. Confirm zombies are killable on bullet hit, die with VFX, and are visually humanoid.
5. Confirm right lane loop: gate pairs + zombie pressure increase soldier-count strategy.
6. Confirm left lane loop: upgrade ice blocks can be broken by bullets and increase weapon progression (fire-rate/damage/shotgun tiers shown in top HUD).
7. Confirm turret charges accumulate from upgrade-block progression and auto-deploy when zombie pressure gets high.
8. Confirm bullet volume stays controlled at high squad count (performance cap is active while damage scales).
9. Confirm Victory at 60 seconds and defeat when squad or base HP reaches 0.
10. In Editor quick-test mode, press `1` add +10 squad, `2` spawn enemy wave, `3` spawn gate pair, `4` damage base, `5` spawn obstacle, `6` spawn left-lane upgrade block.
11. Run EditMode tests and confirm all tests pass (`GateMathTests`, `SquadFormationTests`).
12. Confirm Play Mode has no console errors.

## Milestone Notes
- M0: Repository scaffolding, agent guidance, backlog, changelog, and automation docs/skills.
- M1: Runtime bootstrap scene, bridge environment, drag movement, pooled bullets/VFX, enemy waves, base HP + timer, and start/victory/game-over UI.
- M2: Player power now maps to soldier count with live formation updates and scalable firepower; enemy kills provide reinforcements while breaches remove soldiers.
- M3: Arithmetic lane gates added (`+`, `-`, `x`) with deterministic application, clear color/text signage, and EditMode coverage for gate math and formation sizing.
- M4: Destructible ice obstacles, screen-shake/haptics service hooks, editor debug hotkeys, and iOS build notes in `docs/IOS_BUILD.md`.
- Visual Polish Pass: upgraded lighting rig, material palette, bridge/city backdrop detail, safer top HUD layout for notched screens, clearer "Drag Left/Right To Move" hint, and improved soldier/zombie/gate/obstacle silhouettes.
- Split-Lane Pass: player now swaps freely between lanes, right lane focuses squad growth + zombie defense, left lane focuses weapon upgrades via breakable reward blocks, and bullet hit logic supports child colliders via parent hittable lookup.
- Combat Progression Pass: weapon tiers now ramp from low-rate rifle to shotgun tiers, bullet throughput is capped for performance with compensated damage scaling, and emergency turret support auto-deploys when overrun.
