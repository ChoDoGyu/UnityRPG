using System;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class EncounterSaveData
    {
        public string encounterId;
        public bool hasStarted;
        public bool isCompleted;
    }
}