using System.Collections.Generic;
using InfiniStacker.Combat;
using InfiniStacker.Feedback;
using InfiniStacker.Player;
using InfiniStacker.Visual;
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
        [SerializeField] private int killsPerReinforcement = 5;

        private readonly List<EnemyAgent> _activeEnemies = new();
        private readonly Queue<EnemyAgent> _enemyPool = new();

        private BaseHealth _baseHealth;
        private PlayerSquad _playerSquad;
        private HitVfxPool _hitVfxPool;
        private bool _isRunning;
        private int _killsTowardReinforcement;

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
            _killsTowardReinforcement = 0;
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
            enemy.transform.localScale = Vector3.one * Random.Range(0.9f, 1.08f);
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
            _killsTowardReinforcement++;
            if (_killsTowardReinforcement >= Mathf.Max(1, killsPerReinforcement))
            {
                _killsTowardReinforcement = 0;
                _playerSquad?.AddSoldiers(1);
                FeedbackServices.Haptics?.LightImpact();
            }

            FeedbackServices.ScreenShake?.Shake(0.06f, 0.07f);
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
            FeedbackServices.ScreenShake?.Shake(0.12f, 0.1f);
            FeedbackServices.Haptics?.MediumImpact();
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
            var enemyRoot = new GameObject("Enemy");
            enemyRoot.transform.SetParent(transform, false);

            var hitBody = CreatePart(
                PrimitiveType.Capsule,
                "HitBody",
                enemyRoot.transform,
                new Vector3(0f, 0.58f, 0f),
                new Vector3(0.42f, 0.72f, 0.42f),
                VisualTheme.ZombieCloth,
                false);
            var hitBodyRenderer = hitBody.GetComponent<Renderer>();
            if (hitBodyRenderer != null)
            {
                hitBodyRenderer.enabled = false;
            }

            _ = CreatePart(
                PrimitiveType.Cube,
                "Chest",
                enemyRoot.transform,
                new Vector3(0f, 0.67f, 0.02f),
                new Vector3(0.45f, 0.44f, 0.28f),
                VisualTheme.ZombieCloth,
                true);

            _ = CreatePart(
                PrimitiveType.Cube,
                "Belly",
                enemyRoot.transform,
                new Vector3(0f, 0.46f, 0.03f),
                new Vector3(0.38f, 0.26f, 0.24f),
                VisualTheme.ZombieBody,
                true);

            _ = CreatePart(
                PrimitiveType.Cube,
                "Pelvis",
                enemyRoot.transform,
                new Vector3(0f, 0.3f, 0f),
                new Vector3(0.32f, 0.19f, 0.24f),
                VisualTheme.ZombiePants,
                true);

            _ = CreatePart(
                PrimitiveType.Capsule,
                "LeftLeg",
                enemyRoot.transform,
                new Vector3(-0.12f, 0.19f, 0f),
                new Vector3(0.14f, 0.3f, 0.14f),
                VisualTheme.ZombiePants,
                true);

            _ = CreatePart(
                PrimitiveType.Capsule,
                "RightLeg",
                enemyRoot.transform,
                new Vector3(0.12f, 0.19f, 0f),
                new Vector3(0.14f, 0.3f, 0.14f),
                VisualTheme.ZombiePants,
                true);

            _ = CreatePart(
                PrimitiveType.Cube,
                "LeftShoe",
                enemyRoot.transform,
                new Vector3(-0.12f, 0.02f, 0.06f),
                new Vector3(0.16f, 0.08f, 0.24f),
                VisualTheme.ZombieShoes,
                true);

            _ = CreatePart(
                PrimitiveType.Cube,
                "RightShoe",
                enemyRoot.transform,
                new Vector3(0.12f, 0.02f, 0.06f),
                new Vector3(0.16f, 0.08f, 0.24f),
                VisualTheme.ZombieShoes,
                true);

            _ = CreatePart(
                PrimitiveType.Capsule,
                "LeftArm",
                enemyRoot.transform,
                new Vector3(-0.31f, 0.66f, 0.08f),
                new Vector3(0.1f, 0.3f, 0.1f),
                VisualTheme.ZombieBody,
                true).transform.localRotation = Quaternion.Euler(8f, 0f, 28f);

            _ = CreatePart(
                PrimitiveType.Capsule,
                "RightArm",
                enemyRoot.transform,
                new Vector3(0.31f, 0.66f, 0.08f),
                new Vector3(0.1f, 0.3f, 0.1f),
                VisualTheme.ZombieBody,
                true).transform.localRotation = Quaternion.Euler(8f, 0f, -28f);

            _ = CreatePart(
                PrimitiveType.Sphere,
                "LeftHand",
                enemyRoot.transform,
                new Vector3(-0.37f, 0.46f, 0.14f),
                new Vector3(0.09f, 0.09f, 0.09f),
                VisualTheme.ZombieHead,
                true);

            _ = CreatePart(
                PrimitiveType.Sphere,
                "RightHand",
                enemyRoot.transform,
                new Vector3(0.37f, 0.46f, 0.14f),
                new Vector3(0.09f, 0.09f, 0.09f),
                VisualTheme.ZombieHead,
                true);

            var head = CreatePart(
                PrimitiveType.Sphere,
                "Head",
                enemyRoot.transform,
                new Vector3(0f, 1.03f, 0.01f),
                new Vector3(0.31f, 0.31f, 0.31f),
                VisualTheme.ZombieHead,
                true);
            head.transform.localRotation = Quaternion.Euler(12f, 0f, 0f);

            _ = CreatePart(
                PrimitiveType.Cube,
                "Jaw",
                enemyRoot.transform,
                new Vector3(0f, 0.88f, 0.15f),
                new Vector3(0.18f, 0.09f, 0.11f),
                VisualTheme.ZombieMouth,
                true);

            _ = CreatePart(
                PrimitiveType.Cube,
                "Hair",
                enemyRoot.transform,
                new Vector3(0f, 1.17f, 0f),
                new Vector3(0.25f, 0.08f, 0.2f),
                VisualTheme.ZombieHair,
                true);

            _ = CreatePart(
                PrimitiveType.Sphere,
                "EyeLeft",
                enemyRoot.transform,
                new Vector3(-0.08f, 1.04f, 0.22f),
                new Vector3(0.05f, 0.05f, 0.05f),
                VisualTheme.ZombieEye,
                true);

            _ = CreatePart(
                PrimitiveType.Sphere,
                "EyeRight",
                enemyRoot.transform,
                new Vector3(0.08f, 1.04f, 0.22f),
                new Vector3(0.05f, 0.05f, 0.05f),
                VisualTheme.ZombieEye,
                true);

            var agent = enemyRoot.AddComponent<EnemyAgent>();
            enemyRoot.transform.localRotation = Quaternion.Euler(0f, Random.Range(-7f, 7f), 0f);
            enemyRoot.SetActive(false);
            return agent;
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
                    Object.Destroy(collider);
                }
            }

            return part;
        }
    }
}
