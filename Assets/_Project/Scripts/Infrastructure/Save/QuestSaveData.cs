using System;
using System.Collections.Generic;

namespace UnityRPG.Infrastructure.Save
{
    [Serializable]
    public sealed class QuestSaveData
    {
        public string questId;
        public string state;
        public List<QuestObjectiveSaveData> objectives = new();
    }
}