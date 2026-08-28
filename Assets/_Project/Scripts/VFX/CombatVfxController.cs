using UnityEngine;

namespace UnityRPG.VFX
{
    [DisallowMultipleComponent]
    public sealed class CombatVfxController : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] private Transform slashVfxPoint;

        [Header("Hit")]
        [SerializeField] private GameObject hitVfxPrefab;

        [Header("Player Attack")]
        [SerializeField] private GameObject basicSlashVfxPrefab;
        [SerializeField] private GameObject dashSlashVfxPrefab;
        [SerializeField] private GameObject spinAttackVfxPrefab;
        [SerializeField, Min(0f)] private float spinAttackVfxHeight = 0.9f;
        [SerializeField] private GameObject attackBuffVfxPrefab;
        [SerializeField, Min(0f)] private float attackBuffCleanupDelay = 1.2f;

        private GameObject activeAttackBuffVfx;

        public void PlayHit(Vector3 position)
        {
            VfxSpawner.Spawn(hitVfxPrefab, position, Quaternion.identity);
        }

        public void PlayBasicSlash(int comboStep)
        {
            if (basicSlashVfxPrefab == null || slashVfxPoint == null)
                return;

            Quaternion rotation = GetBasicSlashRotation(comboStep, slashVfxPoint.rotation);
            VfxSpawner.Spawn(basicSlashVfxPrefab, slashVfxPoint.position, rotation);
        }

        public void PlayDashSlash()
        {
            if (dashSlashVfxPrefab == null || slashVfxPoint == null)
                return;

            VfxSpawner.SpawnAttached(dashSlashVfxPrefab, slashVfxPoint);
        }

        private static Quaternion GetBasicSlashRotation(int comboStep, Quaternion baseRotation)
        {
            float zAngle = comboStep switch
            {
                1 => 35f,
                2 => -35f,
                3 => 0f,
                _ => 0f
            };

            return baseRotation * Quaternion.Euler(0f, 0f, zAngle);
        }

        public void PlaySpinAttack()
        {
            if (spinAttackVfxPrefab == null)
                return;

            Vector3 position = transform.position + Vector3.up * spinAttackVfxHeight;
            VfxSpawner.Spawn(spinAttackVfxPrefab, position, Quaternion.identity);
        }

        public void PlayAttackBuff()
        {
            if (attackBuffVfxPrefab == null || activeAttackBuffVfx != null)
                return;

            activeAttackBuffVfx = VfxSpawner.SpawnAttached(attackBuffVfxPrefab, transform);
        }

        public void StopAttackBuff()
        {
            if (activeAttackBuffVfx == null)
                return;

            ParticleSystem[] particleSystems = activeAttackBuffVfx.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < particleSystems.Length; i++)
                particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);

            Destroy(activeAttackBuffVfx, attackBuffCleanupDelay);
            activeAttackBuffVfx = null;
        }
    }
}