using System;
using InfiniStacker.Core;
using UnityEngine;

namespace InfiniStacker.Upgrades
{
    public sealed class WeaponUpgradeSystem : MonoBehaviour
    {
        [SerializeField] private int maxTurretCharges = 3;

        private WeaponTierDefinition[] _tiers;
        private int _tierIndex;
        private int _progressInTier;
        private int _turretCharges;
        private int _brokenBlocksCount;

        public WeaponStats CurrentStats { get; private set; }
        public int TurretCharges => _turretCharges;

        private void Awake()
        {
            EnsureDefaultTiers();
            ResetProgression();
        }

        public void ResetProgression()
        {
            _tierIndex = 0;
            _progressInTier = 0;
            _turretCharges = 0;
            _brokenBlocksCount = 0;
            ApplyCurrentTierStats();
            RaiseStateChanged();
        }

        public void AddUpgradeProgress(int amount)
        {
            if (amount <= 0 || _tiers == null || _tiers.Length == 0)
            {
                return;
            }

            _progressInTier += amount;
            while (_tierIndex < _tiers.Length - 1 && _progressInTier >= _tiers[_tierIndex].PointsToNext)
            {
                _progressInTier -= _tiers[_tierIndex].PointsToNext;
                _tierIndex++;
                ApplyCurrentTierStats();
            }

            RaiseStateChanged();
        }

        public void NotifyUpgradeBlockBroken()
        {
            _brokenBlocksCount++;
            if (_tierIndex < 2)
            {
                return;
            }

            if (_brokenBlocksCount % 2 == 0)
            {
                AddTurretCharge(1);
            }
        }

        public bool TryConsumeTurretCharge()
        {
            if (_turretCharges <= 0)
            {
                return false;
            }

            _turretCharges--;
            RaiseStateChanged();
            return true;
        }

        private void AddTurretCharge(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            var previous = _turretCharges;
            _turretCharges = Mathf.Clamp(_turretCharges + amount, 0, Mathf.Max(1, maxTurretCharges));
            if (_turretCharges != previous)
            {
                RaiseStateChanged();
            }
        }

        private void ApplyCurrentTierStats()
        {
            var tier = _tiers[Mathf.Clamp(_tierIndex, 0, _tiers.Length - 1)];
            CurrentStats = new WeaponStats(
                tier.Name,
                _tierIndex,
                tier.ShotsPerSecond,
                tier.BulletDamage,
                tier.PelletsPerShot,
                tier.SpreadAngleDegrees);
        }

        private void RaiseStateChanged()
        {
            var requirement = _tierIndex < _tiers.Length - 1 ? _tiers[_tierIndex].PointsToNext : 0;
            GameEvents.RaiseWeaponStateChanged(
                CurrentStats.TierName,
                CurrentStats.TierIndex,
                _progressInTier,
                requirement,
                _turretCharges);
        }

        private void EnsureDefaultTiers()
        {
            if (_tiers != null && _tiers.Length > 0)
            {
                return;
            }

            _tiers = new[]
            {
                new WeaponTierDefinition("Rifle I", 3.2f, 1, 1, 0f, 18),
                new WeaponTierDefinition("Rifle II", 4.3f, 1, 1, 0f, 26),
                new WeaponTierDefinition("Rifle III", 5.5f, 2, 1, 0f, 36),
                new WeaponTierDefinition("Shotgun I", 4.5f, 1, 3, 8f, 48),
                new WeaponTierDefinition("Shotgun II", 5.2f, 2, 5, 11f, 0),
            };
        }

        [Serializable]
        private readonly struct WeaponTierDefinition
        {
            public readonly string Name;
            public readonly float ShotsPerSecond;
            public readonly int BulletDamage;
            public readonly int PelletsPerShot;
            public readonly float SpreadAngleDegrees;
            public readonly int PointsToNext;

            public WeaponTierDefinition(
                string name,
                float shotsPerSecond,
                int bulletDamage,
                int pelletsPerShot,
                float spreadAngleDegrees,
                int pointsToNext)
            {
                Name = name;
                ShotsPerSecond = Mathf.Max(0.5f, shotsPerSecond);
                BulletDamage = Mathf.Max(1, bulletDamage);
                PelletsPerShot = Mathf.Max(1, pelletsPerShot);
                SpreadAngleDegrees = Mathf.Max(0f, spreadAngleDegrees);
                PointsToNext = Mathf.Max(0, pointsToNext);
            }
        }
    }

    public readonly struct WeaponStats
    {
        public readonly string TierName;
        public readonly int TierIndex;
        public readonly float ShotsPerSecond;
        public readonly int BulletDamage;
        public readonly int PelletsPerShot;
        public readonly float SpreadAngleDegrees;

        public WeaponStats(
            string tierName,
            int tierIndex,
            float shotsPerSecond,
            int bulletDamage,
            int pelletsPerShot,
            float spreadAngleDegrees)
        {
            TierName = tierName ?? "Rifle I";
            TierIndex = Mathf.Max(0, tierIndex);
            ShotsPerSecond = Mathf.Max(0.5f, shotsPerSecond);
            BulletDamage = Mathf.Max(1, bulletDamage);
            PelletsPerShot = Mathf.Max(1, pelletsPerShot);
            SpreadAngleDegrees = Mathf.Max(0f, spreadAngleDegrees);
        }
    }
}
