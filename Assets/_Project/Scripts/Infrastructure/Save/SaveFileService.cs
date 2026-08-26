using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityRPG.Infrastructure.Save
{
    public sealed class SaveFileService
    {
        private const string DefaultFileName = "save_0.json";

        private readonly string filePath;

        public string FilePath => filePath;

        public SaveFileService(string fileName = DefaultFileName)
        {
            filePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        public SaveLoadStatus Save(SaveGameData data)
        {
            if (data == null)
                return SaveLoadStatus.InvalidData;

            try
            {
                data.version = SaveGameData.CurrentVersion;
                data.savedAtUtc = DateTime.UtcNow.ToString("O");

                string json = JsonUtility.ToJson(data, true);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, json, Encoding.UTF8);

                return SaveLoadStatus.Success;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[Save] 파일 저장 실패: {exception.Message}");
                return SaveLoadStatus.IoError;
            }
        }

        public SaveLoadStatus Load(out SaveGameData data)
        {
            data = null;

            if (!File.Exists(filePath))
                return SaveLoadStatus.FileNotFound;

            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(json))
                    return SaveLoadStatus.InvalidData;

                SaveGameData loadedData = JsonUtility.FromJson<SaveGameData>(json);

                if (loadedData == null)
                    return SaveLoadStatus.InvalidData;

                if (loadedData.version != SaveGameData.CurrentVersion)
                    return SaveLoadStatus.UnsupportedVersion;

                if (!HasRequiredData(loadedData))
                    return SaveLoadStatus.InvalidData;

                data = loadedData;
                return SaveLoadStatus.Success;
            }
            catch (ArgumentException exception)
            {
                Debug.LogWarning($"[Save] JSON 데이터가 손상되었습니다: {exception.Message}");
                return SaveLoadStatus.InvalidData;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[Save] 파일 불러오기 실패: {exception.Message}");
                return SaveLoadStatus.IoError;
            }
        }

        public bool Exists()
        {
            return File.Exists(filePath);
        }

        public bool Delete()
        {
            if (!File.Exists(filePath))
                return false;

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[Save] 파일 삭제 실패: {exception.Message}");
                return false;
            }
        }

        private static bool HasRequiredData(SaveGameData data)
        {
            return data.player != null &&
                   data.inventory != null &&
                   data.equipment != null &&
                   data.quests != null &&
                   data.encounters != null &&
                   data.checkpoint != null;
        }
    }
}