using UnityEngine;

namespace UnityRPG.Character.Player
{
    public readonly struct PlayerDodgeVisualSettings
    {
        public float BodyDrop { get; }
        public float BodyLean { get; }
        public float HandBack { get; }
        public float FootSpread { get; }

        public PlayerDodgeVisualSettings(float bodyDrop, float bodyLean, float handBack, float footSpread)
        {
            BodyDrop = bodyDrop;
            BodyLean = bodyLean;
            HandBack = handBack;
            FootSpread = footSpread;
        }
    }

    public readonly struct PlayerAttackVisualSettings
    {
        public float HandReach { get; }
        public float HandSide { get; }
        public float BodyTurn { get; }
        public float BodyLean { get; }
        public float LeftHandBack { get; }
        public float ThirdAttackHandLift { get; }

        public PlayerAttackVisualSettings(
            float handReach,
            float handSide,
            float bodyTurn,
            float bodyLean,
            float leftHandBack,
            float thirdAttackHandLift)
        {
            HandReach = handReach;
            HandSide = handSide;
            BodyTurn = bodyTurn;
            BodyLean = bodyLean;
            LeftHandBack = leftHandBack;
            ThirdAttackHandLift = thirdAttackHandLift;
        }
    }

    public sealed class PlayerCombatVisual
    {
        private readonly PlayerVisualPose pose;
        private readonly float transitionSpeed;

        private readonly PlayerDodgeVisualSettings dodgeSettings;
        private readonly PlayerAttackVisualSettings attackSettings;

        public PlayerCombatVisual(
            PlayerVisualPose pose,
            float transitionSpeed,
            PlayerDodgeVisualSettings dodgeSettings,
            PlayerAttackVisualSettings attackSettings)
        {
            this.pose = pose;
            this.transitionSpeed = transitionSpeed;
            this.dodgeSettings = dodgeSettings;
            this.attackSettings = attackSettings;
        }

        public void UpdateDodge(float deltaTime)
        {
            float smoothFactor = GetSmoothFactor(deltaTime);

            Vector3 bodyTarget = pose.BodyBasePosition + Vector3.down * dodgeSettings.BodyDrop;

            Quaternion bodyRotationTarget =
                pose.BodyBaseRotation *
                Quaternion.Euler(dodgeSettings.BodyLean, 0f, 0f);

            Vector3 leftHandTarget = pose.LeftHandBasePosition + Vector3.back * dodgeSettings.HandBack;

            Vector3 rightHandTarget = pose.RightHandBasePosition + Vector3.back * dodgeSettings.HandBack;

            Vector3 leftFootTarget = pose.LeftFootBasePosition + Vector3.forward * dodgeSettings.FootSpread;

            Vector3 rightFootTarget = pose.RightFootBasePosition + Vector3.back * dodgeSettings.FootSpread;

            pose.Body.localPosition = Vector3.Lerp(pose.Body.localPosition, bodyTarget, smoothFactor);

            pose.Body.localRotation = Quaternion.Slerp(pose.Body.localRotation, bodyRotationTarget, smoothFactor);

            pose.LeftHand.localPosition = Vector3.Lerp(pose.LeftHand.localPosition, leftHandTarget, smoothFactor);

            pose.RightHand.localPosition = Vector3.Lerp(pose.RightHand.localPosition, rightHandTarget, smoothFactor);

            pose.LeftFoot.localPosition = Vector3.Lerp(pose.LeftFoot.localPosition, leftFootTarget, smoothFactor);

            pose.RightFoot.localPosition = Vector3.Lerp(pose.RightFoot.localPosition, rightFootTarget, smoothFactor);
        }

        public void UpdateAttack(int comboStep, float progress, float deltaTime)
        {
            float t = Mathf.Clamp01(progress);

            switch (comboStep)
            {
                case 1:
                    UpdateFirstAttack(t, deltaTime);
                    break;

                case 2:
                    UpdateSecondAttack(t, deltaTime);
                    break;

                case 3:
                    UpdateThirdAttack(t, deltaTime);
                    break;
            }
        }

        private void UpdateFirstAttack(float progress, float deltaTime)
        {
            Vector3 windupOffset = new Vector3(attackSettings.HandSide, 0.1f, -attackSettings.HandReach * 0.35f);

            Vector3 strikeOffset = new Vector3(-attackSettings.HandSide, -0.05f, attackSettings.HandReach);

            Vector3 handOffset;
            float bodyYaw;

            if (progress < 0.25f)
            {
                float t = progress / 0.25f;

                handOffset = Vector3.Lerp(Vector3.zero, windupOffset, t);

                bodyYaw = Mathf.Lerp(0f, attackSettings.BodyTurn, t);
            }
            else if (progress < 0.7f)
            {
                float t = (progress - 0.25f) / 0.45f;

                handOffset = Vector3.Lerp(windupOffset, strikeOffset, t);

                bodyYaw = Mathf.Lerp(attackSettings.BodyTurn, -attackSettings.BodyTurn, t);
            }
            else
            {
                float t = (progress - 0.7f) / 0.3f;

                handOffset = Vector3.Lerp(strikeOffset, Vector3.zero, t);

                bodyYaw = Mathf.Lerp(-attackSettings.BodyTurn, 0f, t);
            }

            Vector3 rightHandTarget = pose.RightHandBasePosition + handOffset;

            Vector3 leftHandTarget = pose.LeftHandBasePosition + Vector3.back * attackSettings.LeftHandBack;

            Quaternion bodyTargetRotation = pose.BodyBaseRotation * Quaternion.Euler(attackSettings.BodyLean, bodyYaw, 0f);

            ApplyAttackPose(rightHandTarget, leftHandTarget, bodyTargetRotation, deltaTime);
        }

        private void UpdateSecondAttack(float progress, float deltaTime)
        {
            Vector3 windupOffset = new Vector3(-attackSettings.HandSide, -0.05f, -attackSettings.HandReach * 0.25f);

            Vector3 strikeOffset = new Vector3(attackSettings.HandSide, 0.15f, attackSettings.HandReach);

            Vector3 handOffset;
            float bodyYaw;

            if (progress < 0.25f)
            {
                float t = progress / 0.25f;

                handOffset = Vector3.Lerp(Vector3.zero, windupOffset, t);

                bodyYaw = Mathf.Lerp(0f, -attackSettings.BodyTurn, t);
            }
            else if (progress < 0.7f)
            {
                float t = (progress - 0.25f) / 0.45f;

                handOffset = Vector3.Lerp(windupOffset, strikeOffset, t);

                bodyYaw = Mathf.Lerp(-attackSettings.BodyTurn, attackSettings.BodyTurn, t);
            }
            else
            {
                float t = (progress - 0.7f) / 0.3f;

                handOffset = Vector3.Lerp(strikeOffset, Vector3.zero, t);

                bodyYaw = Mathf.Lerp(attackSettings.BodyTurn, 0f, t);
            }

            Vector3 rightHandTarget = pose.RightHandBasePosition + handOffset;

            Vector3 leftHandTarget = pose.LeftHandBasePosition + Vector3.back * attackSettings.LeftHandBack;

            Quaternion bodyTargetRotation = pose.BodyBaseRotation * Quaternion.Euler(attackSettings.BodyLean, bodyYaw, 0f);

            ApplyAttackPose(rightHandTarget, leftHandTarget, bodyTargetRotation, deltaTime);
        }

        private void UpdateThirdAttack(float progress, float deltaTime)
        {
            Vector3 windupOffset = new Vector3(0f, attackSettings.ThirdAttackHandLift, -attackSettings.HandReach * 0.45f);

            Vector3 strikeOffset = new Vector3(0f, -0.3f, attackSettings.HandReach * 1.15f);

            Vector3 handOffset;
            float bodyPitch;

            if (progress < 0.35f)
            {
                float t = progress / 0.35f;

                handOffset = Vector3.Lerp(Vector3.zero, windupOffset, t);

                bodyPitch = Mathf.Lerp(0f, -attackSettings.BodyLean, t);
            }
            else if (progress < 0.7f)
            {
                float t = (progress - 0.35f) / 0.35f;

                handOffset = Vector3.Lerp(windupOffset, strikeOffset, t);

                bodyPitch = Mathf.Lerp(-attackSettings.BodyLean, attackSettings.BodyLean * 1.5f, t);
            }
            else
            {
                float t = (progress - 0.7f) / 0.3f;

                handOffset = Vector3.Lerp(strikeOffset, Vector3.zero, t);

                bodyPitch = Mathf.Lerp(attackSettings.BodyLean * 1.5f, 0f, t);
            }

            Vector3 rightHandTarget = pose.RightHandBasePosition + handOffset;

            Vector3 leftHandTarget = pose.LeftHandBasePosition + Vector3.back * attackSettings.LeftHandBack;

            Quaternion bodyTargetRotation = pose.BodyBaseRotation * Quaternion.Euler(bodyPitch, 0f, 0f);

            ApplyAttackPose(rightHandTarget, leftHandTarget, bodyTargetRotation, deltaTime);
        }

        private void ApplyAttackPose(Vector3 rightHandTarget, Vector3 leftHandTarget, Quaternion bodyTargetRotation, float deltaTime)
        {
            float smoothFactor = GetSmoothFactor(deltaTime);

            pose.Body.localPosition = Vector3.Lerp(pose.Body.localPosition, pose.BodyBasePosition, smoothFactor);

            pose.Body.localRotation = Quaternion.Slerp(pose.Body.localRotation, bodyTargetRotation, smoothFactor);

            pose.RightHand.localPosition = Vector3.Lerp(pose.RightHand.localPosition, rightHandTarget, smoothFactor);

            pose.LeftHand.localPosition = Vector3.Lerp(pose.LeftHand.localPosition, leftHandTarget, smoothFactor);

            pose.LeftFoot.localPosition = Vector3.Lerp(pose.LeftFoot.localPosition, pose.LeftFootBasePosition, smoothFactor);

            pose.RightFoot.localPosition = Vector3.Lerp(pose.RightFoot.localPosition, pose.RightFootBasePosition, smoothFactor);
        }

        private float GetSmoothFactor(float deltaTime)
        {
            return 1f - Mathf.Exp(-transitionSpeed * deltaTime);
        }
    }
}