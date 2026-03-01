using InfiniStacker.Combat;
using TMPro;
using UnityEngine;

namespace InfiniStacker.Upgrades
{
    public sealed class UpgradeBlock : MonoBehaviour, IBulletHittable
    {
        private UpgradeBlockSpawner _owner;
        private TMP_Text _label;
        private int _hp;
        private int _reward;
        private bool _isAlive;

        public void Initialize(UpgradeBlockSpawner owner, int hp, int reward, TMP_Text label)
        {
            _owner = owner;
            _label = label;
            _hp = Mathf.Max(1, hp);
            _reward = Mathf.Max(1, reward);
            _isAlive = true;
            UpdateLabel();
            gameObject.SetActive(true);
        }

        public void Tick(float deltaTime, float speed)
        {
            transform.position += Vector3.back * (speed * deltaTime);
        }

        public bool OnBulletHit(int damage, Vector3 hitPoint)
        {
            if (!_isAlive)
            {
                return false;
            }

            _hp -= Mathf.Max(1, damage);
            if (_hp <= 0)
            {
                _isAlive = false;
                _owner?.NotifyDestroyed(this, _reward, hitPoint);
            }
            else
            {
                UpdateLabel();
            }

            return true;
        }

        public void MarkInactive()
        {
            _isAlive = false;
            gameObject.SetActive(false);
        }

        private void UpdateLabel()
        {
            if (_label != null)
            {
                _label.text = $"UP\n+{_reward}";
            }
        }
    }
}
