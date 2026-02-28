using System.Collections.Generic;
using InfiniStacker.Combat;
using InfiniStacker.Player;
using InfiniStacker.World;
using UnityEngine;

namespace InfiniStacker.Enemy
{
    public sealed class EnemyManager : MonoBehaviour
    {
        [SerializeField] private int maxActiveEnemies = 72;
        [SerializeField] private float breachZ = -1.8f;
        [SerializeField] private int breachBaseDamage = 10;
        [SerializeField] private int breachSquadLoss = 1;

        private readonly List<EnemyAgent> _activeEnemies = new();
        private readonly Queue<EnemyAgent> _enemyPool = new();

        private BaseHealth _baseHealth;
        private PlayerSquad _playerSquad;
        private HitVfxPool _hitVfxPool;
        private bool _isRunning;

        public int ActiveEnemyCount => _activeEnemies.Count;

        public void Initialize(BaseHealth baseHealth, PlayerSquad playerSquad, HitVfxPool hitVfxPool)
        {
            _baseHealth = baseHealth;
            _playerSquad = playerSquad;
            _hitVfxPool = hitVfxPool;
        }

        public void SetRunning(bool isRunning)
        {
            _isRunning = isRunning;
        }

        public void ResetAll()
        {
            for (var i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                Recycle(_activeEnemies[i]);
            }

            _activeEnemies.Clear();
        }

        public bool SpawnEnemy(Vector3 worldPosition, int hp, float speed)
        {
            if (_activeEnemies.Count >= maxActiveEnemies)
            {
                return false;
            }

            var enemy = _enemyPool.Count > 0 ? _enemyPool.Dequeue() : CreateEnemy();
            enemy.transform.position = worldPosition;
            enemy.Initialize(this, hp, speed);
            enemy.gameObject.SetActive(true);
            _activeEnemies.Add(enemy);
            return true;
        }

        private void Update()
        {
            if (!_isRunning || _activeEnemies.Count == 0)
            {
                return;
            }

            for (var i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                _activeEnemies[i].Tick(Time.deltaTime, breachZ);
            }
        }

        public void NotifyKilled(EnemyAgent enemy)
        {
            if (enemy == null || !RemoveActive(enemy))
            {
                return;
            }

            _hitVfxPool?.Spawn(enemy.transform.position);
            Recycle(enemy);
        }

        public void NotifyBreach(EnemyAgent enemy)
        {
            if (enemy == null || !RemoveActive(enemy))
            {
                return;
            }

            _playerSquad?.RemoveSoldiers(breachSquadLoss);
            _baseHealth?.ApplyDamage(breachBaseDamage);
            Recycle(enemy);
        }

        private bool RemoveActive(EnemyAgent enemy)
        {
            var index = _activeEnemies.IndexOf(enemy);
            if (index < 0)
            {
                return false;
            }

            var lastIndex = _activeEnemies.Count - 1;
            _activeEnemies[index] = _activeEnemies[lastIndex];
            _activeEnemies.RemoveAt(lastIndex);
            return true;
        }

        private void Recycle(EnemyAgent enemy)
        {
            enemy.MarkInactive();
            _enemyPool.Enqueue(enemy);
        }

        private EnemyAgent CreateEnemy()
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Enemy";
            body.transform.SetParent(transform, false);
            body.transform.localScale = new Vector3(0.72f, 0.95f, 0.72f);

            var renderer = body.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.43f, 0.68f, 0.43f);
            }

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(body.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.62f, 0f);
            head.transform.localScale = new Vector3(0.42f, 0.32f, 0.42f);
            var headCollider = head.GetComponent<Collider>();
            if (headCollider != null)
            {
                Destroy(headCollider);
            }

            var headRenderer = head.GetComponent<Renderer>();
            if (headRenderer != null)
            {
                headRenderer.material.color = new Color(0.58f, 0.74f, 0.54f);
            }

            var agent = body.AddComponent<EnemyAgent>();
            body.SetActive(false);
            return agent;
        }
    }
}
