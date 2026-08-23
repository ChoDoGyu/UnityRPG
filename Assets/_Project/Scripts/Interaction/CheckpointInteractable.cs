using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRPG.Character.Player;

namespace UnityRPG.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class CheckpointInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string checkpointId;
        [SerializeField] private Transform interactionPoint;
        [SerializeField] private Transform respawnPoint;

        public Transform InteractionTransform => interactionPoint != null ? interactionPoint : transform;

        public bool CanInteract(GameObject interactor)
        {
            return interactor != null && !string.IsNullOrWhiteSpace(checkpointId);
        }

        public void Interact(GameObject interactor)
        {
            if (!CanInteract(interactor))
                return;

            PlayerCheckpointController checkpointController = interactor.GetComponentInParent<PlayerCheckpointController>();

            if (checkpointController == null)
                return;

            Vector3 position = respawnPoint != null ? respawnPoint.position : transform.position;
            string sceneName = SceneManager.GetActiveScene().name;

            if (checkpointController.ActivateCheckpoint(checkpointId, sceneName, position))
                Debug.Log($"[Checkpoint] 활성화: {checkpointId}", this);
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(checkpointId))
                Debug.LogWarning($"[Checkpoint] {name}의 CheckpointId가 비어 있습니다.", this);
        }
    }
}