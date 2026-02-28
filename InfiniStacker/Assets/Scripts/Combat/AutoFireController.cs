using InfiniStacker.Player;
using UnityEngine;

namespace InfiniStacker.Combat
{
    public sealed class AutoFireController : MonoBehaviour
    {
        [SerializeField] private float shotsPerSecond = 8f;
        [SerializeField] private float bulletSpeed = 32f;
        [SerializeField] private int bulletDamage = 1;

        private PlayerSquad _playerSquad;
        private BulletPool _bulletPool;
        private bool _fireEnabled;
        private float _fireAccumulator;

        public void Initialize(PlayerSquad playerSquad, BulletPool bulletPool)
        {
            _playerSquad = playerSquad;
            _bulletPool = bulletPool;
        }

        public void SetEnabled(bool enabled)
        {
            _fireEnabled = enabled;
            if (!enabled)
            {
                _fireAccumulator = 0f;
            }
        }

        private void Update()
        {
            if (!_fireEnabled || _playerSquad == null || _bulletPool == null || _playerSquad.Count <= 0)
            {
                return;
            }

            _fireAccumulator += Time.deltaTime;
            var fireStep = 1f / Mathf.Max(1f, shotsPerSecond);

            while (_fireAccumulator >= fireStep)
            {
                _fireAccumulator -= fireStep;
                FireVolley();
            }
        }

        private void FireVolley()
        {
            var muzzles = _playerSquad.ActiveMuzzles;
            for (var i = 0; i < muzzles.Count; i++)
            {
                _bulletPool.SpawnBullet(muzzles[i].position, Vector3.forward, bulletDamage, bulletSpeed);
            }
        }
    }
}
