using System.Collections.Generic;
using InfiniStacker.Feedback;
using InfiniStacker.Player;
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
        [SerializeField] private float laneOffsetX = 2.2f;
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
                pair.LeftObject.transform.position = new Vector3(-laneOffsetX, gateY, pair.Z);
                pair.RightObject.transform.position = new Vector3(laneOffsetX, gateY, pair.Z);

                if (!pair.Applied && pair.Z <= _playerSquad.transform.position.z + 0.1f)
                {
                    var chooseLeft = _playerSquad.transform.position.x < 0f;
                    var selectedOperation = chooseLeft ? pair.LeftOperation : pair.RightOperation;
                    _playerSquad.ApplyGateOperation(selectedOperation);
                    pair.Applied = true;
                    FeedbackServices.ScreenShake?.Shake(0.07f, 0.07f);
                    FeedbackServices.Haptics?.LightImpact();

                    pair.LeftRenderer.material.color = new Color(0.35f, 0.35f, 0.35f);
                    pair.RightRenderer.material.color = new Color(0.35f, 0.35f, 0.35f);
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
            var leftGate = CreateGateObject($"Gate_{pairId}_L", left, new Vector3(-laneOffsetX, gateY, z));
            var rightGate = CreateGateObject($"Gate_{pairId}_R", right, new Vector3(laneOffsetX, gateY, z));

            var runtime = new GatePairRuntime
            {
                Z = z,
                LeftOperation = left,
                RightOperation = right,
                LeftObject = leftGate,
                RightObject = rightGate,
                LeftRenderer = leftGate.GetComponent<Renderer>(),
                RightRenderer = rightGate.GetComponent<Renderer>(),
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

        private static GameObject CreateGateObject(string name, GateOperation operation, Vector3 position)
        {
            var gate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gate.name = name;
            gate.transform.position = position;
            gate.transform.localScale = new Vector3(2.4f, 2f, 0.3f);

            var collider = gate.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            var renderer = gate.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = operation.Type switch
                {
                    GateOperationType.Add => new Color(0.18f, 0.72f, 0.9f),
                    GateOperationType.Multiply => new Color(0.24f, 0.74f, 0.38f),
                    _ => new Color(0.92f, 0.33f, 0.25f)
                };
            }

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(gate.transform, false);
            textGo.transform.localPosition = new Vector3(0f, 0f, -0.18f);
            textGo.transform.localScale = Vector3.one * 0.34f;

            var text = textGo.AddComponent<TextMeshPro>();
            text.text = operation.ToDisplayText();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 9f;
            text.color = Color.white;
            text.outlineColor = new Color(0f, 0f, 0f, 0.8f);
            text.outlineWidth = 0.25f;

            return gate;
        }

        private struct GatePairRuntime
        {
            public float Z;
            public GateOperation LeftOperation;
            public GateOperation RightOperation;
            public GameObject LeftObject;
            public GameObject RightObject;
            public Renderer LeftRenderer;
            public Renderer RightRenderer;
            public bool Applied;
        }
    }
}
