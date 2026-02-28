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
        private TMP_Text _timerLabel;
        private TMP_Text _baseHpLabel;
        private GameObject _startPanel;
        private GameObject _endPanel;
        private TMP_Text _endMessage;
        private Button _startButton;
        private Button _restartButton;
        private GameStateController _gameStateController;

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
            BuildHud();
            BuildPanels();
        }

        private void OnEnable()
        {
            GameEvents.TimerChanged += OnTimerChanged;
            GameEvents.BaseHpChanged += OnBaseHpChanged;
            GameEvents.GameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.TimerChanged -= OnTimerChanged;
            GameEvents.BaseHpChanged -= OnBaseHpChanged;
            GameEvents.GameStateChanged -= OnGameStateChanged;
        }

        private void OnTimerChanged(float secondsRemaining)
        {
            _timerLabel.text = $"Time: {Mathf.CeilToInt(secondsRemaining)}";
        }

        private void OnBaseHpChanged(int currentHp, int maxHp)
        {
            _baseHpLabel.text = $"Base: {currentHp}";
        }

        private void OnGameStateChanged(GameState state, GameResult? result)
        {
            _startPanel.SetActive(state == GameState.Start);
            _endPanel.SetActive(state == GameState.GameOver || state == GameState.Victory);

            if (state == GameState.Victory)
            {
                _endMessage.text = "Victory";
            }
            else if (state == GameState.GameOver)
            {
                _endMessage.text = result == GameResult.DefeatByBase ? "Game Over\nBase Destroyed" : "Game Over\nSquad Eliminated";
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
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private void BuildHud()
        {
            _timerLabel = CreateLabel("TimerLabel", new Vector2(18f, -20f), TextAlignmentOptions.TopLeft, 62f);
            _baseHpLabel = CreateLabel("BaseLabel", new Vector2(0f, -20f), TextAlignmentOptions.Top, 62f);

            _timerLabel.text = "Time: 60";
            _baseHpLabel.text = "Base: 0";
        }

        private void BuildPanels()
        {
            _startPanel = CreatePanel("StartPanel", new Vector2(0f, 0f), new Vector2(560f, 380f));
            _endPanel = CreatePanel("EndPanel", new Vector2(0f, 0f), new Vector2(560f, 380f));

            var startTitle = CreatePanelText(_startPanel.transform, "Title", "InfiniStacker", new Vector2(0f, 85f), 86f);
            startTitle.alignment = TextAlignmentOptions.Center;
            _startButton = CreateButton(_startPanel.transform, "StartButton", "Start", new Vector2(0f, -60f));

            _endMessage = CreatePanelText(_endPanel.transform, "Result", "Victory", new Vector2(0f, 70f), 72f);
            _endMessage.alignment = TextAlignmentOptions.Center;
            _restartButton = CreateButton(_endPanel.transform, "RestartButton", "Restart", new Vector2(0f, -60f));

            _endPanel.SetActive(false);
        }

        private TMP_Text CreateLabel(string objectName, Vector2 anchoredPosition, TextAlignmentOptions alignment, float width)
        {
            var go = new GameObject(objectName, typeof(RectTransform));
            go.transform.SetParent(transform, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(width * 5f, 120f);

            var text = go.AddComponent<TextMeshProUGUI>();
            text.fontSize = 62f;
            text.alignment = alignment;
            text.color = Color.white;
            text.outlineColor = new Color(0f, 0f, 0f, 0.8f);
            text.outlineWidth = 0.2f;
            return text;
        }

        private GameObject CreatePanel(string objectName, Vector2 anchoredPosition, Vector2 size)
        {
            var panel = new GameObject(objectName, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(transform, false);

            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            var image = panel.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.68f);

            return panel;
        }

        private TMP_Text CreatePanelText(Transform parent, string objectName, string value, Vector2 anchoredPosition, float fontSize)
        {
            var go = new GameObject(objectName, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(460f, 160f);

            var text = go.AddComponent<TextMeshProUGUI>();
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.outlineColor = new Color(0f, 0f, 0f, 0.8f);
            text.outlineWidth = 0.2f;
            return text;
        }

        private Button CreateButton(Transform parent, string objectName, string label, Vector2 anchoredPosition)
        {
            var buttonGo = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonGo.transform.SetParent(parent, false);

            var rect = buttonGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(280f, 120f);

            var image = buttonGo.GetComponent<Image>();
            image.color = new Color(0.16f, 0.58f, 0.95f, 1f);

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
            text.fontSize = 52f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            return button;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            _ = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }
    }
}
