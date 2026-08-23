using UnityEngine;

namespace UnityRPG.Interaction
{
    public interface IInteractable
    {
        Transform InteractionTransform { get; }
        bool CanInteract(GameObject interactor);
        void Interact(GameObject interactor);
    }
}