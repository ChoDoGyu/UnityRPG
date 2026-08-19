using UnityEngine;

namespace UnityRPG.Character.Player
{
    public readonly struct PlayerIdleVisualSettings
    {
        public float CycleSpeed { get; }
        public float BodyBob { get; }
        public float HandBob { get; }

        public PlayerIdleVisualSettings(
            float cycleSpeed,
            float bodyBob,
            float handBob)
        {
            CycleSpeed = cycleSpeed;
            BodyBob = bodyBob;
            HandBob = handBob;
        }
    }

    public readonly struct PlayerMovementVisualSettings
    {
        public float CycleSpeed { get; }
        public float StepDistance { get; }
        public float FootLift { get; }
        public float HandSwing { get; }
        public float BodyBob { get; }
        public float BodyLean { get; }

        public PlayerMovementVisualSettings(
            float cycleSpeed,
            float stepDistance,
            float footLift,
            float handSwing,
            float bodyBob,
            float bodyLean)
        {
            CycleSpeed = cycleSpeed;
            StepDistance = stepDistance;
            FootLift = footLift;
            HandSwing = handSwing;
            BodyBob = bodyBob;
            BodyLean = bodyLean;
        }
    }

    public readonly struct PlayerLockOnVisualSettings
    {
        public float BodyDrop { get; }
        public float StepDistance { get; }
        public float SideStepDistance { get; }
        public float FootLift { get; }
        public float HandSwing { get; }
        public float BodyBob { get; }
        public float IdleHandForward { get; }

        public PlayerLockOnVisualSettings(
            float bodyDrop,
            float stepDistance,
            float sideStepDistance,
            float footLift,
            float handSwing,
            float bodyBob,
            float idleHandForward)
        {
            BodyDrop = bodyDrop;
            StepDistance = stepDistance;
            SideStepDistance = sideStepDistance;
            FootLift = footLift;
            HandSwing = handSwing;
            BodyBob = bodyBob;
            IdleHandForward = idleHandForward;
        }
    }

    public sealed class PlayerLocomotionVisual
    {
        private readonly PlayerVisualPose pose;
        private readonly Transform movementReference;

        private readonly float movingSpeedThreshold;
        private readonly float runSpeedThreshold;
        private readonly float transitionSpeed;

        private readonly PlayerIdleVisualSettings idleSettings;
        private readonly PlayerMovementVisualSettings walkSettings;
        private readonly PlayerMovementVisualSettings runSettings;
        private readonly PlayerLockOnVisualSettings lockOnSettings;

        private float cycle;

        public PlayerLocomotionVisual(
            PlayerVisualPose pose,
            Transform movementReference,
            float movingSpeedThreshold,
            float runSpeedThreshold,
            float transitionSpeed,
            PlayerIdleVisualSettings idleSettings,
            PlayerMovementVisualSettings walkSettings,
            PlayerMovementVisualSettings runSettings,
            PlayerLockOnVisualSettings lockOnSettings)
        {
            this.pose = pose;
            this.movementReference = movementReference;
            this.movingSpeedThreshold = movingSpeedThreshold;
            this.runSpeedThreshold = runSpeedThreshold;
            this.transitionSpeed = transitionSpeed;
            this.idleSettings = idleSettings;
            this.walkSettings = walkSettings;
            this.runSettings = runSettings;
            this.lockOnSettings = lockOnSettings;
        }

        public void UpdateAnimation(
            float horizontalSpeed,
            bool isSprinting,
            bool isLockedOn,
            Vector3 moveDirection,
            float deltaTime)
        {
            bool isMoving = horizontalSpeed > movingSpeedThreshold;

            bool isRunning =
                isMoving &&
                isSprinting &&
                horizontalSpeed > runSpeedThreshold;

            float cycleSpeed;

            if (!isMoving)
            {
                cycleSpeed = idleSettings.CycleSpeed;
            }
            else
            {
                cycleSpeed = isRunning
                    ? runSettings.CycleSpeed
                    : walkSettings.CycleSpeed;
            }

            cycle += cycleSpeed * deltaTime;

            if (isLockedOn)
            {
                UpdateLockOn(
                    moveDirection,
                    isMoving,
                    deltaTime);

                return;
            }

            if (!isMoving)
            {
                UpdateIdle(deltaTime);
                return;
            }

            UpdateMovement(
                isRunning,
                deltaTime);
        }

        private void UpdateIdle(float deltaTime)
        {
            float bodyBob =
                Mathf.Sin(cycle) *
                idleSettings.BodyBob;

            float handBob =
                Mathf.Sin(cycle) *
                idleSettings.HandBob;

            float smoothFactor = GetSmoothFactor(deltaTime);

            Vector3 bodyTarget =
                pose.BodyBasePosition +
                Vector3.up * bodyBob;

            Vector3 leftHandTarget =
                pose.LeftHandBasePosition +
                Vector3.up * handBob;

            Vector3 rightHandTarget =
                pose.RightHandBasePosition +
                Vector3.up * handBob;

            pose.Body.localPosition = Vector3.Lerp(
                pose.Body.localPosition,
                bodyTarget,
                smoothFactor);

            pose.Body.localRotation = Quaternion.Slerp(
                pose.Body.localRotation,
                pose.BodyBaseRotation,
                smoothFactor);

            pose.LeftHand.localPosition = Vector3.Lerp(
                pose.LeftHand.localPosition,
                leftHandTarget,
                smoothFactor);

            pose.RightHand.localPosition = Vector3.Lerp(
                pose.RightHand.localPosition,
                rightHandTarget,
                smoothFactor);

            pose.LeftFoot.localPosition = Vector3.Lerp(
                pose.LeftFoot.localPosition,
                pose.LeftFootBasePosition,
                smoothFactor);

            pose.RightFoot.localPosition = Vector3.Lerp(
                pose.RightFoot.localPosition,
                pose.RightFootBasePosition,
                smoothFactor);
        }

        private void UpdateMovement(
            bool isRunning,
            float deltaTime)
        {
            PlayerMovementVisualSettings settings =
                isRunning
                    ? runSettings
                    : walkSettings;

            float swing = Mathf.Sin(cycle);

            float leftLift =
                Mathf.Max(0f, swing) *
                settings.FootLift;

            float rightLift =
                Mathf.Max(0f, -swing) *
                settings.FootLift;

            float bodyBob =
                (Mathf.Abs(Mathf.Cos(cycle)) - 0.5f) *
                2f *
                settings.BodyBob;

            Vector3 leftFootTarget =
                pose.LeftFootBasePosition +
                new Vector3(
                    0f,
                    leftLift,
                    swing * settings.StepDistance);

            Vector3 rightFootTarget =
                pose.RightFootBasePosition +
                new Vector3(
                    0f,
                    rightLift,
                    -swing * settings.StepDistance);

            Vector3 leftHandTarget =
                pose.LeftHandBasePosition +
                Vector3.forward *
                (-swing * settings.HandSwing);

            Vector3 rightHandTarget =
                pose.RightHandBasePosition +
                Vector3.forward *
                (swing * settings.HandSwing);

            Vector3 bodyTarget =
                pose.BodyBasePosition +
                Vector3.up * bodyBob;

            Quaternion bodyRotationTarget =
                pose.BodyBaseRotation *
                Quaternion.Euler(
                    settings.BodyLean,
                    0f,
                    0f);

            float smoothFactor = GetSmoothFactor(deltaTime);

            pose.Body.localPosition = Vector3.Lerp(
                pose.Body.localPosition,
                bodyTarget,
                smoothFactor);

            pose.Body.localRotation = Quaternion.Slerp(
                pose.Body.localRotation,
                bodyRotationTarget,
                smoothFactor);

            pose.LeftHand.localPosition = Vector3.Lerp(
                pose.LeftHand.localPosition,
                leftHandTarget,
                smoothFactor);

            pose.RightHand.localPosition = Vector3.Lerp(
                pose.RightHand.localPosition,
                rightHandTarget,
                smoothFactor);

            pose.LeftFoot.localPosition = Vector3.Lerp(
                pose.LeftFoot.localPosition,
                leftFootTarget,
                smoothFactor);

            pose.RightFoot.localPosition = Vector3.Lerp(
                pose.RightFoot.localPosition,
                rightFootTarget,
                smoothFactor);
        }

        private void UpdateLockOn(
            Vector3 worldMoveDirection,
            bool isMoving,
            float deltaTime)
        {
            if (!isMoving)
            {
                UpdateLockOnIdle(deltaTime);
                return;
            }

            Vector3 localDirection =
                movementReference.InverseTransformDirection(
                    worldMoveDirection);

            localDirection.y = 0f;

            if (localDirection.sqrMagnitude > 0.001f)
            {
                localDirection.Normalize();
            }

            float swing = Mathf.Sin(cycle);

            float leftLift =
                Mathf.Max(0f, swing) *
                lockOnSettings.FootLift;

            float rightLift =
                Mathf.Max(0f, -swing) *
                lockOnSettings.FootLift;

            float forwardOffset =
                swing *
                lockOnSettings.StepDistance *
                localDirection.z;

            float sideOffset =
                swing *
                lockOnSettings.SideStepDistance *
                localDirection.x;

            Vector3 leftFootTarget =
                pose.LeftFootBasePosition +
                new Vector3(
                    sideOffset,
                    leftLift,
                    forwardOffset);

            Vector3 rightFootTarget =
                pose.RightFootBasePosition +
                new Vector3(
                    -sideOffset,
                    rightLift,
                    -forwardOffset);

            Vector3 handSwing =
                new Vector3(
                    -sideOffset,
                    0f,
                    -forwardOffset).normalized *
                lockOnSettings.HandSwing *
                Mathf.Abs(swing);

            Vector3 leftHandTarget =
                pose.LeftHandBasePosition +
                handSwing;

            Vector3 rightHandTarget =
                pose.RightHandBasePosition -
                handSwing;

            float bodyBob =
                (Mathf.Abs(Mathf.Cos(cycle)) - 0.5f) *
                2f *
                lockOnSettings.BodyBob;

            Vector3 bodyTarget =
                pose.BodyBasePosition +
                Vector3.down * lockOnSettings.BodyDrop +
                Vector3.up * bodyBob;

            float smoothFactor = GetSmoothFactor(deltaTime);

            pose.Body.localPosition = Vector3.Lerp(
                pose.Body.localPosition,
                bodyTarget,
                smoothFactor);

            pose.Body.localRotation = Quaternion.Slerp(
                pose.Body.localRotation,
                pose.BodyBaseRotation,
                smoothFactor);

            pose.LeftHand.localPosition = Vector3.Lerp(
                pose.LeftHand.localPosition,
                leftHandTarget,
                smoothFactor);

            pose.RightHand.localPosition = Vector3.Lerp(
                pose.RightHand.localPosition,
                rightHandTarget,
                smoothFactor);

            pose.LeftFoot.localPosition = Vector3.Lerp(
                pose.LeftFoot.localPosition,
                leftFootTarget,
                smoothFactor);

            pose.RightFoot.localPosition = Vector3.Lerp(
                pose.RightFoot.localPosition,
                rightFootTarget,
                smoothFactor);
        }

        private void UpdateLockOnIdle(float deltaTime)
        {
            float idleBob =
                Mathf.Sin(cycle) *
                idleSettings.BodyBob;

            Vector3 bodyTarget =
                pose.BodyBasePosition +
                Vector3.down * lockOnSettings.BodyDrop +
                Vector3.up * idleBob;

            Vector3 leftHandTarget =
                pose.LeftHandBasePosition +
                Vector3.forward *
                lockOnSettings.IdleHandForward;

            Vector3 rightHandTarget =
                pose.RightHandBasePosition +
                Vector3.forward *
                lockOnSettings.IdleHandForward;

            float smoothFactor = GetSmoothFactor(deltaTime);

            pose.Body.localPosition = Vector3.Lerp(
                pose.Body.localPosition,
                bodyTarget,
                smoothFactor);

            pose.Body.localRotation = Quaternion.Slerp(
                pose.Body.localRotation,
                pose.BodyBaseRotation,
                smoothFactor);

            pose.LeftHand.localPosition = Vector3.Lerp(
                pose.LeftHand.localPosition,
                leftHandTarget,
                smoothFactor);

            pose.RightHand.localPosition = Vector3.Lerp(
                pose.RightHand.localPosition,
                rightHandTarget,
                smoothFactor);

            pose.LeftFoot.localPosition = Vector3.Lerp(
                pose.LeftFoot.localPosition,
                pose.LeftFootBasePosition,
                smoothFactor);

            pose.RightFoot.localPosition = Vector3.Lerp(
                pose.RightFoot.localPosition,
                pose.RightFootBasePosition,
                smoothFactor);
        }

        private float GetSmoothFactor(float deltaTime)
        {
            return 1f - Mathf.Exp(
                -transitionSpeed * deltaTime);
        }
    }
}