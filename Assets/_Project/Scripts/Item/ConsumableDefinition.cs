using UnityEngine;

namespace UnityRPG.Item
{
    [CreateAssetMenu(fileName = "Consumable_", menuName = "UnityRPG/Item/Consumable")]
    public sealed class ConsumableDefinition : ItemDefinition
    {
        [Header("Consumable")]
        [SerializeField, Min(1)] private int maxStack = 10;
        [SerializeField, Min(0f)] private float healAmount = 30f;

        public override ItemType Type => ItemType.Consumable;
        public override int MaxStack => maxStack;
        public float HealAmount => healAmount;
    }
}