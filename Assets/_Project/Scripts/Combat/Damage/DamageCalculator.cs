using UnityEngine;

namespace UnityRPG.Combat
{
    public static class DamageCalculator
    {
        private const float MinimumDamage = 1f;

        public static float CalculateAfterDefense(float damage, float defense)
        {
            if (damage <= 0f)
            {
                return 0f;
            }

            return Mathf.Max(MinimumDamage, damage - Mathf.Max(0f, defense));
        }

        public static DamageInfo CreateAttackDamage(
            float attack, 
            float damageMultiplier, 
            float critChance, 
            float critDamage, 
            GameObject source)
        {
            float damage = Mathf.Max(0f, attack) * Mathf.Max(0f, damageMultiplier);

            bool isCritical = Random.value < Mathf.Clamp01(critChance);

            if (isCritical)
            {
                damage *= Mathf.Max(1f, critDamage);
            }

            return new DamageInfo(damage, source, isCritical);
        }
    }
}