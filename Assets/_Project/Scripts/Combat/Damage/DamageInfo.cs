using UnityEngine;

namespace UnityRPG.Combat
{
    public readonly struct DamageInfo
    {
        public float Amount { get; }
        public GameObject Source { get; }

        public DamageInfo(
            float amount,
            GameObject source)
        {
            Amount =
                Mathf.Max(0f, amount);

            Source = source;
        }
    }
}