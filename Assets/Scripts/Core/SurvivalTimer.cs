using UnityEngine;

namespace InfiniStacker.Core
{
    public sealed class SurvivalTimer
    {
        private float _duration;

        public float RemainingSeconds { get; private set; }
        public bool IsRunning { get; private set; }

        public void Configure(float durationSeconds)
        {
            _duration = Mathf.Max(1f, durationSeconds);
            RemainingSeconds = _duration;
        }

        public void Start()
        {
            RemainingSeconds = _duration;
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public bool Tick(float deltaTime)
        {
            if (!IsRunning)
            {
                return false;
            }

            RemainingSeconds = Mathf.Max(0f, RemainingSeconds - deltaTime);
            return RemainingSeconds <= 0f;
        }
    }
}
