using UnityEngine;
using UnityRPG.Character.Player;

namespace UnityRPG.Infrastructure.Save
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerCheckpointController))]
    public sealed class PlayerCheckpointSaveAdapter : MonoBehaviour
    {
        private PlayerCheckpointController checkpointController;

        private void Awake()
        {
            checkpointController = GetComponent<PlayerCheckpointController>();
        }

        public bool Capture(CheckpointSaveData data)
        {
            if (data == null)
                return false;

            data.hasCheckpoint = checkpointController.HasCheckpoint;

            if (!checkpointController.HasCheckpoint)
            {
                data.checkpointId = null;
                data.sceneName = null;
                data.positionX = 0f;
                data.positionY = 0f;
                data.positionZ = 0f;
                return true;
            }

            Vector3 position = checkpointController.RespawnPosition;

            data.checkpointId = checkpointController.CheckpointId;
            data.sceneName = checkpointController.SceneName;
            data.positionX = position.x;
            data.positionY = position.y;
            data.positionZ = position.z;
            return true;
        }

        public bool Restore(CheckpointSaveData data)
        {
            if (data == null)
                return false;

            if (!data.hasCheckpoint)
            {
                checkpointController.ClearCheckpoint();
                return true;
            }

            if (string.IsNullOrWhiteSpace(data.checkpointId) || string.IsNullOrWhiteSpace(data.sceneName))
                return false;

            Vector3 position = new Vector3(data.positionX, data.positionY, data.positionZ);
            return checkpointController.TryActivateCheckpoint(data.checkpointId, data.sceneName, position);
        }
    }
}