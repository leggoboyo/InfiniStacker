using InfiniStacker.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace InfiniStacker.UI
{
    [RequireComponent(typeof(Canvas))]
    public sealed class GameUIController : MonoBehaviour
    {
        private Canvas _canvas;
        private Transform _safeAreaRoot;
        private TMP_Text _timerLabel;
        private TMP_Text _baseHpLabel;
        private TMP_Text _weaponTierLabel;
        private TMP_Text _weaponProgressLabel;
        private TMP_Text _controlHintLabel;
        private GameObject _startPanel;
        private GameObject _endPanel;
        private TMP_Text _endMessage;
        private Button _startButton;
        private Button _restartButton;
        private GameStateController _gameStateController;
        private float _hintHideTime;
        private bool _hintVisible;

        public void Initialize(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
            _startButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
            _startButton.onClick.AddListener(_gameStateController.StartGame);
            _restartButton.onClick.AddListener(_gameStateController.RestartGame);
        }

        private void Awake()
        {
            EnsureEventSystem();
            BuildCanvas();
            BuildSafeAreaRoot();
            BuildHud();
            BuildPanels();
        }

        private void OnEnable()
        {
            GameEvents.TimerChanged += OnTimerChanged;
            GameEvents.BaseHpChanged += OnBaseHpChanged;
            GameEvents.GameStateChanged += OnGameStateChanged;
            GameEvents.WeaponStateChanged += OnWeaponStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.TimerChanged -= OnTimerChanged;
            GameEvents.BaseHpChanged -= OnBaseHpChanged;
            GameEvents.GameStateChanged -= OnGameStateChanged;
            GameEvents.WeaponStateChanged -= OnWeaponStateChanged;
        }

        private void Update()
        {
            if (_hintVisible && Time.unscaledTime >= _hintHideTime)
            {
                SetHintVisible(false);
            }
        }

        private void OnTimerChanged(float secondsRemaining)
        {
            _timerLabel.text = $"Time {Mathf.CeilToInt(secondsRemaining)}";
        }

        private void OnBaseHpChanged(int currentHp, int maxHp)
        {
            _baseHpLabel.text = $"Base HP {currentHp}";
        }

        private void OnGameStateChanged(GameState state, GameResult? result)
        {
            _startPanel.SetActive(state == GameState.Start);
            _endPanel.SetActive(state == GameState.GameOver || state == GameState.Victory);

            if (state == GameState.Start)
            {
                SetHintVisible(true);
                _hintHideTime = float.PositiveInfinity;
            }
            else if (state == GameState.Playing)
            {
                SetHintVisible(true);
                _hintHideTime = Time.unscaledTime + 8f;
            }
            else
            {
                SetHintVisible(false);
            }

            if (state == GameState.Victory)
            {
                _endMessage.text = "Victory";
            }
            else if (state == GameState.GameOver)
            {
                _endMessage.text = result == GameResult.DefeatByBase ? "Game Over\nBase Breached" : "Game Over\nSquad Eliminated";
            }
        }

        private void OnWeaponStateChanged(
            string tierName,
            int tierIndex,
            int progressValue,
            int nextTierRequirement,
            int turretCharges)
        {
            if (_weaponTierLabel != null)
            {
                _weaponTierLabel.text = $"Weapon {tierName}";
            }

            if (_weaponProgressLabel != null)
            {
                var progressText = nextTierRequirement > 0
                    ? $"UP {progressValue}/{nextTierRequirement}"
                    : "MAX TIER";
                _weaponProgressLabel.text = $"{progressText}  Turrets {turretCharges}";
            }
        }

        private void BuildCanvas()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.pixelPerfect = false;

            var scaler = gameObject.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1170f, 2532f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.75f;

            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private void BuildSafeAreaRoot()
        {
            var safeAreaGo = new GameObject("SafeAreaRoot", typeof(RectTransform));
            safeAreaGo.transform.SetParent(transform, false);
            _safeAreaRoot = safeAreaGo.transform;

            var safeRect = safeAreaGo.GetComponent<RectTransform>();
            safeRect.anchorMin = Vector2.zero;
            safeRect.anchorMax = Vector2.one;
            safeRect.offsetMin = Vector2.zero;
            safeRect.offsetMax = Vector2.zero;

            safeAreaGo.AddComponent<SafeAreaFitter>();
        }

        private void BuildHud()
        {
            var hudBar = new GameObject("TopHudBar", typeof(RectTransform), typeof(Image));
            hudBar.transform.SetParent(_safeAreaRoot, false);

            var hudRect = hudBar.GetComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0f, 1f);
            hudRect.anchorMax = new Vector2(1f, 1f);
            hudRect.pivot = new Vector2(0.5f, 1f);
            hudRect.anchoredPosition = new Vector2(0f, 0f);
            hudRect.sizeDelta = new Vector2(0f, 135f);

            var hudImage = hudBar.GetComponent<Image>();
            hudImage.color = new Color(0.06f, 0.08f, 0.12f, 0.72f);

            _timerLabel = CreateHudLabel(
                hudBar.transform,
                "TimerLabel",
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(16f, -2f),
                TextAlignmentOptions.Left,
                38f);

            _baseHpLabel = CreateHudLabel(
                hudBar.transform,
                "BaseLabel",
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-16f, -2f),
                TextAlignmentOptions.Right,
                38f);

            _weaponTierLabel = CreateHudLabel(
                hudBar.transform,
                "WeaponTierLabel",
                new Vector2(0.5f, 0.66f),
                new Vector2(0.5f, 0.66f),
                new Vector2(0f, 12f),
                TextAlignmentOptions.Center,
                28f);

            _weaponProgressLabel = CreateHudLabel(
                hudBar.transform,
                "WeaponProgressLabel",
                new Vector2(0.5f, 0.34f),
                new Vector2(0.5f, 0.34f),
                new Vector2(0f, -16f),
                TextAlignmentOptions.Center,
                22f);

            _timerLabel.text = "Time 60";
            _baseHpLabel.text = "Base HP 0";
            _weaponTierLabel.text = "Weapon Rifle I";
            _weaponProgressLabel.text = "UP 0/18  Turrets 0";

            _controlHintLabel = CreateHudLabel(
                _safeAreaRoot,
                "ControlHint",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 170f),
                TextAlignmentOptions.Center,
                34f);
            _controlHintLabel.text = "Drag To Swap Lanes";
            _controlHintLabel.color = new Color(1f, 1f, 1f, 0.95f);
            _controlHintLabel.outlineWidth = 0.22f;
            SetHintVisible(true);
        }

        private void BuildPanels()
        {
            _startPanel = CreatePanel(_safeAreaRoot, "StartPanel", Vector2.zero, new Vector2(650f, 430f));
            _endPanel = CreatePanel(_safeAreaRoot, "EndPanel", Vector2.zero, new Vector2(650f, 430f));

            var startTitle = CreatePanelText(_startPanel.transform, "Title", "InfiniStacker", new Vector2(0f, 88f), 96f);
            startTitle.alignment = TextAlignmentOptions.Center;
            _ = CreatePanelText(_startPanel.transform, "Subtitle", "Right Lane: Squad + Zombies\nLeft Lane: Break Upgrade Ice", new Vector2(0f, 5f), 40f);
            _startButton = CreateButton(_startPanel.transform, "StartButton", "Start", new Vector2(0f, -102f));

            _endMessage = CreatePanelText(_endPanel.transform, "Result", "Victory", new Vector2(0f, 78f), 82f);
            _endMessage.alignment = TextAlignmentOptions.Center;
            _restartButton = CreateButton(_endPanel.transform, "RestartButton", "Restart", new Vector2(0f, -102f));

            _endPanel.SetActive(false);
        }

        private static TMP_Text CreateHudLabel(
            Transform parent,
            string objectName,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            TextAlignmentOptions alignment,
            float fontSize)
        {
            var go = new GameObject(objectName, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(anchorMax.x, 0.5f);

            if (anchorMin.x == 0f && anchorMax.x == 0f)
            {
                rect.pivot = new Vector2(0f, 0.5f);
            }

            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(520f, 98f);

            var text = go.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.outlineColor = new Color(0f, 0f, 0f, 0.9f);
            text.outlineWidth = 0.22f;
            return text;
        }

        private static GameObject CreatePanel(Transform parent, string objectName, Vector2 anchoredPosition, Vector2 size)
        {
            var panel = new GameObject(objectName, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);

            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            var image = panel.GetComponent<Image>();
            image.color = new Color(0.03f, 0.06f, 0.1f, 0.82f);

            return panel;
        }

        private static TMP_Text CreatePanelText(Transform parent, string objectName, string value, Vector2 anchoredPosition, float fontSize)
        {
            var go = new GameObject(objectName, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(560f, 160f);

            var text = go.AddComponent<TextMeshProUGUI>();
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.outlineColor = new Color(0f, 0f, 0f, 0.85f);
            text.outlineWidth = 0.2f;
            return text;
        }

        private static Button CreateButton(Transform parent, string objectName, string label, Vector2 anchoredPosition)
        {
            var buttonGo = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonGo.transform.SetParent(parent, false);

            var rect = buttonGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(300f, 118f);

            var image = buttonGo.GetComponent<Image>();
            image.color = new Color(0.08f, 0.56f, 0.92f, 1f);

            var button = buttonGo.GetComponent<Button>();
            button.targetGraphic = image;

            var textGo = new GameObject("Label", typeof(RectTransform));
            textGo.transform.SetParent(buttonGo.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var text = textGo.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 54f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.outlineColor = new Color(0f, 0f, 0f, 0.72f);
            text.outlineWidth = 0.16f;

            return button;
        }

        private void SetHintVisible(bool visible)
        {
            _hintVisible = visible;
            if (_controlHintLabel != null)
            {
                _controlHintLabel.gameObject.SetActive(visible);
            }
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            _ = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }
    }
}
