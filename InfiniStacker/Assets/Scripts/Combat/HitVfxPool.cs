using System.Collections.Generic;
using UnityEngine;

namespace InfiniStacker.Combat
{
    public sealed class HitVfxPool : MonoBehaviour
    {
        [SerializeField] private int prewarmCount = 36;

        private readonly Queue<HitVfx> _pool = new();

        public void Initialize()
        {
            var needed = Mathf.Max(8, prewarmCount) - _pool.Count;
            for (var i = 0; i < needed; i++)
            {
                var vfx = CreateVfx();
                vfx.gameObject.SetActive(false);
                _pool.Enqueue(vfx);
            }
        }

        public void Spawn(Vector3 worldPosition)
        {
            if (_pool.Count == 0)
            {
                _pool.Enqueue(CreateVfx());
            }

            var vfx = _pool.Dequeue();
            vfx.Play(worldPosition, this);
        }

        public void ReturnToPool(HitVfx vfx)
        {
            if (vfx == null)
            {
                return;
            }

            vfx.gameObject.SetActive(false);
            _pool.Enqueue(vfx);
        }

        private HitVfx CreateVfx()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "HitVfx";
            go.transform.SetParent(transform, false);
            go.transform.localScale = Vector3.one * 0.35f;

            var collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(1f, 0.5f, 0.2f);
            }

            return go.AddComponent<HitVfx>();
        }
    }
}
