using UnityEngine;

namespace InfiniStacker.Enemy
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 1.2f;
        [SerializeField] private int minGroupSize = 5;
        [SerializeField] private int maxGroupSize = 9;
        [SerializeField] private float spawnZ = 46f;
        [SerializeField] private float laneHalfWidth = 3.4f;
        [SerializeField] private int enemyHp = 3;
        [SerializeField] private float enemySpeed = 3.3f;

        private EnemyManager _enemyManager;
        private bool _isRunning;
        private float _spawnTimer;

        public void Initialize(EnemyManager enemyManager)
        {
            _enemyManager = enemyManager;
        }

        public void SetRunning(bool isRunning)
        {
            _isRunning = isRunning;
        }

        public void ResetSpawner()
        {
            _spawnTimer = 0f;
        }

        public void SpawnImmediateWave(int enemyCount)
        {
            SpawnWave(Mathf.Max(1, enemyCount));
        }

        private void Update()
        {
            if (!_isRunning || _enemyManager == null)
            {
                return;
            }

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer < spawnInterval)
            {
                return;
            }

            _spawnTimer -= spawnInterval;
            var groupSize = Random.Range(minGroupSize, maxGroupSize + 1);
            SpawnWave(groupSize);
        }

        private void SpawnWave(int count)
        {
            if (_enemyManager == null)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                var laneOffset = Random.Range(-laneHalfWidth, laneHalfWidth);
                var zJitter = Random.Range(0f, 4.2f);
                var spawnPos = new Vector3(laneOffset, 0f, spawnZ + zJitter);
                if (!_enemyManager.SpawnEnemy(spawnPos, enemyHp, enemySpeed))
                {
                    break;
                }
            }
        }
    }
}
