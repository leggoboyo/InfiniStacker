using UnityEngine;

namespace InfiniStacker.Enemy
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 1.4f;
        [SerializeField] private int minGroupSize = 3;
        [SerializeField] private int maxGroupSize = 5;
        [SerializeField] private int maxExtraPerWaveOverTime = 6;
        [SerializeField] private float spawnZ = 46f;
        [SerializeField] private float laneCenterX = 2.2f;
        [SerializeField] private float laneHalfWidth = 1.2f;
        [SerializeField] private int enemyHp = 3;
        [SerializeField] private float enemySpeed = 3.3f;
        [SerializeField] private float maxSpeedBonusOverTime = 2f;

        private EnemyManager _enemyManager;
        private bool _isRunning;
        private float _spawnTimer;
        private float _elapsedTime;

        public void Initialize(EnemyManager enemyManager)
        {
            _enemyManager = enemyManager;
        }

        public void ConfigureLane(float centerX, float halfWidth)
        {
            laneCenterX = centerX;
            laneHalfWidth = Mathf.Clamp(halfWidth, 0.35f, 3.5f);
        }

        public void SetRunning(bool isRunning)
        {
            _isRunning = isRunning;
        }

        public void ResetSpawner()
        {
            _spawnTimer = 0f;
            _elapsedTime = 0f;
        }

        public void SpawnImmediateWave(int enemyCount)
        {
            SpawnWave(Mathf.Max(1, enemyCount), enemyHp, enemySpeed);
        }

        private void Update()
        {
            if (!_isRunning || _enemyManager == null)
            {
                return;
            }

            _elapsedTime += Time.deltaTime;
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer < spawnInterval)
            {
                return;
            }

            _spawnTimer -= spawnInterval;
            var intensity = Mathf.Clamp01(_elapsedTime / 60f);
            var extra = Mathf.RoundToInt(maxExtraPerWaveOverTime * intensity);
            var groupSize = Random.Range(minGroupSize, maxGroupSize + 1 + extra);
            var scaledHp = enemyHp + Mathf.FloorToInt(intensity * 2f);
            var scaledSpeed = enemySpeed + (maxSpeedBonusOverTime * intensity);
            SpawnWave(groupSize, scaledHp, scaledSpeed);
        }

        private void SpawnWave(int count, int hp, float speed)
        {
            if (_enemyManager == null)
            {
                return;
            }

            var clusterCenter = laneCenterX + Random.Range(-laneHalfWidth * 0.45f, laneHalfWidth * 0.45f);
            var minX = laneCenterX - laneHalfWidth;
            var maxX = laneCenterX + laneHalfWidth;

            for (var i = 0; i < count; i++)
            {
                var spread = Random.Range(-0.85f, 0.85f);
                var x = Mathf.Clamp(clusterCenter + spread, minX, maxX);
                var zJitter = Random.Range(0f, 2.8f) + (i * 0.18f);
                var spawnPos = new Vector3(x, 0f, spawnZ + zJitter);
                if (!_enemyManager.SpawnEnemy(spawnPos, hp, speed))
                {
                    break;
                }
            }
        }
    }
}
