using System;
using UnityEngine;

namespace UnityRPG.Interaction
{
    [DisallowMultipleComponent]
    public sealed class NpcInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private NpcDefinition definition;
        [SerializeField] private Transform interactionPoint;

        public event Action<GameObject> Interacted;

        public NpcDefinition Definition => definition;
        public Transform InteractionTransform => interactionPoint != null ? interactionPoint : transform;

        public bool CanInteract(GameObject interactor) => definition != null && interactor != null;

        public void Interact(GameObject interactor)
        {
            if (!CanInteract(interactor))
                return;

            Interacted?.Invoke(interactor);
            Debug.Log($"[NPC] {definition.DisplayName}: {definition.DefaultDialogue}", this);
        }

        private void OnValidate()
        {
            if (definition == null)
                Debug.LogWarning($"[NPC] {name}에 NpcDefinition이 설정되지 않았습니다.", this);
        }
    }
}