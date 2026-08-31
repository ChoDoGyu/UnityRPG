using UnityEngine;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerCameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField]
        private Transform cameraTarget;

        [Header("Look")]
        [SerializeField]
        [Min(0f)]
        private float mouseSensitivity = 0.15f;

        [SerializeField]
        [Min(0f)]
        private float gamepadLookSpeed = 120f;

        [Header("Vertical Limit")]
        [SerializeField]
        private float minPitch = -35f;

        [SerializeField]
        private float maxPitch = 70f;

        [Header("Shake")]
        [SerializeField, Min(0f)] private float damageShakeDuration = 0.14f;
        [SerializeField, Min(0f)] private float damageShakeStrength = 1.5f;

        private float yaw;
        private float pitch;
        private float shakeRemainingTime;
        private float shakeDuration;
        private float shakeStrength;

        private bool isConfigured;

        public Transform CameraTarget => cameraTarget;

        private void Awake()
        {
            if (cameraTarget == null)
            {
                Debug.LogError("[Player] PlayerCameraController의 Camera Target이 설정되지 않았습니다.");

                return;
            }

            Vector3 rotation = cameraTarget.eulerAngles;

            yaw = rotation.y;
            pitch = NormalizeAngle(rotation.x);

            isConfigured = true;
        }

        private void LateUpdate()
        {
            if (!isConfigured)
                return;

            float shakeYaw = 0f;
            float shakePitch = 0f;

            if (shakeRemainingTime > 0f)
            {
                float strength = shakeStrength * Mathf.Clamp01(shakeRemainingTime / shakeDuration);

                shakeYaw = Random.Range(-strength, strength);
                shakePitch = Random.Range(-strength, strength);

                shakeRemainingTime = Mathf.Max(0f, shakeRemainingTime - Time.deltaTime);
            }

            cameraTarget.rotation = Quaternion.Euler(pitch + shakePitch, yaw + shakeYaw, 0f);
        }

        public void RotateCamera(Vector2 lookInput, bool isMouseInput, float deltaTime)
        {
            if (!isConfigured)
            {
                return;
            }

            if (lookInput.sqrMagnitude <= 0.001f)
            {
                return;
            }

            float sensitivity = isMouseInput ? mouseSensitivity : gamepadLookSpeed * deltaTime;

            yaw += lookInput.x * sensitivity;
            pitch -= lookInput.y * sensitivity;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        private static float NormalizeAngle(float angle)
        {
            if (angle > 180f)
            {
                angle -= 360f;
            }

            return angle;
        }

        public void PlayDamageShake()
        {
            if (!isConfigured)
                return;

            shakeDuration = damageShakeDuration;
            shakeRemainingTime = damageShakeDuration;
            shakeStrength = damageShakeStrength;
        }
    }
}