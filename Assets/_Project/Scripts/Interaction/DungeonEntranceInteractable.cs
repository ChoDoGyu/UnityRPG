using System;
using UnityEngine;

namespace UnityRPG.Interaction
{
    [DisallowMultipleComponent]
    public sealed class DungeonEntranceInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform interactionPoint;

        public event Action<GameObject> Interacted;

        public Transform InteractionTransform => interactionPoint != null ? interactionPoint : transform;

        public bool CanInteract(GameObject interactor) => interactor != null;

        public void Interact(GameObject interactor)
        {
            if (!CanInteract(interactor))
                return;

            Interacted?.Invoke(interactor);
            Debug.Log("[Dungeon] 던전 입구와 상호작용했습니다.", this);
        }
    }
}