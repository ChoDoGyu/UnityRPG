using UnityEngine;
using UnityRPG.Core;

namespace UnityRPG.Infrastructure.Save
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioService))]
    public sealed class SettingsService : MonoBehaviour
    {
        public static SettingsService Instance { get; private set; }

        private SettingsFileService fileService;
        private AudioService audioService;
        private SettingsSaveData data;

        public int ResolutionWidth => data.resolutionWidth;
        public int ResolutionHeight => data.resolutionHeight;
        public bool Fullscreen => data.fullscreen;
        public float MasterVolume => data.masterVolume;
        public float BgmVolume => data.bgmVolume;
        public float SfxVolume => data.sfxVolume;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                enabled = false;
                return;
            }

            Instance = this;
            fileService = new SettingsFileService();
            audioService = GetComponent<AudioService>();

            SaveLoadStatus status = fileService.Load(out data);

            if (status != SaveLoadStatus.Success)
            {
                data = CreateDefaultData();

                if (status != SaveLoadStatus.FileNotFound)
                    Debug.LogWarning($"[Settings] 설정을 불러오지 못해 기본값을 사용합니다: {status}", this);
            }

            ApplyAll();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void OnApplicationQuit()
        {
            SaveSettings();
        }

        public void SetDisplaySettings(int width, int height, bool fullscreen)
        {
            data.resolutionWidth = width;
            data.resolutionHeight = height;
            data.fullscreen = fullscreen;

            FullScreenMode mode = fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.SetResolution(width, height, mode);
        }

        public void SetMasterVolume(float volume)
        {
            data.masterVolume = Mathf.Clamp01(volume);
            audioService.SetMasterVolume(data.masterVolume);
        }

        public void SetBgmVolume(float volume)
        {
            data.bgmVolume = Mathf.Clamp01(volume);
            audioService.SetBgmVolume(data.bgmVolume);
        }

        public void SetSfxVolume(float volume)
        {
            data.sfxVolume = Mathf.Clamp01(volume);
            audioService.SetSfxVolume(data.sfxVolume);
        }

        public SaveLoadStatus SaveSettings()
        {
            return fileService.Save(data);
        }

        private void ApplyAll()
        {
            SetDisplaySettings(data.resolutionWidth, data.resolutionHeight, data.fullscreen);
            audioService.SetMasterVolume(data.masterVolume);
            audioService.SetBgmVolume(data.bgmVolume);
            audioService.SetSfxVolume(data.sfxVolume);
        }

        private static SettingsSaveData CreateDefaultData()
        {
            return new SettingsSaveData
            {
                resolutionWidth = Screen.width,
                resolutionHeight = Screen.height,
                fullscreen = Screen.fullScreen,
                masterVolume = 1f,
                bgmVolume = 1f,
                sfxVolume = 1f
            };
        }
    }
}