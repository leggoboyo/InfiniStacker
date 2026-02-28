using UnityEngine;

namespace InfiniStacker.Feedback
{
    public sealed class ScreenShakeService : MonoBehaviour
    {
        private Transform _cameraTransform;
        private Vector3 _baseLocalPosition;
        private float _remainingDuration;
        private float _amplitude;

        public void Initialize(Transform cameraTransform)
        {
            _cameraTransform = cameraTransform;
            if (_cameraTransform != null)
            {
                _baseLocalPosition = _cameraTransform.localPosition;
            }
        }

        public void Shake(float amplitude, float duration)
        {
            _amplitude = Mathf.Max(_amplitude, amplitude);
            _remainingDuration = Mathf.Max(_remainingDuration, duration);
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    return;
                }

                _cameraTransform = mainCamera.transform;
                _baseLocalPosition = _cameraTransform.localPosition;
            }

            if (_remainingDuration <= 0f)
            {
                _cameraTransform.localPosition = _baseLocalPosition;
                _amplitude = 0f;
                return;
            }

            _remainingDuration -= Time.deltaTime;
            var offset = Random.insideUnitSphere * _amplitude;
            offset.z = 0f;
            _cameraTransform.localPosition = _baseLocalPosition + offset;
        }
    }
}
