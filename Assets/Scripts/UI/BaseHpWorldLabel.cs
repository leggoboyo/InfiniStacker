using InfiniStacker.Core;
using TMPro;
using UnityEngine;

namespace InfiniStacker.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class BaseHpWorldLabel : MonoBehaviour
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            GameEvents.BaseHpChanged += OnBaseHpChanged;
        }

        private void OnDisable()
        {
            GameEvents.BaseHpChanged -= OnBaseHpChanged;
        }

        private void OnBaseHpChanged(int currentHp, int maxHp)
        {
            if (_text == null)
            {
                return;
            }

            _text.text = currentHp.ToString();
        }
    }
}
