using UnityEngine;

namespace UnityRPG.Character.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerCheckpointController : MonoBehaviour
    {
        private string checkpointId;
        private string sceneName;
        private Vector3 respawnPosition;
        private bool hasCheckpoint;

        public bool HasCheckpoint => hasCheckpoint;
        public string CheckpointId => checkpointId;
        public string SceneName => sceneName;
        public Vector3 RespawnPosition => respawnPosition;

        public bool ActivateCheckpoint(string id, string scene, Vector3 position)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(scene))
                return false;

            checkpointId = id;
            sceneName = scene;
            respawnPosition = position;
            hasCheckpoint = true;
            return true;
        }

        public void ClearCheckpoint()
        {
            checkpointId = null;
            sceneName = null;
            respawnPosition = default;
            hasCheckpoint = false;
        }
    }
}