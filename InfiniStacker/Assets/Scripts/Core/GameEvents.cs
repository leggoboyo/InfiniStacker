using System;

namespace InfiniStacker.Core
{
    public static class GameEvents
    {
        public static event Action<int> SquadChanged;
        public static event Action<int, int> BaseHpChanged;
        public static event Action<float> TimerChanged;
        public static event Action<GameState, GameResult?> GameStateChanged;

        public static void RaiseSquadChanged(int squadCount)
        {
            SquadChanged?.Invoke(squadCount);
        }

        public static void RaiseBaseHpChanged(int currentHp, int maxHp)
        {
            BaseHpChanged?.Invoke(currentHp, maxHp);
        }

        public static void RaiseTimerChanged(float secondsRemaining)
        {
            TimerChanged?.Invoke(secondsRemaining);
        }

        public static void RaiseGameStateChanged(GameState state, GameResult? result)
        {
            GameStateChanged?.Invoke(state, result);
        }

        public static void ResetAll()
        {
            SquadChanged = null;
            BaseHpChanged = null;
            TimerChanged = null;
            GameStateChanged = null;
        }
    }
}
