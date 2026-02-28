using InfiniStacker.Core;
using UnityEngine;

namespace InfiniStacker.World
{
    public sealed class BaseHealth : MonoBehaviour
    {
        [SerializeField] private int maxHp = 450;

        public int MaxHp => maxHp;
        public int CurrentHp { get; private set; }

        private void Awake()
        {
            CurrentHp = maxHp;
            GameEvents.RaiseBaseHpChanged(CurrentHp, maxHp);
        }

        public void ResetHealth()
        {
            CurrentHp = maxHp;
            GameEvents.RaiseBaseHpChanged(CurrentHp, maxHp);
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0 || CurrentHp <= 0)
            {
                return;
            }

            CurrentHp = Mathf.Max(0, CurrentHp - amount);
            GameEvents.RaiseBaseHpChanged(CurrentHp, maxHp);
        }
    }
}
