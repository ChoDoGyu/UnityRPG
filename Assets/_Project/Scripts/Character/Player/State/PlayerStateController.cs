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

        private void Awake()
        {
            CurrentState = PlayerState.Normal;
        }

        public bool TryEnterDodge()
        {
            if (!CanDodge)
            {
                return false;
            }

            CurrentState = PlayerState.Dodging;
            return true;
        }

        public void ExitDodge()
        {
            if (CurrentState != PlayerState.Dodging)
            {
                return;
            }

            CurrentState = PlayerState.Normal;
        }
    }
}