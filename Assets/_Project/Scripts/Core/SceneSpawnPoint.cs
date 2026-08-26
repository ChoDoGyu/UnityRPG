using UnityEngine;

namespace UnityRPG.Core
{
    [DisallowMultipleComponent]
    public sealed class SceneSpawnPoint : MonoBehaviour
    {
        public void ApplyTo(Transform target)
        {
            target.SetPositionAndRotation(transform.position, transform.rotation);
        }
    }
}