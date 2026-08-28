using UnityEngine;
using UnityRPG.Skill;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerSkillVisual
    {
        private readonly PlayerVisualPose pose;
        private readonly float transitionSpeed;

        public PlayerSkillVisual(PlayerVisualPose pose, float transitionSpeed)
        {
            this.pose = pose;
            this.transitionSpeed = transitionSpeed;
        }

        public void UpdateAttackBuffVisual(bool isActive)
        {
            if (!isActive)
            {
                return;
            }

            float pulse = 1f + Mathf.Sin(Time.time * 8f) * 0.03f;

            pose.ModelRoot.localScale = pose.ModelRootBaseScale * pulse;
        }

        public void UpdateSkillAnimation(SkillId skillId, float progress, float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            switch (skillId)
            {
                case SkillId.DashSlash:
                    UpdateDashSlashAnimation(progress, deltaTime);
                    break;

                case SkillId.Projectile:
                    UpdateProjectileSkillAnimation(progress, deltaTime);
                    break;

                case SkillId.SpinAttack:
                    UpdateSpinAttackAnimation(progress, deltaTime);
                    break;

                case SkillId.AttackBuff:
                    UpdateAttackBuffCastAnimation(progress, deltaTime);
                    break;
            }
        }

        private void UpdateDashSlashAnimation(float progress, float deltaTime)
        {
            float weight = Mathf.Sin(progress * Mathf.PI);

            Vector3 bodyTarget = pose.BodyBasePosition + new Vector3(0f, -0.07f, 0.16f) * weight;
            Quaternion bodyRotationTarget = pose.BodyBaseRotation * Quaternion.Euler(22f * weight, 0f, 0f);

            Vector3 rightHandTarget = pose.RightHandBasePosition + new Vector3(0.15f, 0.05f, 0.6f) * weight;
            Vector3 leftHandTarget = pose.LeftHandBasePosition + new Vector3(-0.1f, 0f, -0.24f) * weight;

            ApplySkillPose(bodyTarget, bodyRotationTarget, leftHandTarget, rightHandTarget, deltaTime);
        }

        private void UpdateProjectileSkillAnimation(float progress, float deltaTime)
        {
            float weight = Mathf.Sin(progress * Mathf.PI);

            Vector3 bodyTarget = pose.BodyBasePosition + Vector3.forward * 0.08f * weight;
            Quaternion bodyRotationTarget = pose.BodyBaseRotation * Quaternion.Euler(10f * weight, 0f, 0f);

            Vector3 rightHandTarget = pose.RightHandBasePosition + new Vector3(0f, 0.1f, 0.62f) * weight;
            Vector3 leftHandTarget = pose.LeftHandBasePosition + new Vector3(0f, 0.05f, -0.18f) * weight;

            ApplySkillPose(bodyTarget, bodyRotationTarget, leftHandTarget, rightHandTarget, deltaTime);
        }

        private void UpdateSpinAttackAnimation(float progress, float deltaTime)
        {
            float weight = Mathf.Sin(progress * Mathf.PI);

            pose.ModelRoot.localRotation = pose.ModelRootBaseRotation * Quaternion.Euler(0f, 360f * progress, 0f);

            Vector3 bodyTarget = pose.BodyBasePosition + Vector3.down * 0.05f * weight;
            Vector3 rightHandTarget = pose.RightHandBasePosition + Vector3.right * 0.45f * weight;
            Vector3 leftHandTarget = pose.LeftHandBasePosition + Vector3.left * 0.45f * weight;

            ApplySkillPose(bodyTarget, pose.BodyBaseRotation, leftHandTarget, rightHandTarget, deltaTime);
        }

        private void UpdateAttackBuffCastAnimation(float progress, float deltaTime)
        {
            float weight = Mathf.Sin(progress * Mathf.PI);

            Vector3 bodyTarget = pose.BodyBasePosition + Vector3.up * 0.08f * weight;

            Vector3 rightHandTarget = pose.RightHandBasePosition + new Vector3(0.14f, 0.45f, 0f) * weight;

            Vector3 leftHandTarget = pose.LeftHandBasePosition + new Vector3(-0.14f, 0.45f, 0f) * weight;

            ApplySkillPose(bodyTarget, pose.BodyBaseRotation, leftHandTarget, rightHandTarget, deltaTime);
        }

        private void ApplySkillPose(Vector3 bodyTarget, Quaternion bodyRotationTarget, Vector3 leftHandTarget, Vector3 rightHandTarget, float deltaTime)
        {
            float smoothFactor = GetSmoothFactor(deltaTime);

            pose.Body.localPosition = Vector3.Lerp(pose.Body.localPosition, bodyTarget, smoothFactor);

            pose.Body.localRotation = Quaternion.Slerp(pose.Body.localRotation, bodyRotationTarget, smoothFactor);

            pose.LeftHand.localPosition = Vector3.Lerp(pose.LeftHand.localPosition, leftHandTarget, smoothFactor);

            pose.RightHand.localPosition = Vector3.Lerp(pose.RightHand.localPosition, rightHandTarget, smoothFactor);

            pose.LeftFoot.localPosition = Vector3.Lerp(pose.LeftFoot.localPosition, pose.LeftFootBasePosition, smoothFactor);

            pose.RightFoot.localPosition = Vector3.Lerp(pose.RightFoot.localPosition, pose.RightFootBasePosition, smoothFactor);
        }

        private float GetSmoothFactor(float deltaTime)
        {
            return 1f - Mathf.Exp(-transitionSpeed * deltaTime);
        }
    }
}