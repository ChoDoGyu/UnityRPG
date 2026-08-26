using UnityEngine;

namespace UnityRPG.Core
{
    [DisallowMultipleComponent]
    public sealed class SceneSpawnPoint : MonoBehaviour
    {
        public void ApplyTo(Transform target)
        {
            CharacterController characterController = target.GetComponent<CharacterController>();

            if (characterController != null)
                characterController.enabled = false;

            target.SetPositionAndRotation(transform.position, transform.rotation);

            if (characterController != null)
                characterController.enabled = true;
        }
    }
}