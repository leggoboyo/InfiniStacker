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
        private float _baseX;
        private float _baseY;
        private float _animPhase;
        private float _weaveAmplitude;
        private float _wobbleAmplitude;

        public void Initialize(EnemyManager manager, int hp, float speed)
        {
            _manager = manager;
            _hp = Mathf.Max(1, hp);
            _speed = Mathf.Max(0.5f, speed);
            _isAlive = true;
            _baseX = transform.position.x;
            _baseY = transform.position.y;
            _animPhase = Random.Range(0f, 6.28f);
            _weaveAmplitude = Random.Range(0.01f, 0.045f);
            _wobbleAmplitude = Random.Range(0.02f, 0.05f);
        }

        public void Tick(float deltaTime, float breachZ)
        {
            if (!_isAlive)
            {
                return;
            }

            var t = Time.time + _animPhase;
            var pos = transform.position;
            pos.z -= _speed * deltaTime;
            pos.x = _baseX + (Mathf.Sin(t * 1.7f) * _weaveAmplitude);
            pos.y = _baseY + (Mathf.Sin(t * 7.2f) * _wobbleAmplitude);
            transform.position = pos;
            transform.localRotation = Quaternion.Euler(0f, Mathf.Sin(t * 3f) * 9f, Mathf.Sin(t * 8f) * 2.5f);

            if (pos.z <= breachZ)
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
