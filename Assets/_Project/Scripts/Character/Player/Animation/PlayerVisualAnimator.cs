using UnityEngine;
using UnityRPG.Skill;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerVisualAnimator : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField]
        private Transform body;

        [SerializeField]
        private Transform leftHand;

        [SerializeField]
        private Transform rightHand;

        [SerializeField]
        private Transform leftFoot;

        [SerializeField]
        private Transform rightFoot;

        [Header("Reference")]
        [SerializeField]
        private Transform movementReference;

        [Header("State")]
        [SerializeField]
        [Min(0f)]
        private float movingSpeedThreshold = 0.15f;

        [SerializeField]
        [Min(0f)]
        private float runSpeedThreshold = 6f;

        [Header("Idle")]
        [SerializeField]
        [Min(0f)]
        private float idleCycleSpeed = 2f;

        [SerializeField]
        [Min(0f)]
        private float idleBodyBob = 0.025f;

        [SerializeField]
        [Min(0f)]
        private float idleHandBob = 0.015f;

        [Header("Walk")]
        [SerializeField]
        [Min(0f)]
        private float walkCycleSpeed = 7f;

        [SerializeField]
        [Min(0f)]
        private float walkStepDistance = 0.16f;

        [SerializeField]
        [Min(0f)]
        private float walkFootLift = 0.08f;

        [SerializeField]
        [Min(0f)]
        private float walkHandSwing = 0.13f;

        [SerializeField]
        [Min(0f)]
        private float walkBodyBob = 0.035f;

        [SerializeField]
        private float walkBodyLean = 2f;

        [Header("Run")]
        [SerializeField]
        [Min(0f)]
        private float runCycleSpeed = 10f;

        [SerializeField]
        [Min(0f)]
        private float runStepDistance = 0.28f;

        [SerializeField]
        [Min(0f)]
        private float runFootLift = 0.13f;

        [SerializeField]
        [Min(0f)]
        private float runHandSwing = 0.23f;

        [SerializeField]
        [Min(0f)]
        private float runBodyBob = 0.065f;

        [SerializeField]
        private float runBodyLean = 8f;

        [Header("Transition")]
        [SerializeField]
        [Min(0f)]
        private float transitionSpeed = 12f;

        [Header("Dodge")]
        [SerializeField]
        [Min(0f)]
        private float dodgeBodyDrop = 0.12f;

        [SerializeField]
        private float dodgeBodyLean = 20f;

        [SerializeField]
        [Min(0f)]
        private float dodgeHandBack = 0.18f;

        [SerializeField]
        [Min(0f)]
        private float dodgeFootSpread = 0.12f;

        [Header("Lock-On")]
        [SerializeField]
        [Min(0f)]
        private float lockOnBodyDrop = 0.04f;

        [SerializeField]
        [Min(0f)]
        private float lockOnStepDistance = 0.14f;

        [SerializeField]
        [Min(0f)]
        private float lockOnSideStepDistance = 0.16f;

        [SerializeField]
        [Min(0f)]
        private float lockOnFootLift = 0.06f;

        [SerializeField]
        [Min(0f)]
        private float lockOnHandSwing = 0.08f;

        [SerializeField]
        [Min(0f)]
        private float lockOnBodyBob = 0.025f;

        [SerializeField]
        [Min(0f)]
        private float lockOnIdleHandForward = 0.06f;

        [Header("Attack")]
        [SerializeField]
        [Min(0f)]
        private float attackHandReach = 0.65f;

        [SerializeField]
        [Min(0f)]
        private float attackHandSide = 0.75f;

        [SerializeField]
        [Min(0f)]
        private float attackBodyTurn = 30f;

        [SerializeField]
        [Min(0f)]
        private float attackBodyLean = 10f;

        [SerializeField]
        [Min(0f)]
        private float attackLeftHandBack = 0.18f;

        [SerializeField]
        [Min(0f)]
        private float thirdAttackHandLift = 0.8f;

        [Header("Skill")]
        [SerializeField]
        private Transform modelRoot;

        [Header("Death")]
        [SerializeField]
        [Min(0.01f)]
        private float deathDuration = 0.5f;

        private PlayerVisualPose pose;
        private PlayerLocomotionVisual locomotionVisual;
        private PlayerCombatVisual combatVisual;
        private PlayerSkillVisual skillVisual;
        private PlayerDeathVisual deathVisual;

        private bool isConfigured;
        private bool isDead;

        private void Awake()
        {
            if (!ValidateParts())
            {
                return;
            }

            pose = new PlayerVisualPose(
                modelRoot,
                body,
                leftHand,
                rightHand,
                leftFoot,
                rightFoot);

            CreateVisualModules();

            isConfigured = true;
        }

        private void Update()
        {
            if (!isConfigured || deathVisual == null || !deathVisual.IsPlaying)
            {
                return;
            }

            deathVisual.UpdateDeath(Time.deltaTime);
        }

        private bool ValidateParts()
        {
            if (movementReference == null ||
                modelRoot == null ||
                body == null ||
                leftHand == null ||
                rightHand == null ||
                leftFoot == null ||
                rightFoot == null)
            {
                Debug.LogError(
                    "[Player] PlayerVisualAnimator의 캐릭터 파츠 또는 이동 기준 참조가 누락되었습니다.",
                    this);

                return false;
            }

            return true;
        }

        private void CreateVisualModules()
        {
            PlayerIdleVisualSettings idleSettings = new PlayerIdleVisualSettings(
                idleCycleSpeed,
                idleBodyBob,
                idleHandBob);

            PlayerMovementVisualSettings walkSettings = new PlayerMovementVisualSettings(
                walkCycleSpeed,
                walkStepDistance,
                walkFootLift,
                walkHandSwing,
                walkBodyBob,
                walkBodyLean);

            PlayerMovementVisualSettings runSettings = new PlayerMovementVisualSettings(
                runCycleSpeed,
                runStepDistance,
                runFootLift,
                runHandSwing,
                runBodyBob,
                runBodyLean);

            PlayerLockOnVisualSettings lockOnSettings = new PlayerLockOnVisualSettings(
                lockOnBodyDrop,
                lockOnStepDistance,
                lockOnSideStepDistance,
                lockOnFootLift,
                lockOnHandSwing,
                lockOnBodyBob,
                lockOnIdleHandForward);

            PlayerDodgeVisualSettings dodgeSettings = new PlayerDodgeVisualSettings(
                dodgeBodyDrop,
                dodgeBodyLean,
                dodgeHandBack,
                dodgeFootSpread);

            PlayerAttackVisualSettings attackSettings = new PlayerAttackVisualSettings(
                attackHandReach,
                attackHandSide,
                attackBodyTurn,
                attackBodyLean,
                attackLeftHandBack,
                thirdAttackHandLift);

            locomotionVisual = new PlayerLocomotionVisual(
                pose,
                movementReference,
                movingSpeedThreshold,
                runSpeedThreshold,
                transitionSpeed,
                idleSettings,
                walkSettings,
                runSettings,
                lockOnSettings);

            combatVisual = new PlayerCombatVisual(
                pose,
                transitionSpeed,
                dodgeSettings,
                attackSettings);

            skillVisual = new PlayerSkillVisual(
                pose,
                transitionSpeed);

            deathVisual = new PlayerDeathVisual(
                pose,
                deathDuration);
        }

        public void UpdateAnimation(
            float horizontalSpeed,
            bool isSprinting,
            bool isDodging,
            bool isAttacking,
            int comboStep,
            float attackProgress,
            bool isLockedOn,
            Vector3 moveDirection,
            float deltaTime,
            bool isUsingSkill,
            SkillId currentSkillId,
            float skillProgress,
            bool isAttackBuffActive)
        {
            if (!isConfigured || isDead)
            {
                return;
            }

            pose.ResetModelRoot();

            skillVisual.UpdateAttackBuffVisual(isAttackBuffActive);

            if (isDodging)
            {
                combatVisual.UpdateDodge(deltaTime);
                return;
            }

            if (isUsingSkill)
            {
                skillVisual.UpdateSkillAnimation(
                    currentSkillId,
                    skillProgress,
                    deltaTime);

                return;
            }

            if (isAttacking)
            {
                combatVisual.UpdateAttack(
                    comboStep,
                    attackProgress,
                    deltaTime);

                return;
            }

            locomotionVisual.UpdateAnimation(
                horizontalSpeed,
                isSprinting,
                isLockedOn,
                moveDirection,
                deltaTime);
        }

        public void PlayDeath()
        {
            if (!isConfigured || isDead)
            {
                return;
            }

            isDead = true;
            deathVisual.BeginDeath();
        }
    }
}