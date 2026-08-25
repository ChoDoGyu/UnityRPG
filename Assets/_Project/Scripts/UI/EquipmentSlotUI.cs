using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Item;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class EquipmentSlotUI : MonoBehaviour
    {
        [Header("Equipment")]
        [SerializeField] private EquipmentSlot equipmentSlot;

        [Header("View")]
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private GameObject selectedBorder;

        private Button button;
        private Action<EquipmentSlotUI> clicked;

        public EquipmentSlot Slot => equipmentSlot;
        public EquipmentDefinition EquippedItem { get; private set; }

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(HandleClicked);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClicked);
        }

        public void Initialize(Action<EquipmentSlotUI> onClicked)
        {
            clicked = onClicked;
            SetSelected(false);
        }

        public void SetEquipment(EquipmentDefinition equipment)
        {
            EquippedItem = equipment;
            itemNameText.text = equipment != null ? equipment.DisplayName : "Empty";
        }

        public void SetSelected(bool selected)
        {
            selectedBorder.SetActive(selected);
        }

        private void HandleClicked()
        {
            clicked?.Invoke(this);
        }
    }
}