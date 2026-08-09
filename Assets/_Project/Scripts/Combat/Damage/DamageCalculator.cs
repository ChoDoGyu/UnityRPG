using UnityEngine;

namespace UnityRPG.Combat
{
    public static class DamageCalculator
    {
        private const float MinimumDamage = 1f;

        public static float CalculateAfterDefense(
            float damage,
            float defense)
        {
            if (damage <= 0f)
            {
                return 0f;
            }

            return Mathf.Max(
                MinimumDamage,
                damage - Mathf.Max(0f, defense));
        }
    }
}