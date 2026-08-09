using UnityEngine;
using UnityRPG.Combat;
using UnityRPG.Skill;

namespace UnityRPG.Character.Player
{
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerRotator))]
    [RequireComponent(typeof(PlayerCameraController))]
    [RequireComponent(typeof(PlayerVisualAnimator))]
    [RequireComponent(typeof(PlayerStateController))]
    [RequireComponent(typeof(PlayerDodger))]
    [RequireComponent(typeof(PlayerLockOnController))]
    [RequireComponent(typeof(PlayerAttackController))]
    [RequireComponent(typeof(PlayerSkillController))]
    public sealed class PlayerController : MonoBehaviour
    {
        private PlayerInputReader inputReader;
        private PlayerMotor playerMotor;
        private PlayerRotator playerRotator;
        private PlayerCameraController playerCameraController;
        private PlayerVisualAnimator visualAnimator;
        private PlayerStateController stateController;
        private PlayerDodger playerDodger;
        private PlayerLockOnController lockOnController;
        private PlayerAttackController attackController;
        private PlayerSkillController skillController;

        private void Awake()
        {
            inputReader = GetComponent<PlayerInputReader>();
            playerMotor = GetComponent<PlayerMotor>();
            playerRotator = GetComponent<PlayerRotator>();
            playerCameraController = GetComponent<PlayerCameraController>();
            visualAnimator = GetComponent<PlayerVisualAnimator>();
            stateController = GetComponent<PlayerStateController>();
            playerDodger = GetComponent<PlayerDodger>();
            lockOnController = GetComponent<PlayerLockOnController>();
            attackController = GetComponent<PlayerAttackController>();
            skillController = GetComponent<PlayerSkillController>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            playerDodger.UpdateCooldown(deltaTime);

            attackController.UpdateAttack(deltaTime);

            skillController.UpdateSkills(deltaTime);

            UpdateAttackState();

            lockOnController.ValidateCurrentTarget();

            UpdateLockOn();
            UpdateCamera(deltaTime);

            bool isDodging = UpdateDodge(deltaTime);

            UpdateAttackInput();

            UpdateSkillInput();

            UpdateAttackRotation(deltaTime);

            if (!isDodging)
            {
                if (skillController.BlocksMovement)
                {
                    UpdateMovementWithoutInput(
                        deltaTime);
                }
                else
                {
                    UpdateMovement(
                        deltaTime);
                }
            }

            UpdateVisual(isDodging, deltaTime);
        }

        private void UpdateCamera(float deltaTime)
        {
            playerCameraController.RotateCamera(
                inputReader.LookInput,
                inputReader.IsLookInputMouse,
                deltaTime);
        }

        private void UpdateMovement(float deltaTime)
        {
            Transform cameraTarget = playerCameraController.CameraTarget;

            if (cameraTarget == null)
            {
                return;
            }

            if (!stateController.CanMove)
            {
                playerMotor.Move(
                    Vector2.zero,
                    false,
                    cameraTarget,
                    deltaTime);

                return;
            }

            playerMotor.Move(
                inputReader.MoveInput,
                inputReader.IsSprintPressed,
                cameraTarget,
                deltaTime);

            UpdateRotation(deltaTime);
        }

        private void UpdateRotation(float deltaTime)
        {
            if (!stateController.CanRotate)
            {
                return;
            }

            if (lockOnController.IsLockedOn)
            {
                Vector3 targetDirection =
                    lockOnController.CurrentTarget.AimPosition -
                    transform.position;

                targetDirection.y = 0f;

                playerRotator.Rotate(
                    targetDirection,
                    deltaTime);

                return;
            }

            playerRotator.Rotate(
                playerMotor.CurrentMoveDirection,
                deltaTime);
        }

        private void UpdateVisual(
            bool isDodging,
            float deltaTime)
        {
            visualAnimator.UpdateAnimation(
                playerMotor.CurrentHorizontalSpeed,
                playerMotor.IsSprinting,
                isDodging,
                attackController.IsAttacking,
                attackController.CurrentComboStep,
                attackController.NormalizedProgress,
                lockOnController.IsLockedOn,
                playerMotor.CurrentMoveDirection,
                deltaTime);
        }

        private bool UpdateDodge(float deltaTime)
        {
            if (inputReader.WasDodgePressed &&
                playerMotor.IsGrounded &&
                playerDodger.CanDodge)
            {
                if (stateController.TryEnterDodge())
                {
                    bool started =
                        playerDodger.TryStartDodge(
                            inputReader.MoveInput,
                            playerCameraController.CameraTarget);

                    if (!started)
                    {
                        stateController.ExitDodge();
                    }
                    else
                    {
                        playerRotator.SetFacingDirection(
                            playerDodger.DodgeDirection);
                    }
                }
            }

            if (stateController.CurrentState !=
                PlayerState.Dodging)
            {
                return false;
            }

            playerDodger.UpdateDodge(
                deltaTime);

            if (!playerDodger.IsDodging)
            {
                stateController.ExitDodge();
            }

            return true;
        }

        private void UpdateLockOn()
        {
            if (!inputReader.WasLockOnPressed)
            {
                return;
            }

            lockOnController.ToggleLockOn(
                playerCameraController.CameraTarget);
        }

        private void UpdateAttackState()
        {
            if (stateController.CurrentState !=
                PlayerState.Attacking)
            {
                return;
            }

            if (attackController.IsAttacking)
            {
                return;
            }

            stateController.ExitAttack();
        }

        private void UpdateAttackInput()
        {
            if (!inputReader.WasAttackPressed)
            {
                return;
            }

            if (!stateController.CanAttack)
            {
                return;
            }

            if (!playerMotor.IsGrounded)
            {
                return;
            }

            if (stateController.CurrentState ==
                PlayerState.Normal)
            {
                if (!stateController.TryEnterAttack())
                {
                    return;
                }
            }

            attackController.RequestAttack();
        }

        private void UpdateAttackRotation(float deltaTime)
        {
            if (!attackController.CanTrackTarget)
            {
                return;
            }

            if (!lockOnController.IsLockedOn)
            {
                return;
            }

            Vector3 targetDirection =
                lockOnController.CurrentTarget.AimPosition -
                transform.position;

            targetDirection.y = 0f;

            playerRotator.Rotate(
                targetDirection,
                deltaTime);
        }

        private void UpdateSkillInput()
        {
            if (inputReader.WasSkill1Pressed)
            {
                skillController.TryUseSkill(
                    SkillId.DashSlash);
            }

            if (inputReader.WasSkill2Pressed)
            {
                skillController.TryUseSkill(
                    SkillId.Projectile);
            }

            if (inputReader.WasSkill3Pressed)
            {
                skillController.TryUseSkill(
                    SkillId.SpinAttack);
            }

            if (inputReader.WasSkill4Pressed)
            {
                skillController.TryUseSkill(
                    SkillId.AttackBuff);
            }
        }

        private void UpdateMovementWithoutInput(float deltaTime)
        {
            Transform cameraTarget =
                playerCameraController.CameraTarget;

            if (cameraTarget == null)
            {
                return;
            }

            playerMotor.Move(
                Vector2.zero,
                false,
                cameraTarget,
                deltaTime);
        }
    }
}