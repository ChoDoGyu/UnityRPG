using System.Collections.Generic;

namespace UnityRPG.Character.Stats
{
    public sealed class RuntimeStat
    {
        private readonly List<StatModifier> modifiers =
            new List<StatModifier>();

        public float BaseValue { get; private set; }

        public float Value
        {
            get
            {
                float value = BaseValue;

                foreach (StatModifier modifier in modifiers)
                {
                    value += modifier.Value;
                }

                return value;
            }
        }

        public RuntimeStat(float baseValue)
        {
            BaseValue = baseValue;
        }

        public void SetBaseValue(float value)
        {
            BaseValue = value;
        }

        public void AddModifier(
            StatModifier modifier)
        {
            if (modifier == null)
            {
                return;
            }

            modifiers.Add(modifier);
        }

        public void RemoveModifiersFromSource(
            object source)
        {
            modifiers.RemoveAll(
                modifier =>
                    ReferenceEquals(
                        modifier.Source,
                        source));
        }
    }
}