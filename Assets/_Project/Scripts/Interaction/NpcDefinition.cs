using UnityEngine;

namespace UnityRPG.Interaction
{
    [CreateAssetMenu(fileName = "NpcDefinition", menuName = "UnityRPG/Interaction/NPC Definition")]
    public sealed class NpcDefinition : ScriptableObject
    {
        [SerializeField] private string npcId;
        [SerializeField] private string displayName;
        [TextArea][SerializeField] private string defaultDialogue;

        public string NpcId => npcId;
        public string DisplayName => displayName;
        public string DefaultDialogue => defaultDialogue;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(npcId))
                Debug.LogWarning($"[NPC] {name}의 NpcId가 비어 있습니다.", this);

            if (string.IsNullOrWhiteSpace(displayName))
                Debug.LogWarning($"[NPC] {name}의 DisplayName이 비어 있습니다.", this);
        }
    }
}