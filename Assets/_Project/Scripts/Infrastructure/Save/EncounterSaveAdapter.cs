using System.Collections.Generic;
using UnityEngine;
using UnityRPG.AI;

namespace UnityRPG.Infrastructure.Save
{
    [DisallowMultipleComponent]
    public sealed class EncounterSaveAdapter : MonoBehaviour
    {
        public bool Capture(SaveGameData data)
        {
            if (data == null || data.encounters == null)
                return false;

            data.encounters.Clear();

            EncounterTrigger[] encounters = FindObjectsByType<EncounterTrigger>(FindObjectsSortMode.None);
            HashSet<string> usedIds = new();

            for (int i = 0; i < encounters.Length; i++)
            {
                EncounterTrigger encounter = encounters[i];

                if (string.IsNullOrWhiteSpace(encounter.EncounterId) || !usedIds.Add(encounter.EncounterId))
                    return false;

                if (!encounter.HasStarted)
                    continue;

                data.encounters.Add(new EncounterSaveData
                {
                    encounterId = encounter.EncounterId,
                    hasStarted = encounter.HasStarted,
                    isCompleted = encounter.IsCompleted
                });
            }

            return true;
        }

        public bool Restore(SaveGameData data)
        {
            if (data == null || data.encounters == null)
                return false;

            EncounterTrigger[] encounters = FindObjectsByType<EncounterTrigger>(FindObjectsSortMode.None);
            Dictionary<string, EncounterTrigger> lookup = new();

            for (int i = 0; i < encounters.Length; i++)
            {
                EncounterTrigger encounter = encounters[i];

                if (string.IsNullOrWhiteSpace(encounter.EncounterId) ||
                    !lookup.TryAdd(encounter.EncounterId, encounter))
                    return false;
            }

            for (int i = 0; i < data.encounters.Count; i++)
            {
                EncounterSaveData saveEncounter = data.encounters[i];

                if (saveEncounter == null ||
                    string.IsNullOrWhiteSpace(saveEncounter.encounterId) ||
                    !lookup.TryGetValue(saveEncounter.encounterId, out EncounterTrigger encounter))
                    return false;

                if (!encounter.TryRestoreState(saveEncounter.hasStarted, saveEncounter.isCompleted))
                    return false;
            }

            return true;
        }
    }
}