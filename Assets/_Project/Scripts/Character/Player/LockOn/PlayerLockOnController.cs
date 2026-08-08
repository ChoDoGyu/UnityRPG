using UnityEngine;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(PlayerTargetFinder))]
    public sealed class PlayerLockOnController : MonoBehaviour
    {
        private PlayerTargetFinder targetFinder;

        public LockOnTarget CurrentTarget { get; private set; }

        public bool IsLockedOn =>
            CurrentTarget != null;

        private void Awake()
        {
            targetFinder =
                GetComponent<PlayerTargetFinder>();
        }

        public bool ToggleLockOn(
            Transform cameraReference)
        {
            if (!IsLockedOn)
            {
                return TryLockOn(
                    cameraReference);
            }

            LockOnTarget nextTarget =
                targetFinder.FindBestTarget(
                    cameraReference);

            if (nextTarget != null &&
                nextTarget != CurrentTarget)
            {
                CurrentTarget = nextTarget;
                return true;
            }

            Unlock();
            return false;
        }

        public bool TryLockOn(
            Transform cameraReference)
        {
            LockOnTarget target =
                targetFinder.FindBestTarget(
                    cameraReference);

            if (target == null)
            {
                return false;
            }

            CurrentTarget = target;
            return true;
        }

        public void Unlock()
        {
            CurrentTarget = null;
        }
    }
}