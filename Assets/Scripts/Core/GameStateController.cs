using InfiniStacker.Combat;
using InfiniStacker.Enemy;
using InfiniStacker.Feedback;
using InfiniStacker.Gates;
using InfiniStacker.Obstacles;
using InfiniStacker.Player;
using InfiniStacker.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InfiniStacker.Core
{
    public sealed class GameStateController : MonoBehaviour
    {
        [SerializeField] private float survivalDurationSeconds = 60f;

        private readonly SurvivalTimer _timer = new();

        private PlayerSquad _playerSquad;
        private BaseHealth _baseHealth;
        private EnemySpawner _enemySpawner;
        private GateSpawner _gateSpawner;
        private ObstacleSpawner _obstacleSpawner;
        private EnemyManager _enemyManager;
        private AutoFireController _autoFireController;
        private PlayerDragMover _playerDragMover;

        public GameState CurrentState { get; private set; } = GameState.Start;

        public void Initialize(
            PlayerSquad playerSquad,
            BaseHealth baseHealth,
            EnemySpawner enemySpawner,
            GateSpawner gateSpawner,
            ObstacleSpawner obstacleSpawner,
            EnemyManager enemyManager,
            AutoFireController autoFireController,
            PlayerDragMover playerDragMover)
        {
            _playerSquad = playerSquad;
            _baseHealth = baseHealth;
            _enemySpawner = enemySpawner;
            _gateSpawner = gateSpawner;
            _obstacleSpawner = obstacleSpawner;
            _enemyManager = enemyManager;
            _autoFireController = autoFireController;
            _playerDragMover = playerDragMover;

            _timer.Configure(survivalDurationSeconds);
            EnterStartState();
        }

        private void OnEnable()
        {
            GameEvents.SquadChanged += OnSquadChanged;
            GameEvents.BaseHpChanged += OnBaseHpChanged;
        }

        private void OnDisable()
        {
            GameEvents.SquadChanged -= OnSquadChanged;
            GameEvents.BaseHpChanged -= OnBaseHpChanged;
        }

        private void Update()
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            if (_timer.Tick(Time.deltaTime))
            {
                EndGame(GameResult.Victory);
                return;
            }

            GameEvents.RaiseTimerChanged(_timer.RemainingSeconds);
        }

        public void StartGame()
        {
            if (_playerSquad == null || _baseHealth == null || _enemySpawner == null || _gateSpawner == null || _obstacleSpawner == null || _enemyManager == null || _autoFireController == null || _playerDragMover == null)
            {
                Debug.LogError("GameStateController dependencies were not initialized.");
                return;
            }

            _playerSquad.ResetSquad();
            _baseHealth.ResetHealth();
            _enemyManager.ResetAll();
            _enemyManager.SetRunning(true);
            _enemySpawner.ResetSpawner();
            _enemySpawner.SetRunning(true);
            _gateSpawner.ResetSpawner();
            _gateSpawner.SetRunning(true);
            _obstacleSpawner.ResetSpawner();
            _obstacleSpawner.SetRunning(true);
            _autoFireController.SetEnabled(true);
            _playerDragMover.SetEnabled(true);

            _timer.Start();
            CurrentState = GameState.Playing;
            GameEvents.RaiseTimerChanged(_timer.RemainingSeconds);
            GameEvents.RaiseGameStateChanged(CurrentState, null);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void EnterStartState()
        {
            CurrentState = GameState.Start;
            _timer.Stop();
            _enemySpawner?.SetRunning(false);
            _gateSpawner?.SetRunning(false);
            _obstacleSpawner?.SetRunning(false);
            _enemyManager?.SetRunning(false);
            _autoFireController?.SetEnabled(false);
            _playerDragMover?.SetEnabled(false);

            if (_baseHealth != null)
            {
                _baseHealth.ResetHealth();
            }

            if (_playerSquad != null)
            {
                _playerSquad.ResetSquad();
            }

            GameEvents.RaiseTimerChanged(survivalDurationSeconds);
            GameEvents.RaiseGameStateChanged(CurrentState, null);
        }

        private void EndGame(GameResult result)
        {
            _timer.Stop();
            _enemySpawner?.SetRunning(false);
            _gateSpawner?.SetRunning(false);
            _obstacleSpawner?.SetRunning(false);
            _enemyManager?.SetRunning(false);
            _autoFireController?.SetEnabled(false);
            _playerDragMover?.SetEnabled(false);

            if (result == GameResult.Victory)
            {
                FeedbackServices.Haptics?.Success();
            }

            CurrentState = result == GameResult.Victory ? GameState.Victory : GameState.GameOver;
            GameEvents.RaiseGameStateChanged(CurrentState, result);
        }

        private void OnSquadChanged(int squadCount)
        {
            if (CurrentState == GameState.Playing && squadCount <= 0)
            {
                EndGame(GameResult.DefeatBySquad);
            }
        }

        private void OnBaseHpChanged(int currentHp, int maxHp)
        {
            if (CurrentState == GameState.Playing && currentHp <= 0)
            {
                EndGame(GameResult.DefeatByBase);
            }
        }
    }
}
