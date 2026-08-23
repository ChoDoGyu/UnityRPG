using System;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class CheckpointSaveData
    {
        public bool hasCheckpoint;
        public string checkpointId;
        public string sceneName;
        public float positionX, positionY, positionZ;
    }
}