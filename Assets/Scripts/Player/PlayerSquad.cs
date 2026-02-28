using System.Collections.Generic;
using InfiniStacker.Core;
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

        private TMP_Text _worldCountLabel;

        public int Count { get; private set; }
        public IReadOnlyList<Transform> ActiveMuzzles => _activeMuzzles;

        public void SetWorldCountLabel(TMP_Text worldCountLabel)
        {
            _worldCountLabel = worldCountLabel;
            UpdateWorldLabel();
        }

        public void ResetSquad()
        {
            SetCount(startCount);
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
            if (amount <= 0)
            {
                return;
            }

            SetCount(Count + amount);
        }

        public void RemoveSoldiers(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            SetCount(Count - amount);
        }

        private void Awake()
        {
            SetCount(startCount);
        }

        private void RefreshVisuals()
        {
            EnsurePoolSize(Count);
            var slots = SquadFormation.GetSlots(Count, formationSpacing);
            _activeMuzzles.Clear();

            for (var i = 0; i < _soldierPool.Count; i++)
            {
                var active = i < Count;
                _soldierPool[i].SetActive(active);

                if (!active)
                {
                    continue;
                }

                _soldierPool[i].transform.localPosition = slots[i];
                _activeMuzzles.Add(_soldierMuzzles[i]);
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
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = $"Soldier_{_soldierPool.Count}";
            body.transform.SetParent(transform, false);
            body.transform.localScale = new Vector3(0.45f, 0.9f, 0.45f);

            var bodyCollider = body.GetComponent<Collider>();
            if (bodyCollider != null)
            {
                Destroy(bodyCollider);
            }

            var bodyRenderer = body.GetComponent<Renderer>();
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = new Color(0.24f, 0.44f, 0.92f);
            }

            var helmet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            helmet.name = "Helmet";
            helmet.transform.SetParent(body.transform, false);
            helmet.transform.localScale = new Vector3(0.44f, 0.35f, 0.44f);
            helmet.transform.localPosition = new Vector3(0f, 0.58f, 0f);
            var helmetCollider = helmet.GetComponent<Collider>();
            if (helmetCollider != null)
            {
                Destroy(helmetCollider);
            }

            var helmetRenderer = helmet.GetComponent<Renderer>();
            if (helmetRenderer != null)
            {
                helmetRenderer.material.color = new Color(0.18f, 0.18f, 0.2f);
            }

            var rifle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rifle.name = "Rifle";
            rifle.transform.SetParent(body.transform, false);
            rifle.transform.localScale = new Vector3(0.08f, 0.08f, 0.6f);
            rifle.transform.localPosition = new Vector3(0f, 0.25f, 0.33f);
            var rifleCollider = rifle.GetComponent<Collider>();
            if (rifleCollider != null)
            {
                Destroy(rifleCollider);
            }

            var rifleRenderer = rifle.GetComponent<Renderer>();
            if (rifleRenderer != null)
            {
                rifleRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
            }

            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(body.transform, false);
            muzzle.localPosition = new Vector3(0f, 0.28f, 0.68f);

            _soldierPool.Add(body);
            _soldierMuzzles.Add(muzzle);
            body.SetActive(false);
        }

        private void UpdateWorldLabel()
        {
            if (_worldCountLabel != null)
            {
                _worldCountLabel.text = $"Squad: {Count}";
            }
        }
    }
}
