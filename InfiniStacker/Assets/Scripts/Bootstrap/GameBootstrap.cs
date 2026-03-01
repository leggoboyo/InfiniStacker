using InfiniStacker.Audio;
using InfiniStacker.Combat;
using InfiniStacker.Core;
using InfiniStacker.DebugTools;
using InfiniStacker.Enemy;
using InfiniStacker.Feedback;
using InfiniStacker.Gates;
using InfiniStacker.Obstacles;
using InfiniStacker.Player;
using InfiniStacker.Upgrades;
using InfiniStacker.UI;
using InfiniStacker.Visual;
using InfiniStacker.World;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace InfiniStacker.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private const float BridgeHalfWidth = 4.25f;
        private const float BridgeLength = 98f;
        private const float BridgeCenterZ = 24f;
        private const float UpgradeLaneCenterX = -2.2f;
        private const float CombatLaneCenterX = 2.2f;
        private const float CombatLaneHalfWidth = 1.25f;
        private const float PlayerStartZ = -4.8f;

        private bool _initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureBootstrapInstance()
        {
            if (Object.FindFirstObjectByType<GameBootstrap>() != null)
            {
                return;
            }

            var bootstrap = new GameObject("GameBootstrap");
            bootstrap.AddComponent<GameBootstrap>();
        }

        private void Awake()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            QualitySettings.shadowDistance = 55f;
            GameEvents.ResetAll();

            ClearLegacySceneRoots();
            EnsureCamera();
            ConfigureRenderSettings();
            EnsurePostProcessingVolume();
            EnsureLightingRig();

            var feedbackRoot = new GameObject("FeedbackServices");
            var screenShakeService = feedbackRoot.AddComponent<ScreenShakeService>();
            screenShakeService.Initialize(Camera.main != null ? Camera.main.transform : null);
            var hapticsService = feedbackRoot.AddComponent<NullHapticsService>();
            FeedbackServices.Configure(screenShakeService, hapticsService);

            _ = feedbackRoot.AddComponent<AudioSettingsService>();

            _ = BuildBridgeEnvironment();

            var playerRoot = new GameObject("PlayerRoot");
            playerRoot.transform.position = new Vector3(CombatLaneCenterX, 0f, PlayerStartZ);

            var playerDragMover = playerRoot.AddComponent<PlayerDragMover>();
            playerDragMover.Configure(-3.25f, 3.25f);
            playerDragMover.SetEnabled(false);

            var playerSquad = playerRoot.AddComponent<PlayerSquad>();
            var autoFire = playerRoot.AddComponent<AutoFireController>();
            playerSquad.SetWorldCountLabel(CreateWorldText("SquadCount", playerRoot.transform, new Vector3(0f, 1.86f, 0f), 2.4f));

            var baseRoot = BuildBaseMech();
            var baseHealth = baseRoot.AddComponent<BaseHealth>();
            var weaponUpgradeSystemGo = new GameObject("WeaponUpgradeSystem");
            var weaponUpgradeSystem = weaponUpgradeSystemGo.AddComponent<WeaponUpgradeSystem>();

            var poolsRoot = new GameObject("Pools");
            var hitVfxPool = poolsRoot.AddComponent<HitVfxPool>();
            hitVfxPool.Initialize();

            var bulletPool = poolsRoot.AddComponent<BulletPool>();
            bulletPool.Initialize(hitVfxPool);

            var enemyManagerGo = new GameObject("EnemyManager");
            var enemyManager = enemyManagerGo.AddComponent<EnemyManager>();
            enemyManager.Initialize(baseHealth, playerSquad, hitVfxPool);
            enemyManager.SetRunning(false);
            autoFire.Initialize(playerSquad, bulletPool, weaponUpgradeSystem, enemyManager);

            var enemySpawnerGo = new GameObject("EnemySpawner");
            var enemySpawner = enemySpawnerGo.AddComponent<EnemySpawner>();
            enemySpawner.Initialize(enemyManager);
            enemySpawner.ConfigureLane(CombatLaneCenterX, CombatLaneHalfWidth - 0.12f);
            enemySpawner.SetRunning(false);

            var gateSpawnerGo = new GameObject("GateSpawner");
            var gateSpawner = gateSpawnerGo.AddComponent<GateSpawner>();
            gateSpawner.Initialize(playerSquad);
            gateSpawner.ConfigureLane(CombatLaneCenterX, 0.82f);
            gateSpawner.SetRunning(false);

            var obstacleSpawnerGo = new GameObject("ObstacleSpawner");
            var obstacleSpawner = obstacleSpawnerGo.AddComponent<ObstacleSpawner>();
            obstacleSpawner.Initialize(playerSquad, hitVfxPool);
            obstacleSpawner.ConfigureLane(CombatLaneCenterX);
            obstacleSpawner.SetRunning(false);

            var upgradeSpawnerGo = new GameObject("UpgradeBlockSpawner");
            var upgradeBlockSpawner = upgradeSpawnerGo.AddComponent<UpgradeBlockSpawner>();
            upgradeBlockSpawner.Initialize(weaponUpgradeSystem, hitVfxPool);
            upgradeBlockSpawner.ConfigureLane(UpgradeLaneCenterX, CombatLaneHalfWidth - 0.1f);
            upgradeBlockSpawner.SetRunning(false);

            var uiGo = new GameObject("GameUI");
            var uiController = uiGo.AddComponent<GameUIController>();

            var gameStateGo = new GameObject("GameStateController");
            var gameStateController = gameStateGo.AddComponent<GameStateController>();
            gameStateController.Initialize(
                playerSquad,
                baseHealth,
                enemySpawner,
                gateSpawner,
                obstacleSpawner,
                upgradeBlockSpawner,
                enemyManager,
                autoFire,
                playerDragMover,
                weaponUpgradeSystem);

            var debugToolsGo = new GameObject("DebugQuickTest");
            var debugQuickTest = debugToolsGo.AddComponent<DebugQuickTestController>();
            debugQuickTest.Initialize(playerSquad, enemySpawner, gateSpawner, obstacleSpawner, upgradeBlockSpawner, baseHealth);

            uiController.Initialize(gameStateController);
        }

        private void ClearLegacySceneRoots()
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (var i = 0; i < roots.Length; i++)
            {
                var root = roots[i];
                if (root == gameObject || root.GetComponent<GameBootstrap>() != null)
                {
                    continue;
                }

                Destroy(root);
            }
        }

        private static void EnsureCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var go = new GameObject("Main Camera");
                camera = go.AddComponent<Camera>();
                go.tag = "MainCamera";
            }

            camera.transform.position = new Vector3(0f, 11.9f, -20.4f);
            camera.transform.rotation = Quaternion.Euler(33.5f, 0f, 0f);
            camera.fieldOfView = 47f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 220f;
            camera.allowHDR = true;
            camera.allowMSAA = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.37f, 0.58f, 0.84f);

            var camData = camera.GetComponent<UniversalAdditionalCameraData>();
            if (camData == null)
            {
                camData = camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            }

            camData.renderPostProcessing = true;
            camData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
            camData.antialiasingQuality = AntialiasingQuality.High;
        }

        private static void ConfigureRenderSettings()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.26f, 0.42f, 0.56f);
            RenderSettings.ambientEquatorColor = new Color(0.17f, 0.22f, 0.28f);
            RenderSettings.ambientGroundColor = new Color(0.08f, 0.1f, 0.12f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.36f, 0.54f, 0.74f);
            RenderSettings.fogDensity = 0.0038f;
            RenderSettings.reflectionIntensity = 0.85f;
        }

        private static void EnsureLightingRig()
        {
            var key = EnsureDirectionalLight("Key Light", new Vector3(48f, -28f, 0f), new Color(1f, 0.97f, 0.9f), 1.08f);
            key.shadows = LightShadows.Soft;
            key.shadowStrength = 0.92f;
            key.shadowResolution = LightShadowResolution.High;

            var fill = EnsureDirectionalLight("Fill Light", new Vector3(14f, 118f, 0f), new Color(0.54f, 0.64f, 0.78f), 0.4f);
            fill.shadows = LightShadows.None;

            var rim = EnsureDirectionalLight("Rim Light", new Vector3(22f, 178f, 0f), new Color(0.56f, 0.72f, 0.92f), 0.35f);
            rim.shadows = LightShadows.None;

            var lights = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (var i = 0; i < lights.Length; i++)
            {
                if (lights[i].type != LightType.Directional)
                {
                    continue;
                }

                var lightName = lights[i].name;
                lights[i].enabled = lightName == "Key Light" || lightName == "Fill Light" || lightName == "Rim Light";
            }
        }

        private static void EnsurePostProcessingVolume()
        {
            var volumeRoot = GameObject.Find("PostProcessVolume") ?? new GameObject("PostProcessVolume");
            var volume = volumeRoot.GetComponent<Volume>();
            if (volume == null)
            {
                volume = volumeRoot.AddComponent<Volume>();
            }

            volume.isGlobal = true;
            volume.priority = 10f;
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            volume.profile = profile;

            if (profile.TryGet(out Bloom bloom) == false)
            {
                bloom = profile.Add<Bloom>(true);
            }

            bloom.active = true;
            bloom.intensity.Override(0.44f);
            bloom.threshold.Override(1.05f);
            bloom.scatter.Override(0.67f);

            if (profile.TryGet(out ColorAdjustments colorAdjustments) == false)
            {
                colorAdjustments = profile.Add<ColorAdjustments>(true);
            }

            colorAdjustments.active = true;
            colorAdjustments.postExposure.Override(0.1f);
            colorAdjustments.contrast.Override(14f);
            colorAdjustments.saturation.Override(6f);

            if (profile.TryGet(out Tonemapping tonemapping) == false)
            {
                tonemapping = profile.Add<Tonemapping>(true);
            }

            tonemapping.active = true;
            tonemapping.mode.Override(TonemappingMode.ACES);

            if (profile.TryGet(out Vignette vignette) == false)
            {
                vignette = profile.Add<Vignette>(true);
            }

            vignette.active = true;
            vignette.intensity.Override(0.16f);
            vignette.smoothness.Override(0.42f);
            vignette.color.Override(new Color(0.06f, 0.08f, 0.12f, 1f));
        }

        private static Light EnsureDirectionalLight(string name, Vector3 eulerAngles, Color color, float intensity)
        {
            var existing = GameObject.Find(name);
            Light light;

            if (existing != null && existing.TryGetComponent<Light>(out var existingLight))
            {
                light = existingLight;
            }
            else
            {
                var go = new GameObject(name);
                light = go.AddComponent<Light>();
                light.type = LightType.Directional;
            }

            light.type = LightType.Directional;
            light.color = color;
            light.intensity = intensity;
            light.transform.rotation = Quaternion.Euler(eulerAngles);
            return light;
        }

        private static float BuildBridgeEnvironment()
        {
            var environmentRoot = new GameObject("Environment").transform;

            var bridge = CreatePrimitive(
                PrimitiveType.Cube,
                "BridgeRoad",
                environmentRoot,
                new Vector3(0f, -0.08f, BridgeCenterZ),
                new Vector3(BridgeHalfWidth * 2f, 0.18f, BridgeLength),
                VisualTheme.Road,
                false);

            CreatePrimitive(
                PrimitiveType.Cube,
                "UpgradeLaneSurface",
                bridge.transform,
                new Vector3(-2.12f, 0.55f, 0f),
                new Vector3(3.95f, 0.01f, BridgeLength),
                VisualTheme.UpgradeLane,
                true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "CombatLaneSurface",
                bridge.transform,
                new Vector3(2.12f, 0.55f, 0f),
                new Vector3(3.95f, 0.01f, BridgeLength),
                VisualTheme.CombatLane,
                true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "RoadShoulderLeft",
                environmentRoot,
                new Vector3(-3.62f, -0.06f, BridgeCenterZ),
                new Vector3(1.1f, 0.12f, BridgeLength),
                VisualTheme.RoadShoulder,
                true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "RoadShoulderRight",
                environmentRoot,
                new Vector3(3.62f, -0.06f, BridgeCenterZ),
                new Vector3(1.1f, 0.12f, BridgeLength),
                VisualTheme.RoadShoulder,
                true);

            for (var z = -18f; z < 70f; z += 2.8f)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"CombatCenterMark_{z:000}",
                    bridge.transform,
                    new Vector3(2.2f, 0.56f, z),
                    new Vector3(0.09f, 0.02f, 1.5f),
                    VisualTheme.LaneMark,
                    true);

                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"CombatEdgeMark_{z:000}",
                    bridge.transform,
                    new Vector3(3.54f, 0.56f, z),
                    new Vector3(0.05f, 0.02f, 1.5f),
                    VisualTheme.LaneMark,
                    true);

                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"UpgradeEdgeMark_{z:000}",
                    bridge.transform,
                    new Vector3(-3.56f, 0.56f, z),
                    new Vector3(0.05f, 0.02f, 1.5f),
                    VisualTheme.LaneMark,
                    true);

                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"LaneSplitMark_{z:000}",
                    bridge.transform,
                    new Vector3(0f, 0.56f, z),
                    new Vector3(0.06f, 0.02f, 1.8f),
                    VisualTheme.LaneMark,
                    true);
            }

            BuildBridgeRails(environmentRoot);
            BuildWater(environmentRoot);
            BuildCityBackdrop(environmentRoot);

            return BridgeHalfWidth;
        }

        private static void BuildBridgeRails(Transform parent)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                "RailTopLeft",
                parent,
                new Vector3(-4.42f, 0.86f, BridgeCenterZ),
                new Vector3(0.12f, 0.08f, BridgeLength),
                VisualTheme.RailMetal,
                true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "RailTopRight",
                parent,
                new Vector3(4.42f, 0.86f, BridgeCenterZ),
                new Vector3(0.12f, 0.08f, BridgeLength),
                VisualTheme.RailMetal,
                true);

            for (var z = -24f; z <= 72f; z += 4f)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"RailPostL_{z:000}",
                    parent,
                    new Vector3(-4.42f, 0.36f, z),
                    new Vector3(0.07f, 0.88f, 0.07f),
                    VisualTheme.RailMetal,
                    true);

                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"RailPostR_{z:000}",
                    parent,
                    new Vector3(4.42f, 0.36f, z),
                    new Vector3(0.07f, 0.88f, 0.07f),
                    VisualTheme.RailMetal,
                    true);
            }
        }

        private static void BuildWater(Transform parent)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                "WaterLeft",
                parent,
                new Vector3(-13f, -0.72f, BridgeCenterZ),
                new Vector3(17f, 0.16f, BridgeLength + 10f),
                VisualTheme.Water,
                true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "WaterRight",
                parent,
                new Vector3(13f, -0.72f, BridgeCenterZ),
                new Vector3(17f, 0.16f, BridgeLength + 10f),
                VisualTheme.Water,
                true);
        }

        private static void BuildCityBackdrop(Transform parent)
        {
            var leftCity = new GameObject("CityLeft").transform;
            leftCity.SetParent(parent, false);
            leftCity.localPosition = new Vector3(-24f, -0.2f, 28f);

            var rightCity = new GameObject("CityRight").transform;
            rightCity.SetParent(parent, false);
            rightCity.localPosition = new Vector3(24f, -0.2f, 28f);

            for (var i = 0; i < 34; i++)
            {
                var depth = i * 4.5f;
                var height = 4f + ((i * 37) % 11);
                var width = 1.8f + ((i * 19) % 5) * 0.3f;

                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"TowerL_{i:00}",
                    leftCity,
                    new Vector3(-((i % 4) * 1.8f), height * 0.5f, depth),
                    new Vector3(width, height, width),
                    VisualTheme.City,
                    true);

                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"TowerR_{i:00}",
                    rightCity,
                    new Vector3((i % 4) * 1.8f, height * 0.5f, depth),
                    new Vector3(width, height, width),
                    VisualTheme.City,
                    true);
            }
        }

        private static GameObject BuildBaseMech()
        {
            var mechRoot = new GameObject("BaseMech");
            mechRoot.transform.position = new Vector3(-3.1f, 0f, -2.8f);

            _ = CreatePrimitive(
                PrimitiveType.Cube,
                "Pedestal",
                mechRoot.transform,
                new Vector3(0f, 0.2f, 0f),
                new Vector3(3.4f, 0.4f, 2.3f),
                VisualTheme.GateFrame,
                true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "Torso",
                mechRoot.transform,
                new Vector3(0f, 1.86f, 0f),
                new Vector3(2.36f, 2.25f, 1.74f),
                VisualTheme.MechBody,
                true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "Cockpit",
                mechRoot.transform,
                new Vector3(0f, 2.34f, 0.72f),
                new Vector3(1.26f, 0.95f, 0.64f),
                VisualTheme.MechDark,
                true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "LeftLeg",
                mechRoot.transform,
                new Vector3(-0.72f, 0.74f, 0f),
                new Vector3(0.82f, 1.34f, 0.86f),
                VisualTheme.MechBody,
                true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "RightLeg",
                mechRoot.transform,
                new Vector3(0.72f, 0.74f, 0f),
                new Vector3(0.82f, 1.34f, 0.86f),
                VisualTheme.MechBody,
                true);

            CreatePrimitive(
                PrimitiveType.Cylinder,
                "LeftCannon",
                mechRoot.transform,
                new Vector3(-1.12f, 2.08f, 0f),
                new Vector3(0.24f, 0.94f, 0.24f),
                VisualTheme.MechDark,
                true).transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            CreatePrimitive(
                PrimitiveType.Cylinder,
                "RightCannon",
                mechRoot.transform,
                new Vector3(1.12f, 2.08f, 0f),
                new Vector3(0.24f, 0.94f, 0.24f),
                VisualTheme.MechDark,
                true).transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            CreatePrimitive(
                PrimitiveType.Sphere,
                "Core",
                mechRoot.transform,
                new Vector3(0f, 1.9f, 0.58f),
                new Vector3(0.46f, 0.46f, 0.46f),
                VisualTheme.SoldierVisor,
                true);

            return mechRoot;
        }

        private static TextMeshPro CreateWorldText(string name, Transform parent, Vector3 localPosition, float scale)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = Vector3.one * scale;

            var text = go.AddComponent<TextMeshPro>();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 2.2f;
            text.color = Color.white;
            text.outlineColor = new Color(0f, 0f, 0f, 0.76f);
            text.outlineWidth = 0.18f;
            text.text = "0";

            return text;
        }

        private static GameObject CreatePrimitive(
            PrimitiveType primitiveType,
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Material material,
            bool removeCollider)
        {
            var go = GameObject.CreatePrimitive(primitiveType);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                VisualTheme.ApplyMaterial(renderer, material);
            }

            if (removeCollider)
            {
                var collider = go.GetComponent<Collider>();
                if (collider != null)
                {
                    Object.Destroy(collider);
                }
            }

            return go;
        }
    }
}
