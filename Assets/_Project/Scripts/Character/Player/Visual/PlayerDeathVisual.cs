using UnityEngine;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerDeathVisual
    {
        private readonly PlayerVisualPose pose;
        private readonly float deathDuration;

        private Quaternion startRotation;
        private Quaternion targetRotation;

        private float elapsedTime;
        private bool isPlaying;

        public bool IsPlaying => isPlaying;

        public PlayerDeathVisual(
            PlayerVisualPose pose,
            float deathDuration)
        {
            this.pose = pose;
            this.deathDuration = Mathf.Max(0.01f, deathDuration);
        }

        public void BeginDeath()
        {
            if (isPlaying)
            {
                return;
            }

            elapsedTime = 0f;
            isPlaying = true;

            pose.ModelRoot.localScale = pose.ModelRootBaseScale;

            startRotation = pose.ModelRoot.localRotation;
            targetRotation =
                pose.ModelRootBaseRotation *
                Quaternion.Euler(0f, 0f, 90f);
        }

        public bool UpdateDeath(float deltaTime)
        {
            if (!isPlaying)
            {
                return true;
            }

            elapsedTime += deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / deathDuration);

            pose.ModelRoot.localRotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                progress);

            if (progress < 1f)
            {
                return false;
            }

            pose.ModelRoot.localRotation = targetRotation;
            isPlaying = false;

            return true;
        }
    }
}