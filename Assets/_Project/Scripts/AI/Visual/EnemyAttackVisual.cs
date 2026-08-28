using UnityEngine;

namespace UnityRPG.AI
{
    public readonly struct EnemyMeleeVisualSettings
    {
        public float WindupBack { get; }
        public float WindupDown { get; }
        public float WindupPitch { get; }
        public float AttackForward { get; }
        public float AttackUp { get; }
        public float AttackPitch { get; }

        public EnemyMeleeVisualSettings(
            float windupBack,
            float windupDown,
            float windupPitch,
            float attackForward,
            float attackUp,
            float attackPitch)
        {
            WindupBack = windupBack;
            WindupDown = windupDown;
            WindupPitch = windupPitch;
            AttackForward = attackForward;
            AttackUp = attackUp;
            AttackPitch = attackPitch;
        }
    }

    public readonly struct EnemyRangedVisualSettings
    {
        public float WindupForward { get; }
        public float WindupDown { get; }
        public float WindupPitch { get; }
        public float WindupScale { get; }
        public float RecoveryBack { get; }
        public float RecoveryPitch { get; }
        public float RecoveryScale { get; }

        public EnemyRangedVisualSettings(
            float windupForward,
            float windupDown,
            float windupPitch,
            float windupScale,
            float recoveryBack,
            float recoveryPitch,
            float recoveryScale)
        {
            WindupForward = windupForward;
            WindupDown = windupDown;
            WindupPitch = windupPitch;
            WindupScale = windupScale;
            RecoveryBack = recoveryBack;
            RecoveryPitch = recoveryPitch;
            RecoveryScale = recoveryScale;
        }
    }

    public sealed class EnemyAttackVisual
    {
        private readonly EnemyVisualPose pose;
        private readonly float transitionSpeed;
        private readonly EnemyMeleeVisualSettings melee;
        private readonly EnemyRangedVisualSettings ranged;

        public EnemyAttackVisual(
            EnemyVisualPose pose,
            float transitionSpeed,
            EnemyMeleeVisualSettings melee,
            EnemyRangedVisualSettings ranged)
        {
            this.pose = pose;
            this.transitionSpeed = transitionSpeed;
            this.melee = melee;
            this.ranged = ranged;
        }

        public void UpdateWindup(EnemyType enemyType, float progress, float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            if (enemyType == EnemyType.Melee)
                UpdateMeleeWindup(progress, deltaTime);
            else
                UpdateRangedWindup(progress, deltaTime);
        }

        public void UpdateRecovery(EnemyType enemyType, float progress, float deltaTime)
        {
            progress = Mathf.Clamp01(progress);

            if (enemyType == EnemyType.Melee)
                UpdateMeleeRecovery(progress, deltaTime);
            else
                UpdateRangedRecovery(progress, deltaTime);
        }

        private void UpdateMeleeWindup(float progress, float deltaTime)
        {
            Vector3 position =
                pose.BodyBasePosition +
                Vector3.back * melee.WindupBack * progress +
                Vector3.down * melee.WindupDown * progress;

            Quaternion rotation =
                pose.BodyBaseRotation *
                Quaternion.Euler(melee.WindupPitch * progress, 0f, 0f);

            ApplyBodyPose(position, rotation, deltaTime);
        }

        private void UpdateMeleeRecovery(float progress, float deltaTime)
        {
            float weight = 1f - progress;

            Vector3 position =
                pose.BodyBasePosition +
                Vector3.forward * melee.AttackForward * weight +
                Vector3.up * melee.AttackUp * weight;

            Quaternion rotation =
                pose.BodyBaseRotation *
                Quaternion.Euler(melee.AttackPitch * weight, 0f, 0f);

            ApplyBodyPose(position, rotation, deltaTime);
        }

        private void UpdateRangedWindup(float progress, float deltaTime)
        {
            Vector3 position =
                pose.BodyBasePosition +
                Vector3.forward * ranged.WindupForward * progress +
                Vector3.down * ranged.WindupDown * progress;

            Quaternion rotation =
                pose.BodyBaseRotation *
                Quaternion.Euler(ranged.WindupPitch * progress, 0f, 0f);

            pose.ModelRoot.localScale =
                pose.ModelRootBaseScale * (1f - ranged.WindupScale * progress);

            ApplyBodyPose(position, rotation, deltaTime);
        }

        private void UpdateRangedRecovery(float progress, float deltaTime)
        {
            float weight = 1f - progress;

            Vector3 position =
                pose.BodyBasePosition +
                Vector3.back * ranged.RecoveryBack * weight;

            Quaternion rotation =
                pose.BodyBaseRotation *
                Quaternion.Euler(ranged.RecoveryPitch * weight, 0f, 0f);

            pose.ModelRoot.localScale =
                pose.ModelRootBaseScale * (1f + ranged.RecoveryScale * weight);

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