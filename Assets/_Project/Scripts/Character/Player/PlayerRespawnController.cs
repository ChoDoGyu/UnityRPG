using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRPG.Character.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerStateController))]
    [RequireComponent(typeof(PlayerVisualAnimator))]
    [RequireComponent(typeof(PlayerCheckpointController))]
    public sealed class PlayerRespawnController : MonoBehaviour
    {
        private CharacterController characterController;
        private PlayerHealth playerHealth;
        private PlayerStateController stateController;
        private PlayerVisualAnimator visualAnimator;
        private PlayerCheckpointController checkpointController;
        private bool isRespawning;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerHealth = GetComponent<PlayerHealth>();
            stateController = GetComponent<PlayerStateController>();
            visualAnimator = GetComponent<PlayerVisualAnimator>();
            checkpointController = GetComponent<PlayerCheckpointController>();
        }

        private void OnEnable()
        {
            playerHealth.Died += HandleDied;
        }

        private void OnDisable()
        {
            playerHealth.Died -= HandleDied;
        }

        private void HandleDied()
        {
            if (!isRespawning)
                StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            isRespawning = true;

            yield return null;

            while (visualAnimator.IsDeathAnimationPlaying)
                yield return null;

            if (!checkpointController.HasCheckpoint)
            {
                isRespawning = false;
                yield break;
            }

            if (checkpointController.SceneName != SceneManager.GetActiveScene().name)
            {
                isRespawning = false;
                yield break;
            }

            characterController.enabled = false;
            transform.position = checkpointController.RespawnPosition;
            characterController.enabled = true;

            playerHealth.Revive();
            visualAnimator.ResetAfterDeath();
            stateController.ExitDead();

            isRespawning = false;
        }
    }
}