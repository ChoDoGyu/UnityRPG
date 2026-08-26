using UnityEngine;
using UnityRPG.AI;

namespace UnityRPG.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EncounterTrigger))]
    public sealed class DungeonExitPortalUnlocker : MonoBehaviour
    {
        [SerializeField] private GameObject portalRoot;

        private EncounterTrigger encounterTrigger;

        private void Awake()
        {
            encounterTrigger = GetComponent<EncounterTrigger>();
        }

        private void OnEnable()
        {
            encounterTrigger.StateChanged += RefreshPortalState;
        }

        private void Start()
        {
            if (portalRoot == null)
            {
                Debug.LogError("[Dungeon] 귀환 포탈 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            RefreshPortalState();
        }

        private void OnDisable()
        {
            if (encounterTrigger != null)
                encounterTrigger.StateChanged -= RefreshPortalState;
        }

        private void RefreshPortalState()
        {
            portalRoot.SetActive(encounterTrigger.IsCompleted);
        }
    }
}