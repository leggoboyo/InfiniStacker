using InfiniStacker.Combat;
using UnityEngine;

namespace InfiniStacker.Enemy
{
    public sealed class EnemyAgent : MonoBehaviour, IBulletHittable
    {
        private EnemyManager _manager;
        private int _hp;
        private float _speed;
        private bool _isAlive;

        public void Initialize(EnemyManager manager, int hp, float speed)
        {
            _manager = manager;
            _hp = Mathf.Max(1, hp);
            _speed = Mathf.Max(0.5f, speed);
            _isAlive = true;
        }

        public void Tick(float deltaTime, float breachZ)
        {
            if (!_isAlive)
            {
                return;
            }

            transform.position += Vector3.back * (_speed * deltaTime);
            if (transform.position.z <= breachZ)
            {
                _manager.NotifyBreach(this);
            }
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
                _manager.NotifyKilled(this);
            }

            return true;
        }

        public void MarkInactive()
        {
            _isAlive = false;
            gameObject.SetActive(false);
        }
    }
}
