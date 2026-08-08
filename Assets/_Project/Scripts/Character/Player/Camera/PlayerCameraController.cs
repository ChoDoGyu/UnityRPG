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

        private float yaw;
        private float pitch;

        public Transform CameraTarget => cameraTarget;

        private void Awake()
        {
            Vector3 rotation = cameraTarget.eulerAngles;

            yaw = rotation.y;
            pitch = NormalizeAngle(rotation.x);
        }

        public void RotateCamera(
            Vector2 lookInput,
            bool isMouseInput,
            float deltaTime)
        {
            if (lookInput.sqrMagnitude <= 0.001f)
            {
                return;
            }

            float sensitivity = isMouseInput
                ? mouseSensitivity
                : gamepadLookSpeed * deltaTime;

            yaw += lookInput.x * sensitivity;
            pitch -= lookInput.y * sensitivity;

            pitch = Mathf.Clamp(
                pitch,
                minPitch,
                maxPitch);

            cameraTarget.rotation =
                Quaternion.Euler(pitch, yaw, 0f);
        }

        private static float NormalizeAngle(float angle)
        {
            if (angle > 180f)
            {
                angle -= 360f;
            }

            return angle;
        }
    }
}