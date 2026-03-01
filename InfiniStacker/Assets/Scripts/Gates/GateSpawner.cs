using System.Collections.Generic;
using InfiniStacker.Feedback;
using InfiniStacker.Player;
using InfiniStacker.Visual;
using TMPro;
using UnityEngine;

namespace InfiniStacker.Gates
{
    public sealed class GateSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 5.5f;
        [SerializeField] private float gateSpeed = 4.3f;
        [SerializeField] private float spawnZ = 27f;
        [SerializeField] private float despawnZ = -5.5f;
        [SerializeField] private float laneCenterX = 2.2f;
        [SerializeField] private float laneChoiceOffsetX = 0.82f;
        [SerializeField] private float activationHalfWidth = 1.65f;
        [SerializeField] private float gateY = 1.05f;

        private readonly List<GatePairRuntime> _activePairs = new();
        private PlayerSquad _playerSquad;
        private bool _isRunning;
        private float _spawnTimer;
        private int _nextPairId;

        public void Initialize(PlayerSquad playerSquad)
        {
            _playerSquad = playerSquad;
        }

        public void ConfigureLane(float centerX, float choiceOffsetX)
        {
            laneCenterX = centerX;
            laneChoiceOffsetX = Mathf.Clamp(choiceOffsetX, 0.35f, 1.6f);
        }

        public void SetRunning(bool isRunning)
        {
            _isRunning = isRunning;
        }

        public void ResetSpawner()
        {
            _spawnTimer = 0f;
            for (var i = _activePairs.Count - 1; i >= 0; i--)
            {
                DestroyPair(_activePairs[i]);
            }

            _activePairs.Clear();
        }

        public void SpawnImmediatePair()
        {
            SpawnPair();
        }

        private void Update()
        {
            if (!_isRunning || _playerSquad == null)
            {
                return;
            }

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= spawnInterval)
            {
                _spawnTimer -= spawnInterval;
                SpawnPair();
            }

            for (var i = _activePairs.Count - 1; i >= 0; i--)
            {
                var pair = _activePairs[i];
                pair.Z -= gateSpeed * Time.deltaTime;
                pair.LeftObject.transform.position = new Vector3(laneCenterX - laneChoiceOffsetX, gateY, pair.Z);
                pair.RightObject.transform.position = new Vector3(laneCenterX + laneChoiceOffsetX, gateY, pair.Z);

                if (!pair.Applied && pair.Z <= _playerSquad.transform.position.z + 0.1f)
                {
                    if (Mathf.Abs(_playerSquad.transform.position.x - laneCenterX) <= activationHalfWidth)
                    {
                        var chooseLeft = _playerSquad.transform.position.x < laneCenterX;
                        var selectedOperation = chooseLeft ? pair.LeftOperation : pair.RightOperation;
                        _playerSquad.ApplyGateOperation(selectedOperation);
                        FeedbackServices.ScreenShake?.Shake(0.07f, 0.07f);
                        FeedbackServices.Haptics?.LightImpact();
                    }

                    pair.Applied = true;

                    if (pair.LeftSignRenderer != null)
                    {
                        VisualTheme.ApplyMaterial(pair.LeftSignRenderer, VisualTheme.GateUsed);
                    }

                    if (pair.RightSignRenderer != null)
                    {
                        VisualTheme.ApplyMaterial(pair.RightSignRenderer, VisualTheme.GateUsed);
                    }
                }

                if (pair.Z <= despawnZ)
                {
                    DestroyPair(pair);
                    _activePairs.RemoveAt(i);
                    continue;
                }

                _activePairs[i] = pair;
            }
        }

        private void SpawnPair()
        {
            var (left, right) = CreateOperationPair();
            var z = spawnZ;
            var pairId = ++_nextPairId;
            var leftGate = CreateGateObject(
                $"Gate_{pairId}_L",
                left,
                new Vector3(laneCenterX - laneChoiceOffsetX, gateY, z),
                out var leftSignRenderer);
            var rightGate = CreateGateObject(
                $"Gate_{pairId}_R",
                right,
                new Vector3(laneCenterX + laneChoiceOffsetX, gateY, z),
                out var rightSignRenderer);

            var runtime = new GatePairRuntime
            {
                Z = z,
                LeftOperation = left,
                RightOperation = right,
                LeftObject = leftGate,
                RightObject = rightGate,
                LeftSignRenderer = leftSignRenderer,
                RightSignRenderer = rightSignRenderer,
                Applied = false
            };

            _activePairs.Add(runtime);
        }

        private static void DestroyPair(GatePairRuntime pair)
        {
            if (pair.LeftObject != null)
            {
                Destroy(pair.LeftObject);
            }

            if (pair.RightObject != null)
            {
                Destroy(pair.RightObject);
            }
        }

        private static (GateOperation left, GateOperation right) CreateOperationPair()
        {
            var positive = Random.value < 0.58f
                ? new GateOperation(GateOperationType.Add, Random.Range(2, 9))
                : new GateOperation(GateOperationType.Multiply, Random.Range(2, 4));

            var negative = new GateOperation(GateOperationType.Subtract, Random.Range(2, 9));

            return Random.value < 0.5f ? (positive, negative) : (negative, positive);
        }

        private static GameObject CreateGateObject(string name, GateOperation operation, Vector3 position, out Renderer signRenderer)
        {
            var root = new GameObject(name);
            root.transform.position = position;

            _ = CreatePart(
                PrimitiveType.Cube,
                "FrameLeft",
                root.transform,
                new Vector3(-0.72f, 0f, 0f),
                new Vector3(0.13f, 2.2f, 0.25f),
                VisualTheme.GateFrame,
                true);

            _ = CreatePart(
                PrimitiveType.Cube,
                "FrameRight",
                root.transform,
                new Vector3(0.72f, 0f, 0f),
                new Vector3(0.13f, 2.2f, 0.25f),
                VisualTheme.GateFrame,
                true);

            _ = CreatePart(
                PrimitiveType.Cube,
                "FrameTop",
                root.transform,
                new Vector3(0f, 1.06f, 0f),
                new Vector3(1.62f, 0.15f, 0.25f),
                VisualTheme.GateFrame,
                true);

            var gateMaterial = operation.Type switch
            {
                GateOperationType.Add => VisualTheme.GateAdd,
                GateOperationType.Multiply => VisualTheme.GateMultiply,
                _ => VisualTheme.GateSubtract
            };

            var sign = CreatePart(
                PrimitiveType.Cube,
                "Sign",
                root.transform,
                new Vector3(0f, 0f, -0.02f),
                new Vector3(1.36f, 1.76f, 0.22f),
                gateMaterial,
                true);

            signRenderer = sign.GetComponent<Renderer>();

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(root.transform, false);
            textGo.transform.localPosition = new Vector3(0f, 0f, -0.18f);
            textGo.transform.localScale = Vector3.one * 0.28f;

            var text = textGo.AddComponent<TextMeshPro>();
            text.text = operation.ToDisplayText();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 11f;
            text.color = Color.white;
            text.outlineColor = new Color(0f, 0f, 0f, 0.82f);
            text.outlineWidth = 0.22f;

            return root;
        }

        private static GameObject CreatePart(
            PrimitiveType primitiveType,
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Material material,
            bool removeCollider)
        {
            var part = GameObject.CreatePrimitive(primitiveType);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;

            var renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                VisualTheme.ApplyMaterial(renderer, material);
            }

            if (removeCollider)
            {
                var collider = part.GetComponent<Collider>();
                if (collider != null)
                {
                    Destroy(collider);
                }
            }

            return part;
        }

        private struct GatePairRuntime
        {
            public float Z;
            public GateOperation LeftOperation;
            public GateOperation RightOperation;
            public GameObject LeftObject;
            public GameObject RightObject;
            public Renderer LeftSignRenderer;
            public Renderer RightSignRenderer;
            public bool Applied;
        }
    }
}
