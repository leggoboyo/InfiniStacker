using InfiniStacker.Enemy;
using InfiniStacker.Gates;
using InfiniStacker.Obstacles;
using InfiniStacker.Player;
using InfiniStacker.Upgrades;
using InfiniStacker.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InfiniStacker.DebugTools
{
    public sealed class DebugQuickTestController : MonoBehaviour
    {
        private PlayerSquad _playerSquad;
        private EnemySpawner _enemySpawner;
        private GateSpawner _gateSpawner;
        private ObstacleSpawner _obstacleSpawner;
        private UpgradeBlockSpawner _upgradeBlockSpawner;
        private BaseHealth _baseHealth;

        public void Initialize(
            PlayerSquad playerSquad,
            EnemySpawner enemySpawner,
            GateSpawner gateSpawner,
            ObstacleSpawner obstacleSpawner,
            UpgradeBlockSpawner upgradeBlockSpawner,
            BaseHealth baseHealth)
        {
            _playerSquad = playerSquad;
            _enemySpawner = enemySpawner;
            _gateSpawner = gateSpawner;
            _obstacleSpawner = obstacleSpawner;
            _upgradeBlockSpawner = upgradeBlockSpawner;
            _baseHealth = baseHealth;
        }

        private void Update()
        {
#if UNITY_EDITOR
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.digit1Key.wasPressedThisFrame)
            {
                _playerSquad?.AddSoldiers(10);
            }

            if (keyboard.digit2Key.wasPressedThisFrame)
            {
                _enemySpawner?.SpawnImmediateWave(12);
            }

            if (keyboard.digit3Key.wasPressedThisFrame)
            {
                _gateSpawner?.SpawnImmediatePair();
            }

            if (keyboard.digit4Key.wasPressedThisFrame)
            {
                _baseHealth?.ApplyDamage(100);
            }

            if (keyboard.digit5Key.wasPressedThisFrame)
            {
                _obstacleSpawner?.SpawnImmediateObstacle();
            }

            if (keyboard.digit6Key.wasPressedThisFrame)
            {
                _upgradeBlockSpawner?.SpawnImmediateBlock();
            }
#endif
        }
    }
}
