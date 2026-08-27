using UnityEngine;

namespace UnityRPG.VFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class VfxLifetime : MonoBehaviour
    {
        private ParticleSystem vfxParticleSystem;

        private void Awake()
        {
            vfxParticleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!vfxParticleSystem.IsAlive(true))
                Destroy(gameObject);
        }
    }
}