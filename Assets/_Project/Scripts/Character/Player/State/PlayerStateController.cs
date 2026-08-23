using UnityEngine;

namespace UnityRPG.Character.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerStateController : MonoBehaviour
    {
        public PlayerState CurrentState { get; private set; }

        public bool CanMove => CurrentState == PlayerState.Normal;

        public bool CanRotate => CurrentState == PlayerState.Normal;

        public bool CanDodge => CurrentState == PlayerState.Normal;

        public bool CanAttack =>
            CurrentState == PlayerState.Normal ||
            CurrentState == PlayerState.Attacking;

        public bool CanUseSkill => CurrentState == PlayerState.Normal;

        private void Awake()
        {
            CurrentState = PlayerState.Normal;
        }

        public bool TryEnterAttack()
        {
            if (CurrentState != PlayerState.Normal)
            {
                return false;
            }

            CurrentState = PlayerState.Attacking;
            return true;
        }

        public void ExitAttack()
        {
            if (CurrentState != PlayerState.Attacking)
            {
                return;
            }

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

        public bool TryEnterSkill()
        {
            if (!CanUseSkill)
            {
                return false;
            }

            CurrentState = PlayerState.UsingSkill;
            return true;
        }

        public void ExitSkill()
        {
            if (CurrentState != PlayerState.UsingSkill)
            {
                return;
            }

            CurrentState = PlayerState.Normal;
        }

        public void EnterDead()
        {
            CurrentState = PlayerState.Dead;
        }

        public void ExitDead()
        {
            if (CurrentState != PlayerState.Dead)
                return;

            CurrentState = PlayerState.Normal;
        }
    }
}