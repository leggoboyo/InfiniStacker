using UnityEngine;

namespace InfiniStacker.Combat
{
    public sealed class Bullet : MonoBehaviour
    {
        private static readonly Collider[] HitBuffer = new Collider[12];

        private Vector3 _direction;
        private float _speed;
        private int _damage;
        private float _remainingLife;
        private bool _isActive;
        private BulletPool _pool;
        private HitVfxPool _hitVfxPool;

        public void Launch(
            Vector3 worldPosition,
            Vector3 direction,
            int damage,
            float speed,
            float lifetime,
            BulletPool pool,
            HitVfxPool hitVfxPool)
        {
            transform.position = worldPosition;
            _direction = direction.normalized;
            _damage = Mathf.Max(1, damage);
            _speed = Mathf.Max(1f, speed);
            _remainingLife = Mathf.Max(0.05f, lifetime);
            _pool = pool;
            _hitVfxPool = hitVfxPool;
            _isActive = true;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            var nextPosition = transform.position + (_direction * (_speed * Time.deltaTime));
            var hitCount = Physics.OverlapSphereNonAlloc(nextPosition, 0.28f, HitBuffer, ~0, QueryTriggerInteraction.Ignore);

            for (var i = 0; i < hitCount; i++)
            {
                var collider = HitBuffer[i];
                if (collider == null)
                {
                    continue;
                }

                if (collider.TryGetComponent<IBulletHittable>(out var hittable) && hittable.OnBulletHit(_damage, nextPosition))
                {
                    _hitVfxPool?.Spawn(nextPosition);
                    Despawn();
                    return;
                }
            }

            transform.position = nextPosition;
            _remainingLife -= Time.deltaTime;
            if (_remainingLife <= 0f)
            {
                Despawn();
            }
        }

        private void Despawn()
        {
            if (!_isActive)
            {
                return;
            }

            _isActive = false;
            _pool.ReturnToPool(this);
        }
    }
}
