using UnityEngine;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerStateController : MonoBehaviour
    {
        public PlayerState CurrentState { get; private set; }

        public bool CanMove =>
            CurrentState == PlayerState.Normal;

        public bool CanRotate =>
            CurrentState == PlayerState.Normal;

        public bool CanDodge =>
            CurrentState == PlayerState.Normal;

        public bool CanAttack =>
            CurrentState == PlayerState.Normal ||
            CurrentState == PlayerState.Attacking;

        private void Awake()
        {
            CurrentState =
                PlayerState.Normal;
        }

        public bool TryEnterAttack()
        {
            if (CurrentState != PlayerState.Normal)
            {
                return false;
            }

            CurrentState =
                PlayerState.Attacking;

            return true;
        }

        public void ExitAttack()
        {
            if (CurrentState != PlayerState.Attacking)
            {
                return;
            }

            CurrentState =
                PlayerState.Normal;
        }

        public bool TryEnterDodge()
        {
            if (!CanDodge)
            {
                return false;
            }

            CurrentState =
                PlayerState.Dodging;

            return true;
        }

        public void ExitDodge()
        {
            if (CurrentState != PlayerState.Dodging)
            {
                return;
            }

            CurrentState =
                PlayerState.Normal;
        }
    }
}