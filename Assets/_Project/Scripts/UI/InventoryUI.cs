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
        [SerializeField] private PlayerEquipmentController playerEquipmentController;

        [Header("Slots")]
        [SerializeField] private Transform slotGridRoot;
        [SerializeField] private InventorySlotUI slotPrefab;

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

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] InventoryUI의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            BuildSlots();

            playerInventory.Changed += Refresh;
            actionButton.onClick.AddListener(HandleActionClicked);

            Refresh();
            ClearSelection();
        }

        private void OnDestroy()
        {
            if (playerInventory != null)
                playerInventory.Changed -= Refresh;

            if (actionButton != null)
                actionButton.onClick.RemoveListener(HandleActionClicked);
        }

        private void BuildSlots()
        {
            for (int i = 0; i < playerInventory.MaxSlots; i++)
            {
                InventorySlotUI slotView = Instantiate(slotPrefab, slotGridRoot);
                slotView.Initialize(HandleSlotSelected);
                slotViews.Add(slotView);
            }
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

            RefreshSelection(inventorySlots);
        }

        private void HandleSlotSelected(InventorySlotUI slotView)
        {
            if (slotView == null || slotView.Slot == null)
            {
                ClearSelection();
                return;
            }

            if (selectedSlotView != null)
                selectedSlotView.SetSelected(false);

            selectedSlotView = slotView;
            selectedSlot = slotView.Slot;

            selectedSlotView.SetSelected(true);
            RefreshDetail(selectedSlot);
        }

        private void HandleActionClicked()
        {
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

        private void RefreshSelection(IReadOnlyList<InventorySlot> inventorySlots)
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
            RefreshDetail(selectedSlot);
        }

        private void RefreshDetail(InventorySlot slot)
        {
            ItemDefinition item = slot.Item;

            itemDetailPanel.SetActive(true);

            itemNameText.text = item.DisplayName;
            itemDescriptionText.text = string.IsNullOrWhiteSpace(item.Description) ? "-" : item.Description;
            itemAmountText.text = $"Amount: {slot.Count}";

            if (item is ConsumableDefinition consumable)
            {
                itemTypeText.text = $"Consumable · Heal {consumable.HealAmount:0.#}";
                actionText.text = "Use";
                actionButton.interactable = true;
                return;
            }

            if (item is EquipmentDefinition equipment)
            {
                itemTypeText.text = $"Equipment · {equipment.Slot}";
                actionText.text = "Equip";
                actionButton.interactable = true;
                return;
            }

            itemTypeText.text = item.Type.ToString();
            actionText.text = string.Empty;
            actionButton.interactable = false;
        }

        private void ClearSelection()
        {
            if (selectedSlotView != null)
                selectedSlotView.SetSelected(false);

            selectedSlot = null;
            selectedSlotView = null;

            itemDetailPanel.SetActive(false);
        }

        private bool HasAllReferences()
        {
            return playerInventory != null &&
                   playerConsumableController != null &&
                   playerEquipmentController != null &&
                   slotGridRoot != null &&
                   slotPrefab != null &&
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