using System.Collections.Generic;
using UnityEngine;
using UnityRPG.Character.Player;

namespace UnityRPG.DeveloperTools
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DeveloperConsole))]
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class DeveloperConsoleView : MonoBehaviour
    {
        private const string InputControlName = "DeveloperConsoleInput";
        private const float ReferenceWidth = 1920f;
        private const float ReferenceHeight = 1080f;

        [Header("View")]
        [SerializeField, Min(100f)] private float panelHeight = 260f;
        [SerializeField, Min(1)] private int maxHistoryLines = 12;

        private readonly List<string> history = new();

        private DeveloperConsole developerConsole;
        private PlayerInputReader inputReader;
        private GUIStyle titleStyle;
        private GUIStyle historyStyle;
        private GUIStyle inputStyle;

        private string input = string.Empty;
        private bool isOpen;
        private bool focusInput;

        public bool IsOpen => isOpen;

        private void Awake()
        {
            developerConsole = GetComponent<DeveloperConsole>();
            inputReader = GetComponent<PlayerInputReader>();

            if (!developerConsole.IsAvailable)
                enabled = false;
        }

        private void Update()
        {
            if (inputReader.WasDeveloperConsolePressed)
                SetOpen(!isOpen);
        }

        private void OnGUI()
        {
            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.BackQuote)
            {
                currentEvent.Use();
                return;
            }

            if (!isOpen)
                return;

            if (historyStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.fontSize = 20;
                titleStyle.alignment = TextAnchor.MiddleLeft;

                historyStyle = new GUIStyle(GUI.skin.label);
                historyStyle.fontSize = 18;
                historyStyle.wordWrap = true;

                inputStyle = new GUIStyle(GUI.skin.textField);
                inputStyle.fontSize = 18;
            }

            float uiScale = GetUIScale();
            float screenWidth = Screen.width / uiScale;
            float screenHeight = Screen.height / uiScale;

            Matrix4x4 previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(uiScale, uiScale, 1f));

            bool submit = currentEvent.type == EventType.KeyDown &&
                          (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter);

            float margin = 10f;
            float titleHeight = 32f;
            float inputHeight = 32f;

            float panelY = screenHeight - panelHeight;
            float inputY = screenHeight - margin - inputHeight;
            float historyY = panelY + margin + titleHeight;
            float historyWidth = screenWidth - margin * 2f;
            float historyHeight = inputY - historyY - 10f;

            GUI.Box(new Rect(0f, panelY, screenWidth, panelHeight), GUIContent.none);
            GUI.Label(new Rect(margin, panelY + margin, historyWidth, titleHeight), "Developer Console [`]", titleStyle);

            string historyText = BuildVisibleHistoryText(historyWidth, historyHeight);
            GUI.Label(new Rect(margin, historyY, historyWidth, historyHeight), historyText, historyStyle);

            GUI.SetNextControlName(InputControlName);
            input = GUI.TextField(new Rect(margin, inputY, historyWidth, inputHeight), input, inputStyle);

            if (focusInput)
            {
                GUI.FocusControl(InputControlName);
                focusInput = false;
            }

            if (submit)
            {
                Submit();
                currentEvent.Use();
            }

            GUI.matrix = previousMatrix;
        }

        private float GetUIScale()
        {
            float widthScale = Screen.width / ReferenceWidth;
            float heightScale = Screen.height / ReferenceHeight;

            return Mathf.Sqrt(widthScale * heightScale);
        }

        private void Submit()
        {
            string command = input.Trim();

            if (command.Length == 0)
            {
                input = string.Empty;
                focusInput = true;
                return;
            }

            AddHistory($"> {command}");

            ConsoleCommandResult result = developerConsole.Execute(command);
            AddHistory($"{(result.Success ? "[OK]" : "[FAIL]")} {result.Message}");

            input = string.Empty;
            focusInput = true;
        }

        private void AddHistory(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            string[] lines = message.Split('\n');

            for (int i = 0; i < lines.Length; i++)
                history.Add(lines[i]);

            while (history.Count > maxHistoryLines)
                history.RemoveAt(0);
        }

        private string BuildVisibleHistoryText(float width, float height)
        {
            if (history.Count == 0)
                return string.Empty;

            for (int startIndex = 0; startIndex < history.Count; startIndex++)
            {
                string text = string.Join("\n", history.GetRange(startIndex, history.Count - startIndex));
                float requiredHeight = historyStyle.CalcHeight(new GUIContent(text), width);

                if (requiredHeight <= height)
                    return text;
            }

            return history[history.Count - 1];
        }

        private void SetOpen(bool open)
        {
            isOpen = open;
            input = string.Empty;

            if (open)
            {
                inputReader.BlockGameplayInput(this);
                inputReader.BlockUIInput(this);
                focusInput = true;
            }
            else
            {
                inputReader.UnblockGameplayInput(this);
                inputReader.UnblockUIInput(this);
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.UnblockGameplayInput(this);
                inputReader.UnblockUIInput(this);
            }

            isOpen = false;
        }
    }
}