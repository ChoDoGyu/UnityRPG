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

        [Header("View")]
        [SerializeField, Min(100f)] private float panelHeight = 260f;
        [SerializeField, Min(1)] private int maxHistoryLines = 12;

        private readonly List<string> history = new();

        private DeveloperConsole developerConsole;
        private PlayerInputReader inputReader;
        private GUIStyle historyStyle;

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
                historyStyle = new GUIStyle(GUI.skin.label);
                historyStyle.wordWrap = true;
            }

            bool submit = currentEvent.type == EventType.KeyDown &&
                          (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter);

            float margin = 10f;
            float panelY = Screen.height - panelHeight;
            float inputY = Screen.height - 35f;
            float historyY = panelY + 35f;
            float historyWidth = Screen.width - margin * 2f;
            float historyHeight = inputY - historyY - 10f;

            GUI.Box(new Rect(0f, panelY, Screen.width, panelHeight), GUIContent.none);
            GUI.Label(new Rect(margin, panelY + margin, historyWidth, 25f), "Developer Console [`]");

            string historyText = BuildVisibleHistoryText(historyWidth, historyHeight);
            GUI.Label(new Rect(margin, historyY, historyWidth, historyHeight), historyText, historyStyle);

            GUI.SetNextControlName(InputControlName);
            input = GUI.TextField(new Rect(margin, inputY, historyWidth, 25f), input);

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

            inputReader.SetGameplayInputEnabled(!open);

            if (open)
                focusInput = true;
        }

        private void OnDisable()
        {
            if (isOpen && inputReader != null)
                inputReader.SetGameplayInputEnabled(true);

            isOpen = false;
        }
    }
}