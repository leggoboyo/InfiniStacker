using InfiniStacker.Combat;
using InfiniStacker.Core;
using InfiniStacker.Enemy;
using InfiniStacker.Player;
using InfiniStacker.UI;
using InfiniStacker.World;
using TMPro;
using UnityEngine;

namespace InfiniStacker.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private bool _initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureBootstrapInstance()
        {
            if (Object.FindObjectOfType<GameBootstrap>() != null)
            {
                return;
            }

            var bootstrap = new GameObject(\"GameBootstrap\");
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
            GameEvents.ResetAll();

            EnsureCamera();
            EnsureDirectionalLight();

            var moveLimit = BuildBridgeEnvironment();

            var playerRoot = new GameObject("PlayerRoot");
            playerRoot.transform.position = new Vector3(0f, 0f, 0f);

            var playerDragMover = playerRoot.AddComponent<PlayerDragMover>();
            playerDragMover.Configure(moveLimit);
            playerDragMover.SetEnabled(false);

            var playerSquad = playerRoot.AddComponent<PlayerSquad>();
            var autoFire = playerRoot.AddComponent<AutoFireController>();
            playerSquad.SetWorldCountLabel(CreateWorldText("SquadCount", playerRoot.transform, new Vector3(0f, 2.2f, 0f), 5.2f));

            var baseRoot = BuildBaseMech();
            var baseHealth = baseRoot.AddComponent<BaseHealth>();
            CreateWorldText("BaseHpWorld", baseRoot.transform, new Vector3(0f, 3.2f, 0f), 8f)
                .gameObject
                .AddComponent<BaseHpWorldLabel>();

            var poolsRoot = new GameObject("Pools");
            var hitVfxPool = poolsRoot.AddComponent<HitVfxPool>();
            hitVfxPool.Initialize();

            var bulletPool = poolsRoot.AddComponent<BulletPool>();
            bulletPool.Initialize(hitVfxPool);
            autoFire.Initialize(playerSquad, bulletPool);

            var enemyManagerGo = new GameObject("EnemyManager");
            var enemyManager = enemyManagerGo.AddComponent<EnemyManager>();
            enemyManager.Initialize(baseHealth, playerSquad, hitVfxPool);
            enemyManager.SetRunning(false);

            var enemySpawnerGo = new GameObject("EnemySpawner");
            var enemySpawner = enemySpawnerGo.AddComponent<EnemySpawner>();
            enemySpawner.Initialize(enemyManager);
            enemySpawner.SetRunning(false);

            var uiGo = new GameObject("GameUI");
            var uiController = uiGo.AddComponent<GameUIController>();

            var gameStateGo = new GameObject("GameStateController");
            var gameStateController = gameStateGo.AddComponent<GameStateController>();
            gameStateController.Initialize(playerSquad, baseHealth, enemySpawner, enemyManager, autoFire, playerDragMover);

            uiController.Initialize(gameStateController);
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

            camera.transform.position = new Vector3(0f, 9.4f, -12.8f);
            camera.transform.rotation = Quaternion.Euler(28f, 0f, 0f);
            camera.fieldOfView = 46f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.44f, 0.68f, 0.92f);
        }

        private static void EnsureDirectionalLight()
        {
            var existing = Object.FindObjectOfType<Light>();
            if (existing != null)
            {
                existing.type = LightType.Directional;
                existing.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                return;
            }

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.05f;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static float BuildBridgeEnvironment()
        {
            var bridge = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bridge.name = "Bridge";
            bridge.transform.position = new Vector3(0f, -0.1f, 23f);
            bridge.transform.localScale = new Vector3(8.6f, 0.2f, 92f);
            var bridgeRenderer = bridge.GetComponent<Renderer>();
            if (bridgeRenderer != null)
            {
                bridgeRenderer.material.color = new Color(0.5f, 0.5f, 0.5f);
            }

            var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "CenterLine";
            line.transform.SetParent(bridge.transform, false);
            line.transform.localPosition = new Vector3(0f, 0.52f, 0f);
            line.transform.localScale = new Vector3(0.14f, 0.02f, 92f);
            var lineRenderer = line.GetComponent<Renderer>();
            if (lineRenderer != null)
            {
                lineRenderer.material.color = new Color(0.92f, 0.92f, 0.92f);
            }

            CreateRail("RailLeft", -4.42f);
            CreateRail("RailRight", 4.42f);

            CreateWaterPlane("WaterLeft", -13f);
            CreateWaterPlane("WaterRight", 13f);

            return 3.6f;
        }

        private static void CreateRail(string name, float x)
        {
            var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.name = name;
            rail.transform.position = new Vector3(x, 0.55f, 23f);
            rail.transform.localScale = new Vector3(0.12f, 1.1f, 92f);
            var renderer = rail.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.36f, 0.22f, 0.15f);
            }
        }

        private static void CreateWaterPlane(string name, float x)
        {
            var water = GameObject.CreatePrimitive(PrimitiveType.Cube);
            water.name = name;
            water.transform.position = new Vector3(x, -0.55f, 23f);
            water.transform.localScale = new Vector3(16f, 0.1f, 92f);
            var renderer = water.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.24f, 0.54f, 0.84f);
            }
        }

        private static GameObject BuildBaseMech()
        {
            var mechRoot = new GameObject("BaseMech");
            mechRoot.transform.position = new Vector3(-6.4f, 0f, -0.5f);

            var torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
            torso.name = "Torso";
            torso.transform.SetParent(mechRoot.transform, false);
            torso.transform.localScale = new Vector3(2.6f, 2.2f, 1.8f);
            torso.transform.localPosition = new Vector3(0f, 1.5f, 0f);

            var leftLeg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftLeg.transform.SetParent(mechRoot.transform, false);
            leftLeg.transform.localScale = new Vector3(0.8f, 1.3f, 0.8f);
            leftLeg.transform.localPosition = new Vector3(-0.7f, 0.45f, 0f);

            var rightLeg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightLeg.transform.SetParent(mechRoot.transform, false);
            rightLeg.transform.localScale = new Vector3(0.8f, 1.3f, 0.8f);
            rightLeg.transform.localPosition = new Vector3(0.7f, 0.45f, 0f);

            var cannon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cannon.transform.SetParent(mechRoot.transform, false);
            cannon.transform.localScale = new Vector3(0.35f, 0.8f, 0.35f);
            cannon.transform.localPosition = new Vector3(0f, 2.3f, 0f);
            cannon.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            var renderers = mechRoot.GetComponentsInChildren<Renderer>();
            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = new Color(0.68f, 0.88f, 1f, 0.9f);
            }

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
            text.fontSize = 3.2f;
            text.color = Color.white;
            text.outlineColor = new Color(0f, 0f, 0f, 0.75f);
            text.outlineWidth = 0.18f;
            text.text = "0";

            return text;
        }
    }
}
