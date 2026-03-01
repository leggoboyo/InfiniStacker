using System.Collections.Generic;
using InfiniStacker.Combat;
using InfiniStacker.Feedback;
using InfiniStacker.Player;
using InfiniStacker.Visual;
using TMPro;
using UnityEngine;

namespace InfiniStacker.Obstacles
{
    public sealed class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 4.6f;
        [SerializeField] private float moveSpeed = 4.1f;
        [SerializeField] private float spawnZ = 32f;
        [SerializeField] private float despawnZ = -6f;
        [SerializeField] private int obstacleHp = 14;
        [SerializeField] private int collisionSquadLoss = 3;
        [SerializeField] private float laneCenterX = 2.2f;

        private static readonly float[] LaneOffsets = { -0.82f, 0f, 0.82f };

        private readonly List<IceObstacle> _activeObstacles = new();
        private readonly Queue<IceObstacle> _obstaclePool = new();

        private PlayerSquad _playerSquad;
        private HitVfxPool _hitVfxPool;
        private bool _isRunning;
        private float _timer;

        public void Initialize(PlayerSquad playerSquad, HitVfxPool hitVfxPool)
        {
            _playerSquad = playerSquad;
            _hitVfxPool = hitVfxPool;
        }

        public void ConfigureLane(float centerX)
        {
            laneCenterX = centerX;
        }

        public void SetRunning(bool isRunning)
        {
            _isRunning = isRunning;
        }

        public void ResetSpawner()
        {
            _timer = 0f;
            for (var i = _activeObstacles.Count - 1; i >= 0; i--)
            {
                Recycle(_activeObstacles[i]);
            }

            _activeObstacles.Clear();
        }

        public void SpawnImmediateObstacle()
        {
            SpawnObstacle();
        }

        public void NotifyDestroyed(IceObstacle obstacle, Vector3 hitPoint)
        {
            if (!RemoveActive(obstacle))
            {
                return;
            }

            _hitVfxPool?.Spawn(hitPoint);
            FeedbackServices.ScreenShake?.Shake(0.08f, 0.08f);
            FeedbackServices.Haptics?.LightImpact();
            Recycle(obstacle);
        }

        private void Update()
        {
            if (!_isRunning || _playerSquad == null)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= spawnInterval)
            {
                _timer -= spawnInterval;
                SpawnObstacle();
            }

            for (var i = _activeObstacles.Count - 1; i >= 0; i--)
            {
                var obstacle = _activeObstacles[i];
                obstacle.Tick(Time.deltaTime, moveSpeed);

                var worldPos = obstacle.transform.position;
                if (worldPos.z <= _playerSquad.transform.position.z + 0.1f)
                {
                    if (Mathf.Abs(worldPos.x - _playerSquad.transform.position.x) <= 1.05f)
                    {
                        _playerSquad.RemoveSoldiers(collisionSquadLoss);
                        FeedbackServices.ScreenShake?.Shake(0.18f, 0.12f);
                        FeedbackServices.Haptics?.MediumImpact();
                    }

                    RecycleAndRemoveAt(i);
                    continue;
                }

                if (worldPos.z <= despawnZ)
                {
                    RecycleAndRemoveAt(i);
                }
            }
        }

        private void SpawnObstacle()
        {
            var obstacle = _obstaclePool.Count > 0 ? _obstaclePool.Dequeue() : CreateObstacle();
            var laneIndex = Random.Range(0, LaneOffsets.Length);
            var x = laneCenterX + LaneOffsets[laneIndex];

            obstacle.transform.position = new Vector3(x, 0.95f, spawnZ);
            var hpLabel = obstacle.GetComponentInChildren<TMP_Text>();
            obstacle.Initialize(this, obstacleHp, hpLabel);
            _activeObstacles.Add(obstacle);
        }

        private void RecycleAndRemoveAt(int index)
        {
            var obstacle = _activeObstacles[index];
            var last = _activeObstacles.Count - 1;
            _activeObstacles[index] = _activeObstacles[last];
            _activeObstacles.RemoveAt(last);
            Recycle(obstacle);
        }

        private bool RemoveActive(IceObstacle obstacle)
        {
            var index = _activeObstacles.IndexOf(obstacle);
            if (index < 0)
            {
                return false;
            }

            var last = _activeObstacles.Count - 1;
            _activeObstacles[index] = _activeObstacles[last];
            _activeObstacles.RemoveAt(last);
            return true;
        }

        private void Recycle(IceObstacle obstacle)
        {
            obstacle.MarkInactive();
            _obstaclePool.Enqueue(obstacle);
        }

        private IceObstacle CreateObstacle()
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = "IceObstacle";
            block.transform.SetParent(transform, false);
            block.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);

            var renderer = block.GetComponent<Renderer>();
            if (renderer != null)
            {
                VisualTheme.ApplyMaterial(renderer, VisualTheme.Ice);
            }

            for (var i = 0; i < 4; i++)
            {
                var shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shard.name = $"Shard_{i}";
                shard.transform.SetParent(block.transform, false);
                shard.transform.localScale = new Vector3(0.54f, 0.92f, 0.54f);
                shard.transform.localPosition = new Vector3(
                    (i - 1.5f) * 0.28f,
                    0.25f + ((i % 2) * 0.18f),
                    (i % 2 == 0 ? -0.2f : 0.2f));
                shard.transform.localRotation = Quaternion.Euler(12f * i, 25f * i, 8f * i);

                var shardRenderer = shard.GetComponent<Renderer>();
                if (shardRenderer != null)
                {
                    VisualTheme.ApplyMaterial(shardRenderer, VisualTheme.Ice);
                }

                var shardCollider = shard.GetComponent<Collider>();
                if (shardCollider != null)
                {
                    Destroy(shardCollider);
                }
            }

            var hpLabelGo = new GameObject("HpLabel");
            hpLabelGo.transform.SetParent(block.transform, false);
            hpLabelGo.transform.localPosition = new Vector3(0f, 0.82f, 0f);
            hpLabelGo.transform.localScale = Vector3.one * 0.2f;

            var hpBackplate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hpBackplate.name = "LabelBackplate";
            hpBackplate.transform.SetParent(hpLabelGo.transform, false);
            hpBackplate.transform.localPosition = new Vector3(0f, 0f, 0.38f);
            hpBackplate.transform.localScale = new Vector3(2.6f, 1.3f, 0.2f);
            var hpBackplateRenderer = hpBackplate.GetComponent<Renderer>();
            if (hpBackplateRenderer != null)
            {
                VisualTheme.ApplyMaterial(hpBackplateRenderer, VisualTheme.GateFrame);
            }

            var hpBackplateCollider = hpBackplate.GetComponent<Collider>();
            if (hpBackplateCollider != null)
            {
                Destroy(hpBackplateCollider);
            }

            var hpText = hpLabelGo.AddComponent<TextMeshPro>();
            hpText.fontSize = 10f;
            hpText.alignment = TextAlignmentOptions.Center;
            hpText.color = Color.white;
            hpText.outlineColor = new Color(0f, 0f, 0f, 0.7f);
            hpText.outlineWidth = 0.2f;

            var obstacle = block.AddComponent<IceObstacle>();
            block.SetActive(false);
            return obstacle;
        }
    }
}
