using UnityEngine;
using UnityRPG.Core;

namespace UnityRPG.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DungeonEntranceInteractable))]
    public sealed class DungeonEntranceTransition : MonoBehaviour
    {
        private DungeonEntranceInteractable entrance;

        private void Awake()
        {
            entrance = GetComponent<DungeonEntranceInteractable>();
        }

        private void OnEnable()
        {
            entrance.Interacted += HandleInteracted;
        }

        private void OnDisable()
        {
            entrance.Interacted -= HandleInteracted;
        }

        private void HandleInteracted(GameObject interactor)
        {
            if (SceneTransitionService.Instance == null)
            {
                Debug.LogError("[Dungeon] SceneTransitionService를 찾을 수 없습니다.", this);
                return;
            }

            SceneTransitionService.Instance.LoadScene(SceneNames.Dungeon);
        }
    }
}