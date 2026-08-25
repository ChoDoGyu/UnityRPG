using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Item;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class InventorySlotUI : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private GameObject itemIcon;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text stackText;
        [SerializeField] private GameObject selectedBorder;

        private Button button;
        private InventorySlot slot;
        private Action<InventorySlotUI> clicked;

        public InventorySlot Slot => slot;

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

        public void Initialize(Action<InventorySlotUI> onClicked)
        {
            clicked = onClicked;
            Clear();
        }

        public void SetSlot(InventorySlot inventorySlot)
        {
            slot = inventorySlot;

            if (slot == null || slot.Item == null)
            {
                Clear();
                return;
            }

            itemIcon.SetActive(false);

            itemNameText.gameObject.SetActive(true);
            itemNameText.text = slot.Item.DisplayName;

            bool showStack = slot.Count > 1;
            stackText.gameObject.SetActive(showStack);
            stackText.text = showStack ? slot.Count.ToString() : string.Empty;
        }

        public void SetSelected(bool selected)
        {
            selectedBorder.SetActive(selected);
        }

        public void Clear()
        {
            slot = null;

            itemIcon.SetActive(false);

            itemNameText.gameObject.SetActive(false);
            itemNameText.text = string.Empty;

            stackText.gameObject.SetActive(false);
            stackText.text = string.Empty;

            selectedBorder.SetActive(false);
        }

        private void HandleClicked()
        {
            clicked?.Invoke(this);
        }
    }
}