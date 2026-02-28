using InfiniStacker.Combat;
using TMPro;
using UnityEngine;

namespace InfiniStacker.Obstacles
{
    public sealed class IceObstacle : MonoBehaviour, IBulletHittable
    {
        private ObstacleSpawner _owner;
        private TMP_Text _hpLabel;
        private int _hp;
        private bool _isAlive;

        public void Initialize(ObstacleSpawner owner, int hp, TMP_Text hpLabel)
        {
            _owner = owner;
            _hpLabel = hpLabel;
            _hp = Mathf.Max(1, hp);
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
                _owner.NotifyDestroyed(this, hitPoint);
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
            if (_hpLabel != null)
            {
                _hpLabel.text = _hp.ToString();
            }
        }
    }
}
