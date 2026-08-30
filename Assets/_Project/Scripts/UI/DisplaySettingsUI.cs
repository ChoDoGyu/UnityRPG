using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class DisplaySettingsUI : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        private readonly List<Vector2Int> resolutions = new List<Vector2Int>();

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] DisplaySettingsUI의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            BuildResolutionOptions();
            RefreshCurrentValues();

            resolutionDropdown.onValueChanged.AddListener(HandleResolutionChanged);
            fullscreenToggle.onValueChanged.AddListener(HandleFullscreenChanged);
        }

        private void OnDestroy()
        {
            if (resolutionDropdown != null)
                resolutionDropdown.onValueChanged.RemoveListener(HandleResolutionChanged);

            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.RemoveListener(HandleFullscreenChanged);
        }

        private void BuildResolutionOptions()
        {
            resolutions.Clear();
            resolutionDropdown.ClearOptions();

            Resolution[] availableResolutions = Screen.resolutions;

            for (int i = 0; i < availableResolutions.Length; i++)
            {
                Vector2Int resolution = new Vector2Int(
                    availableResolutions[i].width,
                    availableResolutions[i].height);

                if (!resolutions.Contains(resolution))
                    resolutions.Add(resolution);
            }

            if (resolutions.Count == 0)
                resolutions.Add(new Vector2Int(Screen.width, Screen.height));

            resolutions.Sort((a, b) =>
            {
                int pixelComparison = (b.x * b.y).CompareTo(a.x * a.y);

                if (pixelComparison != 0)
                    return pixelComparison;

                return b.x.CompareTo(a.x);
            });

            List<string> options = new List<string>();

            for (int i = 0; i < resolutions.Count; i++)
                options.Add($"{resolutions[i].x} x {resolutions[i].y}");

            resolutionDropdown.AddOptions(options);
        }

        private void RefreshCurrentValues()
        {
            int currentIndex = FindCurrentResolutionIndex();

            resolutionDropdown.SetValueWithoutNotify(currentIndex);
            resolutionDropdown.RefreshShownValue();

            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        }

        private int FindCurrentResolutionIndex()
        {
            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i].x == Screen.width && resolutions[i].y == Screen.height)
                    return i;
            }

            return 0;
        }

        private void HandleResolutionChanged(int index)
        {
            if (index < 0 || index >= resolutions.Count)
                return;

            Vector2Int resolution = resolutions[index];

            ApplyDisplaySettings(resolution.x, resolution.y, fullscreenToggle.isOn);

            UISfxService.Instance?.PlayClick();
        }

        private void HandleFullscreenChanged(bool fullscreen)
        {
            int index = resolutionDropdown.value;

            if (index < 0 || index >= resolutions.Count)
                return;

            Vector2Int resolution = resolutions[index];

            ApplyDisplaySettings(resolution.x, resolution.y, fullscreen);

            UISfxService.Instance?.PlayClick();
        }

        private static void ApplyDisplaySettings(int width, int height, bool fullscreen)
        {
            FullScreenMode mode = fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.SetResolution(width, height, mode);
        }

        private bool HasAllReferences()
        {
            return resolutionDropdown != null &&
                   fullscreenToggle != null;
        }
    }
}