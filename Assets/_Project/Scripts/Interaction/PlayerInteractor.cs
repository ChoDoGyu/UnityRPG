using UnityEngine;

namespace UnityRPG.Interaction
{
    [DisallowMultipleComponent]
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private Transform interactionOrigin;
        [SerializeField, Min(0.1f)] private float interactionRadius = 1.5f;
        [SerializeField, Min(0f)] private float forwardOffset = 1f;
        [SerializeField] private LayerMask interactionMask = ~0;

        private readonly Collider[] overlapResults = new Collider[16];

        public bool TryInteract()
        {
            IInteractable target = FindBestTarget();

            if (target == null)
                return false;

            target.Interact(gameObject);
            return true;
        }

        private IInteractable FindBestTarget()
        {
            if (interactionOrigin == null)
                return null;

            Vector3 center = interactionOrigin.position + interactionOrigin.forward * forwardOffset;
            int count = Physics.OverlapSphereNonAlloc(center, interactionRadius, overlapResults, interactionMask);

            IInteractable bestTarget = null;
            float bestDistanceSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                IInteractable interactable = FindInteractable(overlapResults[i]);

                if (interactable == null || !interactable.CanInteract(gameObject))
                    continue;

                Vector3 difference = interactable.InteractionTransform.position - interactionOrigin.position;
                float distanceSqr = difference.sqrMagnitude;

                if (distanceSqr >= bestDistanceSqr)
                    continue;

                bestTarget = interactable;
                bestDistanceSqr = distanceSqr;
            }

            return bestTarget;
        }

        private static IInteractable FindInteractable(Collider targetCollider)
        {
            MonoBehaviour[] behaviours = targetCollider.GetComponentsInParent<MonoBehaviour>();

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IInteractable interactable)
                    return interactable;
            }

            return null;
        }

        private void OnValidate()
        {
            interactionRadius = Mathf.Max(0.1f, interactionRadius);
            forwardOffset = Mathf.Max(0f, forwardOffset);
        }

        private void OnDrawGizmosSelected()
        {
            if (interactionOrigin == null)
                return;

            Vector3 center = interactionOrigin.position + interactionOrigin.forward * forwardOffset;
            Gizmos.DrawWireSphere(center, interactionRadius);
        }
    }
}