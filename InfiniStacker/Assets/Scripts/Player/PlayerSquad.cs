using System.Collections.Generic;
using InfiniStacker.Core;
using InfiniStacker.Gates;
using InfiniStacker.Visual;
using TMPro;
using UnityEngine;

namespace InfiniStacker.Player
{
    public sealed class PlayerSquad : MonoBehaviour
    {
        [SerializeField] private int startCount = 1;
        [SerializeField] private int maxCount = 120;
        [SerializeField] private float formationSpacing = 0.8f;

        private readonly List<GameObject> _soldierPool = new();
        private readonly List<Transform> _soldierMuzzles = new();
        private readonly List<Transform> _activeMuzzles = new();
        private readonly List<Vector3> _slotPositions = new();

        private TMP_Text _worldCountLabel;

        public int Count { get; private set; }
        public int Power => Count;
        public IReadOnlyList<Transform> ActiveMuzzles => _activeMuzzles;

        public void SetWorldCountLabel(TMP_Text worldCountLabel)
        {
            _worldCountLabel = worldCountLabel;
            UpdateWorldLabel();
        }

        public void ResetSquad()
        {
            ForceCount(startCount);
        }

        public void SetCount(int value)
        {
            var next = Mathf.Clamp(value, 0, maxCount);
            if (next == Count)
            {
                return;
            }

            Count = next;
            RefreshVisuals();
            UpdateWorldLabel();
            GameEvents.RaiseSquadChanged(Count);
        }

        public void AddSoldiers(int amount)
        {
            if (amount > 0)
            {
                SetCount(Count + amount);
            }
        }

        public void RemoveSoldiers(int amount)
        {
            if (amount > 0)
            {
                SetCount(Count - amount);
            }
        }

        public void ApplyGateOperation(GateOperation operation)
        {
            SetCount(GateMath.Apply(Count, operation));
        }

        private void Awake()
        {
            ForceCount(startCount);
        }

        private void ForceCount(int value)
        {
            Count = Mathf.Clamp(value, 0, maxCount);
            RefreshVisuals();
            UpdateWorldLabel();
            GameEvents.RaiseSquadChanged(Count);
        }

        private void RefreshVisuals()
        {
            EnsurePoolSize(Count);
            var slots = SquadFormation.GetSlots(Count, formationSpacing);
            _activeMuzzles.Clear();
            _slotPositions.Clear();

            for (var i = 0; i < _soldierPool.Count; i++)
            {
                var active = i < Count;
                _soldierPool[i].SetActive(active);

                if (!active)
                {
                    continue;
                }

                _soldierPool[i].transform.localPosition = slots[i];
                _slotPositions.Add(slots[i]);
                _activeMuzzles.Add(_soldierMuzzles[i]);
            }
        }

        private void LateUpdate()
        {
            if (Count <= 0)
            {
                return;
            }

            var t = Time.time;
            for (var i = 0; i < Count; i++)
            {
                var soldier = _soldierPool[i].transform;
                var slot = _slotPositions[i];
                slot.y += Mathf.Sin((t * 8.4f) + (i * 0.47f)) * 0.03f;
                soldier.localPosition = slot;
                soldier.localRotation = Quaternion.Euler(0f, Mathf.Sin((t * 2.9f) + (i * 0.31f)) * 3.2f, 0f);
            }
        }

        private void EnsurePoolSize(int needed)
        {
            while (_soldierPool.Count < needed)
            {
                CreateSoldierVisual();
            }
        }

        private void CreateSoldierVisual()
        {
            var soldier = new GameObject($"Soldier_{_soldierPool.Count}");
            soldier.transform.SetParent(transform, false);

            _ = CreatePart(
                PrimitiveType.Capsule,
                "Torso",
                soldier.transform,
                new Vector3(0f, 0.56f, 0f),
                new Vector3(0.42f, 0.72f, 0.42f),
                VisualTheme.SoldierSuit);

            _ = CreatePart(
                PrimitiveType.Cube,
                "Vest",
                soldier.transform,
                new Vector3(0f, 0.56f, 0.02f),
                new Vector3(0.38f, 0.34f, 0.34f),
                VisualTheme.SoldierArmor);

            _ = CreatePart(
                PrimitiveType.Cube,
                "Backpack",
                soldier.transform,
                new Vector3(0f, 0.55f, -0.2f),
                new Vector3(0.25f, 0.32f, 0.12f),
                VisualTheme.SoldierArmor);

            _ = CreatePart(
                PrimitiveType.Capsule,
                "LeftLeg",
                soldier.transform,
                new Vector3(-0.12f, 0.2f, 0f),
                new Vector3(0.17f, 0.33f, 0.17f),
                VisualTheme.SoldierSuit);

            _ = CreatePart(
                PrimitiveType.Capsule,
                "RightLeg",
                soldier.transform,
                new Vector3(0.12f, 0.2f, 0f),
                new Vector3(0.17f, 0.33f, 0.17f),
                VisualTheme.SoldierSuit);

            _ = CreatePart(
                PrimitiveType.Capsule,
                "LeftArm",
                soldier.transform,
                new Vector3(-0.26f, 0.62f, 0.05f),
                new Vector3(0.11f, 0.28f, 0.11f),
                VisualTheme.SoldierSuit).transform.localRotation = Quaternion.Euler(0f, 0f, 18f);

            _ = CreatePart(
                PrimitiveType.Capsule,
                "RightArm",
                soldier.transform,
                new Vector3(0.26f, 0.62f, 0.05f),
                new Vector3(0.11f, 0.28f, 0.11f),
                VisualTheme.SoldierSuit).transform.localRotation = Quaternion.Euler(0f, 0f, -18f);

            _ = CreatePart(
                PrimitiveType.Sphere,
                "Head",
                soldier.transform,
                new Vector3(0f, 1.03f, 0f),
                new Vector3(0.22f, 0.24f, 0.22f),
                VisualTheme.SoldierSuit);

            _ = CreatePart(
                PrimitiveType.Cube,
                "Helmet",
                soldier.transform,
                new Vector3(0f, 1.1f, 0f),
                new Vector3(0.34f, 0.17f, 0.34f),
                VisualTheme.SoldierHelmet);

            _ = CreatePart(
                PrimitiveType.Cube,
                "Visor",
                soldier.transform,
                new Vector3(0f, 1.03f, 0.12f),
                new Vector3(0.26f, 0.1f, 0.06f),
                VisualTheme.SoldierVisor);

            _ = CreatePart(
                PrimitiveType.Cube,
                "Rifle",
                soldier.transform,
                new Vector3(0f, 0.58f, 0.34f),
                new Vector3(0.07f, 0.07f, 0.54f),
                VisualTheme.Weapon);

            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(soldier.transform, false);
            muzzle.localPosition = new Vector3(0f, 0.6f, 0.64f);

            soldier.SetActive(false);
            _soldierPool.Add(soldier);
            _soldierMuzzles.Add(muzzle);
        }

        private static GameObject CreatePart(
            PrimitiveType primitiveType,
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Material material)
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

            var collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                Object.Destroy(collider);
            }

            return go;
        }

        private void UpdateWorldLabel()
        {
            if (_worldCountLabel != null)
            {
                _worldCountLabel.text = Count.ToString();
            }
        }
    }
}
