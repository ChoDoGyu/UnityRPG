using System;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class QuestObjectiveSaveData
    {
        public int objectiveIndex;
        public int currentAmount;
    }
}