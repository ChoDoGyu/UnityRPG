using UnityEngine;
using UnityRPG.Character.Player;

namespace UnityRPG.VFX
{
    [DisallowMultipleComponent]
    public sealed class PlayerDamageCameraFeedback : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerCameraController cameraController;

        private void OnEnable()
        {
            if (playerHealth != null)
                playerHealth.Damaged += HandleDamaged;
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.Damaged -= HandleDamaged;
        }

        private void HandleDamaged(float damage)
        {
            if (cameraController != null)
                cameraController.PlayDamageShake();
        }
    }
}