using UnityEngine;
using UnityEngine.InputSystem;

namespace InfiniStacker.Player
{
    public sealed class PlayerDragMover : MonoBehaviour
    {
        [SerializeField] private float moveSmoothing = 18f;
        [SerializeField] private float dragToWorldScale = 14f;
        [SerializeField] private float minX = -3.6f;
        [SerializeField] private float maxX = 3.6f;

        private bool _isDragging;
        private Vector2 _lastPointerPosition;
        private float _targetX;

        public bool MovementEnabled { get; private set; }

        private void Awake()
        {
            _targetX = transform.position.x;
        }

        public void Configure(float horizontalBounds)
        {
            var clampedBounds = Mathf.Max(0.5f, horizontalBounds);
            Configure(-clampedBounds, clampedBounds);
        }

        public void Configure(float newMinX, float newMaxX)
        {
            minX = Mathf.Min(newMinX, newMaxX);
            maxX = Mathf.Max(newMinX, newMaxX);
            _targetX = Mathf.Clamp(transform.position.x, minX, maxX);
        }

        public void SetEnabled(bool enabled)
        {
            MovementEnabled = enabled;
            if (!enabled)
            {
                _isDragging = false;
            }
        }

        private void Update()
        {
            if (!MovementEnabled)
            {
                return;
            }

            var pressed = false;
            var pointerPosition = Vector2.zero;

            var touch = Touchscreen.current;
            if (touch != null && touch.primaryTouch.press.isPressed)
            {
                pressed = true;
                pointerPosition = touch.primaryTouch.position.ReadValue();
            }
            else
            {
                var mouse = Mouse.current;
                if (mouse != null && mouse.leftButton.isPressed)
                {
                    pressed = true;
                    pointerPosition = mouse.position.ReadValue();
                }
            }

            if (pressed)
            {
                if (!_isDragging)
                {
                    _isDragging = true;
                    _lastPointerPosition = pointerPosition;
                }

                var deltaX = pointerPosition.x - _lastPointerPosition.x;
                _lastPointerPosition = pointerPosition;

                var normalized = deltaX / Mathf.Max(1f, Screen.width);
                _targetX = Mathf.Clamp(_targetX + (normalized * dragToWorldScale), minX, maxX);
            }
            else
            {
                _isDragging = false;
            }

            var current = transform.position;
            current.x = Mathf.Lerp(current.x, _targetX, 1f - Mathf.Exp(-moveSmoothing * Time.deltaTime));
            transform.position = current;
        }
    }
}
