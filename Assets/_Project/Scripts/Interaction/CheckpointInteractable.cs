using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRPG.Character.Player;
using UnityRPG.Infrastructure.Save;

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

            if (!checkpointController.TryActivateCheckpoint(checkpointId, sceneName, position))
                return;

            Debug.Log($"[Checkpoint] 활성화: {checkpointId}", this);

            SaveGameController saveGameController = interactor.GetComponentInParent<SaveGameController>();

            if (saveGameController == null)
            {
                Debug.LogError("[Save] Checkpoint 저장용 SaveGameController를 찾을 수 없습니다.", this);
                return;
            }

            SaveLoadStatus status = saveGameController.SaveGame();

            if (status != SaveLoadStatus.Success)
                Debug.LogError($"[Save] Checkpoint 자동 저장에 실패했습니다: {status}", this);
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(checkpointId))
                Debug.LogWarning($"[Checkpoint] {name}의 CheckpointId가 비어 있습니다.", this);
        }
    }
}