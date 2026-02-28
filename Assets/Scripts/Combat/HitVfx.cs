using UnityEngine;

namespace InfiniStacker.Combat
{
    public sealed class HitVfx : MonoBehaviour
    {
        private float _remaining;
        private HitVfxPool _pool;

        public void Play(Vector3 worldPosition, HitVfxPool pool)
        {
            transform.position = worldPosition;
            transform.localScale = Vector3.one * 0.35f;
            _pool = pool;
            _remaining = 0.12f;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            _remaining -= Time.deltaTime;
            transform.localScale *= 0.92f;

            if (_remaining <= 0f)
            {
                _pool?.ReturnToPool(this);
            }
        }
    }
}
