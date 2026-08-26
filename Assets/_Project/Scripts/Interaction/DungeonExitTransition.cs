using UnityEngine;
using UnityRPG.Core;

namespace UnityRPG.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DungeonExitInteractable))]
    public sealed class DungeonExitTransition : MonoBehaviour
    {
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

            SceneTransitionService.Instance.LoadScene(SceneNames.Hub);
        }
    }
}