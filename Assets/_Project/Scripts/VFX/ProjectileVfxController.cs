using UnityEngine;

namespace UnityRPG.VFX
{
    [DisallowMultipleComponent]
    public sealed class ProjectileVfxController : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] private GameObject projectileVisual;
        [SerializeField] private ParticleSystem projectileParticle;
        [SerializeField] private TrailRenderer trailRenderer;

        [Header("Impact")]
        [SerializeField] private GameObject impactVfxPrefab;

        [Header("Cleanup")]
        [SerializeField, Min(0f)] private float cleanupDelay = 0.4f;

        public float Finish(Vector3 impactPosition, bool playImpact)
        {
            if (playImpact)
                PlayImpact(impactPosition);

            if (projectileVisual != null)
                projectileVisual.SetActive(false);

            if (projectileParticle != null)
                projectileParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            if (trailRenderer != null)
                trailRenderer.emitting = false;

            return cleanupDelay;
        }

        public void PlayImpact(Vector3 position)
        {
            VfxSpawner.Spawn(impactVfxPrefab, position, Quaternion.identity);
        }
    }
}