using UnityEngine;

namespace UnityRPG.AI
{
    public readonly struct BossHeavySlashVisualSettings
    {
        public float WindupBack { get; }
        public Vector3 WindupRotation { get; }

        public float ActiveForward { get; }
        public Vector3 ActiveRotation { get; }

        public float RecoveryForward { get; }
        public Vector3 RecoveryRotation { get; }

        public BossHeavySlashVisualSettings(
            float windupBack,
            Vector3 windupRotation,
            float activeForward,
            Vector3 activeRotation,
            float recoveryForward,
            Vector3 recoveryRotation)
        {
            WindupBack = windupBack;
            WindupRotation = windupRotation;
            ActiveForward = activeForward;
            ActiveRotation = activeRotation;
            RecoveryForward = recoveryForward;
            RecoveryRotation = recoveryRotation;
        }
    }

    public readonly struct BossGroundSlamVisualSettings
    {
        public float WindupBack { get; }
        public float WindupUp { get; }
        public float WindupPitch { get; }
        public float WindupScale { get; }

        public float ActiveForward { get; }
        public float ActiveDown { get; }
        public float ActivePitch { get; }
        public float ImpactWidthScale { get; }
        public float ImpactHeightScale { get; }

        public float RecoveryForward { get; }
        public float RecoveryDown { get; }
        public float RecoveryPitch { get; }

        public BossGroundSlamVisualSettings(
            float windupBack,
            float windupUp,
            float windupPitch,
            float windupScale,
            float activeForward,
            float activeDown,
            float activePitch,
            float impactWidthScale,
            float impactHeightScale,
            float recoveryForward,
            float recoveryDown,
            float recoveryPitch)
        {
            WindupBack = windupBack;
            WindupUp = windupUp;
            WindupPitch = windupPitch;
            WindupScale = windupScale;
            ActiveForward = activeForward;
            ActiveDown = activeDown;
            ActivePitch = activePitch;
            ImpactWidthScale = impactWidthScale;
            ImpactHeightScale = impactHeightScale;
            RecoveryForward = recoveryForward;
            RecoveryDown = recoveryDown;
            RecoveryPitch = recoveryPitch;
        }
    }

    public readonly struct BossChargeVisualSettings
    {
        public float WindupBack { get; }
        public float WindupDown { get; }
        public float WindupPitch { get; }

        public float ActiveForward { get; }
        public float ActiveDown { get; }
        public float ActivePitch { get; }

        public float RecoveryForward { get; }
        public float RecoveryPitch { get; }

        public BossChargeVisualSettings(
            float windupBack,
            float windupDown,
            float windupPitch,
            float activeForward,
            float activeDown,
            float activePitch,
            float recoveryForward,
            float recoveryPitch)
        {
            WindupBack = windupBack;
            WindupDown = windupDown;
            WindupPitch = windupPitch;
            ActiveForward = activeForward;
            ActiveDown = activeDown;
            ActivePitch = activePitch;
            RecoveryForward = recoveryForward;
            RecoveryPitch = recoveryPitch;
        }
    }

    public readonly struct BossShockwaveVisualSettings
    {
        public float WindupBack { get; }
        public float WindupUp { get; }
        public float WindupPitch { get; }
        public float WindupScale { get; }

        public float ActiveForward { get; }
        public float ActivePitch { get; }

        public float RecoveryForward { get; }
        public float RecoveryPitch { get; }

        public BossShockwaveVisualSettings(
            float windupBack,
            float windupUp,
            float windupPitch,
            float windupScale,
            float activeForward,
            float activePitch,
            float recoveryForward,
            float recoveryPitch)
        {
            WindupBack = windupBack;
            WindupUp = windupUp;
            WindupPitch = windupPitch;
            WindupScale = windupScale;
            ActiveForward = activeForward;
            ActivePitch = activePitch;
            RecoveryForward = recoveryForward;
            RecoveryPitch = recoveryPitch;
        }
    }

    public sealed class BossPatternVisual
    {
        private readonly BossVisualPose pose;
        private readonly float transitionSpeed;

        private readonly BossHeavySlashVisualSettings heavySlash;
        private readonly BossGroundSlamVisualSettings groundSlam;
        private readonly BossChargeVisualSettings charge;
        private readonly BossShockwaveVisualSettings shockwave;

        public BossPatternVisual(
            BossVisualPose pose,
            float transitionSpeed,
            BossHeavySlashVisualSettings heavySlash,
            BossGroundSlamVisualSettings groundSlam,
            BossChargeVisualSettings charge,
            BossShockwaveVisualSettings shockwave)
        {
            this.pose = pose;
            this.transitionSpeed = transitionSpeed;
            this.heavySlash = heavySlash;
            this.groundSlam = groundSlam;
            this.charge = charge;
            this.shockwave = shockwave;
        }

        public void UpdatePattern(
            BossPatternType patternType,
            BossPatternPhase phase,
            float progress,
            float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            switch (patternType)
            {
                case BossPatternType.HeavySlash:
                    UpdateHeavySlash(phase, progress, deltaTime);
                    break;

                case BossPatternType.GroundSlam:
                    UpdateGroundSlam(phase, progress, deltaTime);
                    break;

                case BossPatternType.Charge:
                    UpdateCharge(phase, progress, deltaTime);
                    break;

                case BossPatternType.Shockwave:
                    UpdateShockwave(phase, progress, deltaTime);
                    break;
            }
        }

        private void UpdateHeavySlash(BossPatternPhase phase, float progress, float deltaTime)
        {
            switch (phase)
            {
                case BossPatternPhase.Windup:
                    {
                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.back * heavySlash.WindupBack * progress;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(heavySlash.WindupRotation * progress);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Active:
                    {
                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.forward * heavySlash.ActiveForward;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(heavySlash.ActiveRotation);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Recovery:
                    {
                        float weight = 1f - progress;

                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.forward * heavySlash.RecoveryForward * weight;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(heavySlash.RecoveryRotation * weight);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }
            }
        }

        private void UpdateGroundSlam(BossPatternPhase phase, float progress, float deltaTime)
        {
            switch (phase)
            {
                case BossPatternPhase.Windup:
                    {
                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.back * groundSlam.WindupBack * progress +
                            Vector3.up * groundSlam.WindupUp * progress;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(groundSlam.WindupPitch * progress, 0f, 0f);

                        pose.ModelRoot.localScale =
                            pose.ModelRootBaseScale * (1f + groundSlam.WindupScale * progress);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Active:
                    {
                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.forward * groundSlam.ActiveForward +
                            Vector3.down * groundSlam.ActiveDown;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(groundSlam.ActivePitch, 0f, 0f);

                        pose.ModelRoot.localScale = new Vector3(
                            pose.ModelRootBaseScale.x * groundSlam.ImpactWidthScale,
                            pose.ModelRootBaseScale.y * groundSlam.ImpactHeightScale,
                            pose.ModelRootBaseScale.z * groundSlam.ImpactWidthScale);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Recovery:
                    {
                        float weight = 1f - progress;

                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.forward * groundSlam.RecoveryForward * weight +
                            Vector3.down * groundSlam.RecoveryDown * weight;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(groundSlam.RecoveryPitch * weight, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }
            }
        }

        private void UpdateCharge(BossPatternPhase phase, float progress, float deltaTime)
        {
            switch (phase)
            {
                case BossPatternPhase.Windup:
                    {
                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.back * charge.WindupBack * progress +
                            Vector3.down * charge.WindupDown * progress;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(charge.WindupPitch * progress, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Active:
                    {
                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.forward * charge.ActiveForward +
                            Vector3.down * charge.ActiveDown;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(charge.ActivePitch, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Recovery:
                    {
                        float weight = 1f - progress;

                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.forward * charge.RecoveryForward * weight;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(charge.RecoveryPitch * weight, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }
            }
        }

        private void UpdateShockwave(BossPatternPhase phase, float progress, float deltaTime)
        {
            switch (phase)
            {
                case BossPatternPhase.Windup:
                    {
                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.back * shockwave.WindupBack * progress +
                            Vector3.up * shockwave.WindupUp * progress;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(shockwave.WindupPitch * progress, 0f, 0f);

                        pose.ModelRoot.localScale =
                            pose.ModelRootBaseScale * (1f + shockwave.WindupScale * progress);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Active:
                    {
                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.forward * shockwave.ActiveForward;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(shockwave.ActivePitch, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }

                case BossPatternPhase.Recovery:
                    {
                        float weight = 1f - progress;

                        Vector3 position =
                            pose.BodyBasePosition +
                            Vector3.forward * shockwave.RecoveryForward * weight;

                        Quaternion rotation =
                            pose.BodyBaseRotation *
                            Quaternion.Euler(shockwave.RecoveryPitch * weight, 0f, 0f);

                        ApplyBodyPose(position, rotation, deltaTime);
                        break;
                    }
            }
        }

        private void ApplyBodyPose(Vector3 position, Quaternion rotation, float deltaTime)
        {
            float smoothFactor = 1f - Mathf.Exp(-transitionSpeed * deltaTime);

            pose.Body.localPosition = Vector3.Lerp(pose.Body.localPosition, position, smoothFactor);
            pose.Body.localRotation = Quaternion.Slerp(pose.Body.localRotation, rotation, smoothFactor);
        }
    }
}