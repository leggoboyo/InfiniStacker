# iOS Build Notes (Unity Project Version)

This project is offline-only and ships without ads/analytics/IAP/network dependencies.

## 1) Unity Player Settings
1. Open project in Unity version from `InfiniStacker/ProjectSettings/ProjectVersion.txt`.
2. Open `File > Build Settings...` and switch platform to `iOS`.
3. In `Player Settings` configure:
   - `Company Name` / `Product Name`.
   - `Bundle Identifier` (for example `com.yourstudio.infinistacker`).
   - `Version` and `Build` values.
4. Orientation:
   - Default orientation: `Portrait`.
   - Disable landscape autorotation options.
5. Architecture:
   - `Target minimum iOS Version`: choose a modern baseline your devices support.
   - `Architecture`: `ARM64`.
6. Scripting backend:
   - `IL2CPP`.
7. Stripping:
   - Use default Managed Stripping level unless debugging build issues.

## 2) Build From Unity
1. Ensure `Assets/Scenes/Game.unity` exists and is in Build Settings.
2. Click `Build` and select a folder such as `Builds/iOS`.
3. Wait for Unity to generate the Xcode project.

## 3) Xcode Steps
1. Open the generated `.xcodeproj` (or `.xcworkspace` if created).
2. Select the app target and set:
   - Team
   - Signing Certificate
   - Provisioning Profile (automatic signing is fine for local testing)
3. Confirm deployment target and device family (`iPhone`).
4. Select a connected iPhone device and click Run.

## 4) Device Smoke Checklist
1. App launches with start screen visible.
2. Touch drag moves squad left/right.
3. Auto-fire works and enemies spawn/die.
4. Base HP decreases on breaches.
5. Gates modify squad count.
6. Ice obstacles break when shot.
7. Victory at 60s or Game Over on squad/base depletion.
8. Restart flow works from both end states.

## 5) Release Hygiene
1. Confirm no runtime console errors in Unity Play Mode before final build.
2. Confirm no network/ads/analytics/IAP packages were added.
3. Keep visuals lightweight to preserve 60 FPS on modern iPhones.
