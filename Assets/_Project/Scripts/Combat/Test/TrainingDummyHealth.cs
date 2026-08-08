using UnityEngine;

namespace UnityRPG.Combat
{
    public sealed class TrainingDummyHealth :
        MonoBehaviour,
        IDamageable
    {
        public void TakeDamage(
            DamageInfo damageInfo)
        {
        }
    }
}