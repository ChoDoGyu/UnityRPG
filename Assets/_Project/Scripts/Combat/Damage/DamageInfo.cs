using UnityEngine;

namespace UnityRPG.Combat
{
    public readonly struct DamageInfo
    {
        public float Amount { get; }
        public GameObject Source { get; }
        public bool IsCritical { get; }

        public DamageInfo(float amount, GameObject source, bool isCritical = false)
        {
            Amount = Mathf.Max(0f, amount);

            Source = source;

            IsCritical = isCritical;
        }
    }
}