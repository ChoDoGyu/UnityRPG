using System.Collections.Generic;
using UnityEngine;

namespace UnityRPG.Item
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerEquipment))]
    public sealed class PlayerEquipmentVisual : MonoBehaviour
    {
        [Header("Weapon")]
        [SerializeField] private Transform weaponAnchor;
        [SerializeField] private Transform weaponPoint;

        [Header("Armor")]
        [SerializeField] private Transform headEquipPoint;
        [SerializeField] private Transform bodyEquipPoint;

        private readonly Dictionary<EquipmentSlot, GameObject> activeVisuals = new();

        private PlayerEquipment equipment;

        private void Awake()
        {
            equipment = GetComponent<PlayerEquipment>();
            SyncWeaponPoint();
        }

        private void OnEnable()
        {
            equipment.Changed += RefreshVisuals;
            RefreshVisuals();
        }

        private void OnDisable()
        {
            equipment.Changed -= RefreshVisuals;
        }

        private void LateUpdate()
        {
            if (activeVisuals.ContainsKey(EquipmentSlot.Weapon))
                SyncWeaponPoint();
        }

        private void SyncWeaponPoint()
        {
            if (weaponAnchor == null || weaponPoint == null)
                return;

            weaponPoint.SetPositionAndRotation(weaponAnchor.position, weaponAnchor.rotation);
        }

        private void RefreshVisuals()
        {
            RefreshVisual(EquipmentSlot.Weapon);
            RefreshVisual(EquipmentSlot.Head);
            RefreshVisual(EquipmentSlot.Body);
        }

        private void RefreshVisual(EquipmentSlot slot)
        {
            if (activeVisuals.TryGetValue(slot, out GameObject currentVisual))
            {
                Destroy(currentVisual);
                activeVisuals.Remove(slot);
            }

            EquipmentDefinition equipped = equipment.GetEquipped(slot);

            if (equipped == null || equipped.EquippedVisualPrefab == null)
                return;

            Transform point = GetEquipPoint(slot);

            if (point == null)
                return;

            if (slot == EquipmentSlot.Weapon)
                SyncWeaponPoint();

            GameObject visual = Instantiate(equipped.EquippedVisualPrefab, point);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;

            activeVisuals.Add(slot, visual);
        }

        private Transform GetEquipPoint(EquipmentSlot slot)
        {
            return slot switch
            {
                EquipmentSlot.Weapon => weaponPoint,
                EquipmentSlot.Head => headEquipPoint,
                EquipmentSlot.Body => bodyEquipPoint,
                _ => null
            };
        }
    }
}