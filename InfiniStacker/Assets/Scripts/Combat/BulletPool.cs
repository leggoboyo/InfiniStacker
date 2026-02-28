using System.Collections.Generic;
using UnityEngine;

namespace InfiniStacker.Combat
{
    public sealed class BulletPool : MonoBehaviour
    {
        [SerializeField] private int prewarmCount = 180;

        private readonly Queue<Bullet> _pool = new();
        private HitVfxPool _hitVfxPool;

        public void Initialize(HitVfxPool hitVfxPool)
        {
            _hitVfxPool = hitVfxPool;
            Prewarm();
        }

        public void SpawnBullet(Vector3 worldPosition, Vector3 direction, int damage, float speed)
        {
            if (_pool.Count == 0)
            {
                _pool.Enqueue(CreateBullet());
            }

            var bullet = _pool.Dequeue();
            bullet.Launch(worldPosition, direction, damage, speed, 2.2f, this, _hitVfxPool);
        }

        public void ReturnToPool(Bullet bullet)
        {
            if (bullet == null)
            {
                return;
            }

            bullet.gameObject.SetActive(false);
            _pool.Enqueue(bullet);
        }

        private void Prewarm()
        {
            var needed = Mathf.Max(8, prewarmCount) - _pool.Count;
            for (var i = 0; i < needed; i++)
            {
                var bullet = CreateBullet();
                bullet.gameObject.SetActive(false);
                _pool.Enqueue(bullet);
            }
        }

        private Bullet CreateBullet()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Bullet";
            go.transform.SetParent(transform, false);
            go.transform.localScale = new Vector3(0.1f, 0.1f, 0.2f);

            var collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(1f, 0.84f, 0.35f);
            }

            return go.AddComponent<Bullet>();
        }
    }
}
