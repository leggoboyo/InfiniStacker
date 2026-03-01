using InfiniStacker.Player;
using InfiniStacker.Enemy;
using InfiniStacker.Upgrades;
using UnityEngine;

namespace InfiniStacker.Combat
{
    public sealed class AutoFireController : MonoBehaviour
    {
        [SerializeField] private float baseShotsPerSecond = 3.2f;
        [SerializeField] private float bulletSpeed = 32f;
        [SerializeField] private int baseBulletDamage = 1;
        [SerializeField] private int maxShotsPerVolley = 16;
        [SerializeField] private int maxProjectilesPerVolley = 36;
        [SerializeField] private int overrunEnemyThreshold = 34;
        [SerializeField] private float turretDurationSeconds = 8f;
        [SerializeField] private float turretShotsPerSecond = 11f;
        [SerializeField] private float turretBulletSpeed = 35f;
        [SerializeField] private int turretBulletDamage = 2;

        private PlayerSquad _playerSquad;
        private BulletPool _bulletPool;
        private WeaponUpgradeSystem _weaponUpgradeSystem;
        private EnemyManager _enemyManager;
        private bool _fireEnabled;
        private float _fireAccumulator;
        private float _turretFireAccumulator;
        private float _turretTimer;

        public void Initialize(
            PlayerSquad playerSquad,
            BulletPool bulletPool,
            WeaponUpgradeSystem weaponUpgradeSystem = null,
            EnemyManager enemyManager = null)
        {
            _playerSquad = playerSquad;
            _bulletPool = bulletPool;
            _weaponUpgradeSystem = weaponUpgradeSystem;
            _enemyManager = enemyManager;
        }

        public void SetEnabled(bool enabled)
        {
            _fireEnabled = enabled;
            if (!enabled)
            {
                _fireAccumulator = 0f;
                _turretFireAccumulator = 0f;
                _turretTimer = 0f;
            }
        }

        private void Update()
        {
            if (!_fireEnabled || _playerSquad == null || _bulletPool == null || _playerSquad.Count <= 0)
            {
                return;
            }

            var weaponStats = _weaponUpgradeSystem != null
                ? _weaponUpgradeSystem.CurrentStats
                : new WeaponStats("Rifle I", 0, baseShotsPerSecond, baseBulletDamage, 1, 0f);

            TryAutoDeployTurret();
            _fireAccumulator += Time.deltaTime;
            var crowdRateBoost = Mathf.Lerp(1f, 1.65f, Mathf.InverseLerp(1f, 80f, _playerSquad.Count));
            var fireStep = 1f / Mathf.Max(0.5f, weaponStats.ShotsPerSecond * crowdRateBoost);

            while (_fireAccumulator >= fireStep)
            {
                _fireAccumulator -= fireStep;
                FireVolley(weaponStats);
            }

            TickTurretFire();
        }

        private void FireVolley(WeaponStats weaponStats)
        {
            var muzzles = _playerSquad.ActiveMuzzles;
            if (muzzles.Count == 0)
            {
                return;
            }

            var pelletsPerShot = Mathf.Max(1, weaponStats.PelletsPerShot);
            var shotCount = Mathf.Clamp(muzzles.Count, 1, Mathf.Max(1, maxShotsPerVolley));
            var maxShotsByProjectileBudget = Mathf.Max(1, maxProjectilesPerVolley / pelletsPerShot);
            shotCount = Mathf.Min(shotCount, maxShotsByProjectileBudget);
            var compressionFactor = Mathf.Max(1, Mathf.CeilToInt(muzzles.Count / (float)shotCount));
            var baseDamage = Mathf.Max(1, weaponStats.BulletDamage * compressionFactor);

            for (var shotIndex = 0; shotIndex < shotCount; shotIndex++)
            {
                var muzzleIndex = Mathf.FloorToInt((shotIndex / (float)shotCount) * muzzles.Count);
                if (muzzleIndex >= muzzles.Count)
                {
                    muzzleIndex = muzzles.Count - 1;
                }

                var muzzlePosition = muzzles[muzzleIndex].position;
                if (pelletsPerShot <= 1)
                {
                    _bulletPool.SpawnBullet(muzzlePosition, Vector3.forward, baseDamage, bulletSpeed);
                    continue;
                }

                var pelletDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage / Mathf.Sqrt(pelletsPerShot)));
                var mid = (pelletsPerShot - 1) * 0.5f;
                for (var pellet = 0; pellet < pelletsPerShot; pellet++)
                {
                    var offset = pellet - mid;
                    var normalizedOffset = mid <= 0.001f ? 0f : offset / mid;
                    var spread = normalizedOffset * weaponStats.SpreadAngleDegrees;
                    var dir = Quaternion.Euler(0f, spread, 0f) * Vector3.forward;
                    _bulletPool.SpawnBullet(muzzlePosition, dir, pelletDamage, bulletSpeed);
                }
            }
        }

        private void TryAutoDeployTurret()
        {
            if (_turretTimer > 0f || _enemyManager == null || _weaponUpgradeSystem == null)
            {
                return;
            }

            if (_enemyManager.ActiveEnemyCount < Mathf.Max(10, overrunEnemyThreshold))
            {
                return;
            }

            if (_weaponUpgradeSystem.TryConsumeTurretCharge())
            {
                _turretTimer = turretDurationSeconds;
                _turretFireAccumulator = 0f;
            }
        }

        private void TickTurretFire()
        {
            if (_turretTimer <= 0f)
            {
                return;
            }

            _turretTimer -= Time.deltaTime;
            _turretFireAccumulator += Time.deltaTime;

            var fireStep = 1f / Mathf.Max(1f, turretShotsPerSecond);
            while (_turretFireAccumulator >= fireStep)
            {
                _turretFireAccumulator -= fireStep;
                SpawnTurretVolley();
            }
        }

        private void SpawnTurretVolley()
        {
            if (_playerSquad == null)
            {
                return;
            }

            var turretZ = _playerSquad.transform.position.z + 0.4f;
            var leftTurretPos = new Vector3(1.2f, 1.12f, turretZ);
            var rightTurretPos = new Vector3(3.15f, 1.12f, turretZ);
            _bulletPool.SpawnBullet(leftTurretPos, Vector3.forward, turretBulletDamage, turretBulletSpeed);
            _bulletPool.SpawnBullet(rightTurretPos, Vector3.forward, turretBulletDamage, turretBulletSpeed);
        }
    }
}
