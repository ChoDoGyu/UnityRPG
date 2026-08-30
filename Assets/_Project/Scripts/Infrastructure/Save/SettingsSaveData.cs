using System;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class SettingsSaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public string savedAtUtc;

        public int resolutionWidth;
        public int resolutionHeight;
        public bool fullscreen;

        public float masterVolume = 1f;
        public float bgmVolume = 1f;
        public float sfxVolume = 1f;
    }
}