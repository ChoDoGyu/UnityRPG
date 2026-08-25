using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Item;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class InventoryUI : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private PlayerConsumableController playerConsumableController;
        [SerializeField] private PlayerEquipment playerEquipment;
        [SerializeField] private PlayerEquipmentController playerEquipmentController;

        [Header("Inventory Slots")]
        [SerializeField] private Transform slotGridRoot;
        [SerializeField] private InventorySlotUI slotPrefab;

        [Header("Equipment")]
        [SerializeField] private GameObject equipmentSlotsPanel;
        [SerializeField] private EquipmentSlotUI weaponSlot;
        [SerializeField] private EquipmentSlotUI headSlot;
        [SerializeField] private EquipmentSlotUI bodySlot;
        [SerializeField] private EquipmentSlotUI accessorySlot;

        [Header("Detail")]
        [SerializeField] private GameObject itemDetailPanel;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text itemTypeText;
        [SerializeField] private TMP_Text itemDescriptionText;
        [SerializeField] private TMP_Text itemAmountText;
        [SerializeField] private Button actionButton;
        [SerializeField] private TMP_Text actionText;

        private readonly List<InventorySlotUI> slotViews = new List<InventorySlotUI>();

        private InventorySlot selectedSlot;
        private InventorySlotUI selectedSlotView;
        private EquipmentSlotUI selectedEquipmentSlotView;

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] InventoryUI의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            BuildSlots();
            InitializeEquipmentSlots();

            playerInventory.Changed += Refresh;
            playerEquipment.Changed += RefreshEquipment;
            actionButton.onClick.AddListener(HandleActionClicked);

            Refresh();
            RefreshEquipment();
            ClearSelection();
        }

        private void OnDestroy()
        {
            if (playerInventory != null)
                playerInventory.Changed -= Refresh;

            if (playerEquipment != null)
                playerEquipment.Changed -= RefreshEquipment;

            if (actionButton != null)
                actionButton.onClick.RemoveListener(HandleActionClicked);
        }

        private void BuildSlots()
        {
            for (int i = 0; i < playerInventory.MaxSlots; i++)
            {
                InventorySlotUI slotView = Instantiate(slotPrefab, slotGridRoot);
                slotView.Initialize(HandleInventorySlotSelected);
                slotViews.Add(slotView);
            }
        }

        private void InitializeEquipmentSlots()
        {
            weaponSlot.Initialize(HandleEquipmentSlotSelected);
            headSlot.Initialize(HandleEquipmentSlotSelected);
            bodySlot.Initialize(HandleEquipmentSlotSelected);
            accessorySlot.Initialize(HandleEquipmentSlotSelected);
        }

        private void Refresh()
        {
            IReadOnlyList<InventorySlot> inventorySlots = playerInventory.Slots;

            for (int i = 0; i < slotViews.Count; i++)
            {
                if (i < inventorySlots.Count)
                    slotViews[i].SetSlot(inventorySlots[i]);
                else
                    slotViews[i].Clear();
            }

            RefreshInventorySelection(inventorySlots);
        }

        private void RefreshEquipment()
        {
            weaponSlot.SetEquipment(playerEquipment.GetEquipped(EquipmentSlot.Weapon));
            headSlot.SetEquipment(playerEquipment.GetEquipped(EquipmentSlot.Head));
            bodySlot.SetEquipment(playerEquipment.GetEquipped(EquipmentSlot.Body));
            accessorySlot.SetEquipment(playerEquipment.GetEquipped(EquipmentSlot.Accessory));

            if (selectedEquipmentSlotView == null)
                return;

            if (selectedEquipmentSlotView.EquippedItem == null)
            {
                ClearSelection();
                return;
            }

            selectedEquipmentSlotView.SetSelected(true);
            RefreshEquipmentDetail(selectedEquipmentSlotView);
        }

        private void HandleInventorySlotSelected(InventorySlotUI slotView)
        {
            if (slotView == null || slotView.Slot == null)
            {
                ClearSelection();
                return;
            }

            ClearSelectedBorders();

            selectedSlot = slotView.Slot;
            selectedSlotView = slotView;
            selectedEquipmentSlotView = null;

            selectedSlotView.SetSelected(true);
            RefreshInventoryDetail(selectedSlot);
        }

        private void HandleEquipmentSlotSelected(EquipmentSlotUI slotView)
        {
            if (slotView == null || slotView.EquippedItem == null)
            {
                ClearSelection();
                return;
            }

            ClearSelectedBorders();

            selectedSlot = null;
            selectedSlotView = null;
            selectedEquipmentSlotView = slotView;

            selectedEquipmentSlotView.SetSelected(true);
            RefreshEquipmentDetail(selectedEquipmentSlotView);
        }

        private void HandleActionClicked()
        {
            if (selectedEquipmentSlotView != null)
            {
                if (selectedEquipmentSlotView.EquippedItem != null)
                    playerEquipmentController.TryUnequip(selectedEquipmentSlotView.Slot);

                return;
            }

            if (selectedSlot == null || selectedSlot.Item == null)
                return;

            if (selectedSlot.Item is ConsumableDefinition consumable)
            {
                playerConsumableController.TryUse(consumable);
                return;
            }

            if (selectedSlot.Item is EquipmentDefinition equipment)
                playerEquipmentController.TryEquip(equipment);
        }

        private void RefreshInventorySelection(IReadOnlyList<InventorySlot> inventorySlots)
        {
            if (selectedSlot == null)
                return;

            int selectedIndex = -1;

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (!ReferenceEquals(inventorySlots[i], selectedSlot))
                    continue;

                selectedIndex = i;
                break;
            }

            if (selectedIndex < 0)
            {
                ClearSelection();
                return;
            }

            if (selectedSlotView != null)
                selectedSlotView.SetSelected(false);

            selectedSlotView = slotViews[selectedIndex];
            selectedSlotView.SetSelected(true);
            RefreshInventoryDetail(selectedSlot);
        }

        private void RefreshInventoryDetail(InventorySlot slot)
        {
            ItemDefinition item = slot.Item;

            ShowDetail(item);
            itemAmountText.text = $"Amount: {slot.Count}";

            if (item is ConsumableDefinition consumable)
            {
                itemTypeText.text = $"Consumable - Heal {consumable.HealAmount:0.#}";
                actionText.text = "Use";
                actionButton.interactable = true;
                return;
            }

            if (item is EquipmentDefinition equipment)
            {
                itemTypeText.text = $"Equipment - {equipment.Slot}";
                actionText.text = "Equip";
                actionButton.interactable = true;
                return;
            }

            itemTypeText.text = item.Type.ToString();
            actionText.text = string.Empty;
            actionButton.interactable = false;
        }

        private void RefreshEquipmentDetail(EquipmentSlotUI slotView)
        {
            EquipmentDefinition equipment = slotView.EquippedItem;

            ShowDetail(equipment);

            itemTypeText.text = $"Equipment - {equipment.Slot}";
            itemAmountText.text = "Equipped";
            actionText.text = "Unequip";
            actionButton.interactable = true;
        }

        private void ShowDetail(ItemDefinition item)
        {
            equipmentSlotsPanel.SetActive(false);
            itemDetailPanel.SetActive(true);

            itemNameText.text = item.DisplayName;
            itemDescriptionText.text = string.IsNullOrWhiteSpace(item.Description) ? "-" : item.Description;
        }

        private void ClearSelection()
        {
            ClearSelectedBorders();

            selectedSlot = null;
            selectedSlotView = null;
            selectedEquipmentSlotView = null;

            itemDetailPanel.SetActive(false);
            equipmentSlotsPanel.SetActive(true);
        }

        private void ClearSelectedBorders()
        {
            if (selectedSlotView != null)
                selectedSlotView.SetSelected(false);

            if (selectedEquipmentSlotView != null)
                selectedEquipmentSlotView.SetSelected(false);
        }

        private bool HasAllReferences()
        {
            return playerInventory != null &&
                   playerConsumableController != null &&
                   playerEquipment != null &&
                   playerEquipmentController != null &&
                   slotGridRoot != null &&
                   slotPrefab != null &&
                   equipmentSlotsPanel != null &&
                   weaponSlot != null &&
                   headSlot != null &&
                   bodySlot != null &&
                   accessorySlot != null &&
                   itemDetailPanel != null &&
                   itemNameText != null &&
                   itemTypeText != null &&
                   itemDescriptionText != null &&
                   itemAmountText != null &&
                   actionButton != null &&
                   actionText != null;
        }
    }
}