using System;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class PlayerSaveData
    {
        public string sceneName;
        public float positionX, positionY, positionZ;
        public int level = 1;
        public int currentExperience;
    }
}