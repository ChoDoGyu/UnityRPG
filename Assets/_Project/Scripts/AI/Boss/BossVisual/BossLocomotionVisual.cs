using UnityEngine;

namespace UnityRPG.AI
{
    public sealed class BossLocomotionVisual
    {
        private readonly BossVisualPose pose;

        private readonly float idleCycleSpeed;
        private readonly float idleBob;

        private readonly float moveCycleSpeed;
        private readonly float moveBob;
        private readonly float moveLean;

        private readonly float transitionSpeed;

        private float cycle;

        public BossLocomotionVisual(
            BossVisualPose pose,
            float idleCycleSpeed,
            float idleBob,
            float moveCycleSpeed,
            float moveBob,
            float moveLean,
            float transitionSpeed)
        {
            this.pose = pose;
            this.idleCycleSpeed = idleCycleSpeed;
            this.idleBob = idleBob;
            this.moveCycleSpeed = moveCycleSpeed;
            this.moveBob = moveBob;
            this.moveLean = moveLean;
            this.transitionSpeed = transitionSpeed;
        }

        public void UpdateIdle(float deltaTime)
        {
            cycle += idleCycleSpeed * deltaTime;

            float bob = Mathf.Sin(cycle) * idleBob;
            Vector3 position = pose.BodyBasePosition + Vector3.up * bob;

            ApplyBodyPose(position, pose.BodyBaseRotation, deltaTime);
        }

        public void UpdateMovement(float deltaTime)
        {
            cycle += moveCycleSpeed * deltaTime;

            float bob = Mathf.Abs(Mathf.Sin(cycle)) * moveBob;
            Vector3 position = pose.BodyBasePosition + Vector3.up * bob;
            Quaternion rotation = pose.BodyBaseRotation * Quaternion.Euler(moveLean, 0f, 0f);

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