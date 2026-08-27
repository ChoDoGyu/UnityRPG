using UnityEngine;

namespace UnityRPG.VFX
{
    [DisallowMultipleComponent]
    public sealed class CombatVfxController : MonoBehaviour
    {
        [SerializeField] private GameObject hitVfxPrefab;

        public void PlayHit(Vector3 position)
        {
            VfxSpawner.Spawn(hitVfxPrefab, position, Quaternion.identity);
        }
    }
}