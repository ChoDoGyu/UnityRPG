using UnityEngine;

namespace UnityRPG.AI
{
    public sealed class EliteSlamVisual
    {
        private readonly EnemyVisualPose pose;
        private readonly float transitionSpeed;

        private readonly float windupBack;
        private readonly float windupUp;
        private readonly float windupPitch;
        private readonly float windupScale;

        private readonly float recoveryForward;
        private readonly float recoveryDown;
        private readonly float recoveryPitch;
        private readonly float impactWidthScale;
        private readonly float impactHeightScale;

        public EliteSlamVisual(
            EnemyVisualPose pose,
            float transitionSpeed,
            float windupBack,
            float windupUp,
            float windupPitch,
            float windupScale,
            float recoveryForward,
            float recoveryDown,
            float recoveryPitch,
            float impactWidthScale,
            float impactHeightScale)
        {
            this.pose = pose;
            this.transitionSpeed = transitionSpeed;
            this.windupBack = windupBack;
            this.windupUp = windupUp;
            this.windupPitch = windupPitch;
            this.windupScale = windupScale;
            this.recoveryForward = recoveryForward;
            this.recoveryDown = recoveryDown;
            this.recoveryPitch = recoveryPitch;
            this.impactWidthScale = impactWidthScale;
            this.impactHeightScale = impactHeightScale;
        }

        public void UpdateWindup(float progress, float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            Vector3 position =
                pose.BodyBasePosition +
                Vector3.back * windupBack * progress +
                Vector3.up * windupUp * progress;

            Quaternion rotation =
                pose.BodyBaseRotation *
                Quaternion.Euler(windupPitch * progress, 0f, 0f);

            pose.ModelRoot.localScale =
                pose.ModelRootBaseScale * (1f + windupScale * progress);

            ApplyBodyPose(position, rotation, deltaTime);
        }

        public void UpdateRecovery(float progress, float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            float weight = 1f - progress;

            Vector3 position =
                pose.BodyBasePosition +
                Vector3.forward * recoveryForward * weight +
                Vector3.down * recoveryDown * weight;

            Quaternion rotation =
                pose.BodyBaseRotation *
                Quaternion.Euler(recoveryPitch * weight, 0f, 0f);

            Vector3 impactScale = new Vector3(
                pose.ModelRootBaseScale.x * impactWidthScale,
                pose.ModelRootBaseScale.y * impactHeightScale,
                pose.ModelRootBaseScale.z * impactWidthScale);

            pose.ModelRoot.localScale =
                Vector3.Lerp(pose.ModelRootBaseScale, impactScale, weight);

            ApplyBodyPose(position, rotation, deltaTime);
        }

        private void ApplyBodyPose(Vector3 position, Quaternion rotation, float deltaTime)
        {
            float smoothFactor = 1f - Mathf.Exp(-transitionSpeed * deltaTime);

            pose.Body.localPosition = Vector3.Lerp(pose.Body.localPosition, position, smoothFactor);
            pose.Body.localRotation = Quaternion.Slerp(pose.Body.localRotation, rotation, smoothFactor);
        }
    }
}