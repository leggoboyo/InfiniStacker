using System.Collections.Generic;
using InfiniStacker.Combat;
using InfiniStacker.Feedback;
using InfiniStacker.Visual;
using TMPro;
using UnityEngine;

namespace InfiniStacker.Upgrades
{
    public sealed class UpgradeBlockSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 3.2f;
        [SerializeField] private float moveSpeed = 3.9f;
        [SerializeField] private float spawnZ = 34f;
        [SerializeField] private float despawnZ = -7f;
        [SerializeField] private float laneCenterX = -2.2f;
        [SerializeField] private float laneHalfWidth = 1.1f;

        private readonly List<UpgradeBlock> _activeBlocks = new();
        private readonly Queue<UpgradeBlock> _pool = new();

        private WeaponUpgradeSystem _weaponUpgradeSystem;
        private HitVfxPool _hitVfxPool;
        private bool _isRunning;
        private float _timer;
        private int _spawnCounter;

        public void Initialize(WeaponUpgradeSystem weaponUpgradeSystem, HitVfxPool hitVfxPool)
        {
            _weaponUpgradeSystem = weaponUpgradeSystem;
            _hitVfxPool = hitVfxPool;
        }

        public void ConfigureLane(float centerX, float halfWidth)
        {
            laneCenterX = centerX;
            laneHalfWidth = Mathf.Clamp(halfWidth, 0.4f, 2.2f);
        }

        public void SetRunning(bool isRunning)
        {
            _isRunning = isRunning;
        }

        public void ResetSpawner()
        {
            _timer = 0f;
            _spawnCounter = 0;
            for (var i = _activeBlocks.Count - 1; i >= 0; i--)
            {
                Recycle(_activeBlocks[i]);
            }

            _activeBlocks.Clear();
        }

        public void SpawnImmediateBlock()
        {
            SpawnBlock();
        }

        public void NotifyDestroyed(UpgradeBlock block, int reward, Vector3 hitPoint)
        {
            if (!RemoveActive(block))
            {
                return;
            }

            _hitVfxPool?.Spawn(hitPoint);
            _weaponUpgradeSystem?.AddUpgradeProgress(reward);
            _weaponUpgradeSystem?.NotifyUpgradeBlockBroken();
            FeedbackServices.Haptics?.LightImpact();
            FeedbackServices.ScreenShake?.Shake(0.07f, 0.06f);
            Recycle(block);
        }

        private void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= spawnInterval)
            {
                _timer -= spawnInterval;
                SpawnBlock();
            }

            for (var i = _activeBlocks.Count - 1; i >= 0; i--)
            {
                var block = _activeBlocks[i];
                block.Tick(Time.deltaTime, moveSpeed);

                if (block.transform.position.z <= despawnZ)
                {
                    RecycleAndRemoveAt(i);
                }
            }
        }

        private void SpawnBlock()
        {
            var block = _pool.Count > 0 ? _pool.Dequeue() : CreateBlock();
            _spawnCounter++;
            var hp = Mathf.Min(42, 6 + (_spawnCounter / 2));
            var reward = Mathf.Min(18, 4 + (_spawnCounter / 3) + Random.Range(2, 6));
            var x = laneCenterX + Random.Range(-laneHalfWidth, laneHalfWidth);
            block.transform.position = new Vector3(x, 0.95f, spawnZ + Random.Range(0f, 2.6f));
            var label = block.GetComponentInChildren<TMP_Text>();
            block.Initialize(this, hp, reward, label);
            _activeBlocks.Add(block);
        }

        private UpgradeBlock CreateBlock()
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = "UpgradeBlock";
            block.transform.SetParent(transform, false);
            block.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);

            var renderer = block.GetComponent<Renderer>();
            if (renderer != null)
            {
                VisualTheme.ApplyMaterial(renderer, VisualTheme.Ice);
            }

            var emblem = GameObject.CreatePrimitive(PrimitiveType.Cube);
            emblem.name = "Emblem";
            emblem.transform.SetParent(block.transform, false);
            emblem.transform.localPosition = new Vector3(0f, 0.26f, 0.82f);
            emblem.transform.localScale = new Vector3(0.72f, 0.72f, 0.1f);
            var emblemRenderer = emblem.GetComponent<Renderer>();
            if (emblemRenderer != null)
            {
                VisualTheme.ApplyMaterial(emblemRenderer, VisualTheme.GateAdd);
            }

            var emblemCollider = emblem.GetComponent<Collider>();
            if (emblemCollider != null)
            {
                Destroy(emblemCollider);
            }

            var labelGo = new GameObject("UpgradeLabel");
            labelGo.transform.SetParent(block.transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 0.7f, 0.84f);
            labelGo.transform.localScale = Vector3.one * 0.18f;

            var label = labelGo.AddComponent<TextMeshPro>();
            label.fontSize = 9f;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.outlineColor = new Color(0f, 0f, 0f, 0.8f);
            label.outlineWidth = 0.2f;

            var upgradeBlock = block.AddComponent<UpgradeBlock>();
            block.SetActive(false);
            return upgradeBlock;
        }

        private void RecycleAndRemoveAt(int index)
        {
            var block = _activeBlocks[index];
            var last = _activeBlocks.Count - 1;
            _activeBlocks[index] = _activeBlocks[last];
            _activeBlocks.RemoveAt(last);
            Recycle(block);
        }

        private bool RemoveActive(UpgradeBlock block)
        {
            var index = _activeBlocks.IndexOf(block);
            if (index < 0)
            {
                return false;
            }

            var last = _activeBlocks.Count - 1;
            _activeBlocks[index] = _activeBlocks[last];
            _activeBlocks.RemoveAt(last);
            return true;
        }

        private void Recycle(UpgradeBlock block)
        {
            block.MarkInactive();
            _pool.Enqueue(block);
        }
    }
}
