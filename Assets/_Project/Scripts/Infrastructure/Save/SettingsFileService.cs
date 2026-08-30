using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityRPG.Infrastructure.Save
{
    public sealed class SettingsFileService
    {
        private const string FileName = "settings.json";

        private readonly string filePath;

        public string FilePath => filePath;

        public SettingsFileService()
        {
            filePath = Path.Combine(Application.persistentDataPath, FileName);
        }

        public SaveLoadStatus Save(SettingsSaveData data)
        {
            if (data == null)
                return SaveLoadStatus.InvalidData;

            try
            {
                data.version = SettingsSaveData.CurrentVersion;
                data.savedAtUtc = DateTime.UtcNow.ToString("O");

                string json = JsonUtility.ToJson(data, true);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, json, Encoding.UTF8);

                return SaveLoadStatus.Success;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[Settings] 파일 저장 실패: {exception.Message}");
                return SaveLoadStatus.IoError;
            }
        }

        public SaveLoadStatus Load(out SettingsSaveData data)
        {
            data = null;

            if (!File.Exists(filePath))
                return SaveLoadStatus.FileNotFound;

            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(json))
                    return SaveLoadStatus.InvalidData;

                SettingsSaveData loadedData = JsonUtility.FromJson<SettingsSaveData>(json);

                if (loadedData == null)
                    return SaveLoadStatus.InvalidData;

                if (loadedData.version != SettingsSaveData.CurrentVersion)
                    return SaveLoadStatus.UnsupportedVersion;

                if (!HasRequiredData(loadedData))
                    return SaveLoadStatus.InvalidData;

                data = loadedData;
                return SaveLoadStatus.Success;
            }
            catch (ArgumentException exception)
            {
                Debug.LogWarning($"[Settings] JSON 데이터가 손상되었습니다: {exception.Message}");
                return SaveLoadStatus.InvalidData;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[Settings] 파일 불러오기 실패: {exception.Message}");
                return SaveLoadStatus.IoError;
            }
        }

        private static bool HasRequiredData(SettingsSaveData data)
        {
            return data.resolutionWidth > 0 &&
                   data.resolutionHeight > 0 &&
                   data.masterVolume >= 0f && data.masterVolume <= 1f &&
                   data.bgmVolume >= 0f && data.bgmVolume <= 1f &&
                   data.sfxVolume >= 0f && data.sfxVolume <= 1f;
        }
    }
}