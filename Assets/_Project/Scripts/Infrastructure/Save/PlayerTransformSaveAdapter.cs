using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRPG.Infrastructure.Save
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerTransformSaveAdapter : MonoBehaviour
    {
        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        public bool Capture(PlayerSaveData data)
        {
            if (data == null)
                return false;

            Vector3 position = transform.position;

            data.sceneName = SceneManager.GetActiveScene().name;
            data.positionX = position.x;
            data.positionY = position.y;
            data.positionZ = position.z;
            return true;
        }

        public bool Restore(PlayerSaveData data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.sceneName))
                return false;

            if (SceneManager.GetActiveScene().name != data.sceneName)
                return false;

            Vector3 position = new Vector3(data.positionX, data.positionY, data.positionZ);

            characterController.enabled = false;
            transform.position = position;
            characterController.enabled = true;

            return true;
        }
    }
}