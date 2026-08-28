using UnityEngine;
using UnityRPG.Combat;
using UnityRPG.Skill;
using UnityRPG.Interaction;

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
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerInteractor))]
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
        private PlayerHealth playerHealth;
        private PlayerInteractor playerInteractor;

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
            playerHealth = GetComponent<PlayerHealth>();
            playerInteractor = GetComponent<PlayerInteractor>();

            playerHealth.Died += HandleDied;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            if (stateController.CurrentState == PlayerState.Dead)
            {
                UpdateCamera(deltaTime);
                return;
            }

            playerDodger.UpdateCooldown(deltaTime);
            attackController.UpdateAttack(deltaTime);
            skillController.UpdateSkills(deltaTime);

            UpdateSkillState();
            UpdateAttackState();

            lockOnController.ValidateCurrentTarget();

            UpdateLockOn();
            UpdateCamera(deltaTime);

            bool isDodging = UpdateDodge(deltaTime);

            UpdateSkillInput();
            UpdateAttackInput();
            UpdateInteractionInput();

            UpdateAttackRotation(deltaTime);

            if (!isDodging)
            {
                UpdateMovement(deltaTime);
            }

            UpdateVisual(isDodging, deltaTime);
        }

        private void UpdateCamera(float deltaTime)
        {
            playerCameraController.RotateCamera(inputReader.LookInput, inputReader.IsLookInputMouse, deltaTime);
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
                playerMotor.Move(Vector2.zero, false, cameraTarget, deltaTime);

                return;
            }

            playerMotor.Move(inputReader.MoveInput, inputReader.IsSprintPressed, cameraTarget, deltaTime);

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
                Vector3 targetDirection = lockOnController.CurrentTarget.AimPosition - transform.position;

                targetDirection.y = 0f;

                playerRotator.Rotate(targetDirection, deltaTime);

                return;
            }

            playerRotator.Rotate(playerMotor.CurrentMoveDirection, deltaTime);
        }

        private void UpdateVisual(bool isDodging, float deltaTime)
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
                deltaTime,
                skillController.IsUsingSkill,
                skillController.CurrentSkillId,
                skillController.ActionNormalizedProgress,
                skillController.IsAttackBuffActive);
        }

        private bool UpdateDodge(float deltaTime)
        {
            if (inputReader.WasDodgePressed && playerMotor.IsGrounded && playerDodger.CanDodge)
            {
                if (stateController.TryEnterDodge())
                {
                    bool started = playerDodger.TryStartDodge(inputReader.MoveInput, playerCameraController.CameraTarget);

                    if (!started)
                    {
                        stateController.ExitDodge();
                    }
                    else
                    {
                        playerRotator.SetFacingDirection(playerDodger.DodgeDirection);
                    }
                }
            }

            if (stateController.CurrentState != PlayerState.Dodging)
            {
                return false;
            }

            playerDodger.UpdateDodge(deltaTime);

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

            lockOnController.ToggleLockOn(playerCameraController.CameraTarget);
        }

        private void UpdateAttackState()
        {
            if (stateController.CurrentState != PlayerState.Attacking)
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

            if (stateController.CurrentState == PlayerState.Normal)
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

            Vector3 targetDirection = lockOnController.CurrentTarget.AimPosition - transform.position;

            targetDirection.y = 0f;

            playerRotator.Rotate(targetDirection, deltaTime);
        }

        private void UpdateSkillInput()
        {
            if (inputReader.WasSkill1Pressed && TryUseSkill(SkillId.DashSlash))
            {
                return;
            }

            if (inputReader.WasSkill2Pressed && TryUseSkill(SkillId.Projectile))
            {
                return;
            }

            if (inputReader.WasSkill3Pressed && TryUseSkill(SkillId.SpinAttack))
            {
                return;
            }

            if (inputReader.WasSkill4Pressed)
            {
                TryUseSkill(SkillId.AttackBuff);
            }
        }

        private void UpdateSkillState()
        {
            if (stateController.CurrentState != PlayerState.UsingSkill)
            {
                return;
            }

            if (skillController.IsUsingSkill)
            {
                return;
            }

            stateController.ExitSkill();
        }

        private bool TryUseSkill(SkillId skillId)
        {
            RuntimeSkill runtimeSkill = skillController.GetSkill(skillId);

            if (runtimeSkill == null || !runtimeSkill.IsReady)
            {
                return false;
            }

            if (!stateController.TryEnterSkill())
            {
                return false;
            }

            UpdateSkillFacing(skillId);

            if (!skillController.TryUseSkill(skillId))
            {
                stateController.ExitSkill();

                return false;
            }

            return true;
        }

        private void UpdateSkillFacing(SkillId skillId)
        {
            if (skillId != SkillId.DashSlash && skillId != SkillId.Projectile)
            {
                return;
            }

            var target = lockOnController.CurrentTarget;

            if (target == null)
            {
                return;
            }

            Vector3 direction = target.AimPosition - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            playerRotator.SetFacingDirection(direction.normalized);
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.Died -= HandleDied;
            }
        }

        private void HandleDied()
        {
            stateController.EnterDead();
            visualAnimator.PlayDeath();
        }

        private void UpdateInteractionInput()
        {
            if (!inputReader.WasInteractPressed || stateController.CurrentState != PlayerState.Normal)
                return;

            playerInteractor.TryInteract();
        }
    }
}