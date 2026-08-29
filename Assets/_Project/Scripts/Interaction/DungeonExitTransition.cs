using UnityEngine;
using UnityRPG.Core;
using UnityRPG.Infrastructure.Save;

namespace UnityRPG.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DungeonExitInteractable))]
    public sealed class DungeonExitTransition : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private AudioClip portalActivateSfx;

        private DungeonExitInteractable exitInteractable;

        private void Awake()
        {
            exitInteractable = GetComponent<DungeonExitInteractable>();
        }

        private void OnEnable()
        {
            exitInteractable.Interacted += HandleInteracted;
        }

        private void OnDisable()
        {
            exitInteractable.Interacted -= HandleInteracted;
        }

        private void HandleInteracted(GameObject interactor)
        {
            if (SceneTransitionService.Instance == null)
            {
                Debug.LogError("[Dungeon] SceneTransitionService를 찾을 수 없습니다.", this);
                return;
            }

            SaveGameController saveGameController =
                interactor.GetComponentInParent<SaveGameController>();

            if (saveGameController == null)
            {
                Debug.LogError("[Save] SaveGameController를 찾을 수 없습니다.", this);
                return;
            }

            SaveLoadStatus status = saveGameController.SaveGame();

            if (status != SaveLoadStatus.Success)
            {
                Debug.LogError($"[Save] Hub 복귀 전 저장에 실패했습니다: {status}", this);
                return;
            }

            AudioService.Instance?.PlaySfx(portalActivateSfx);
            SceneTransitionService.Instance.LoadScene(SceneNames.Hub);
        }
    }
}