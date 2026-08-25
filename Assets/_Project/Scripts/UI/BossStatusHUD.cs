using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRPG.AI;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class BossStatusHUD : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private EnemyHealth bossHealth;
        [SerializeField] private BossCombatController bossCombatController;
        [SerializeField] private BossController bossController;

        [Header("Boss")]
        [SerializeField] private string bossDisplayName = "Boss";

        [Header("View")]
        [SerializeField] private TMP_Text bossNameText;
        [SerializeField] private TMP_Text phaseText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text healthText;

        private CanvasGroup canvasGroup;
        private bool isInitialized;
        private bool isVisible;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] BossStatusHUD의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            bossNameText.text = bossDisplayName;
            bossHealth.Died += HandleBossDied;

            SetVisible(false);
            Refresh();
            isInitialized = true;
        }

        private void LateUpdate()
        {
            if (!isInitialized)
                return;

            if (bossHealth == null || bossCombatController == null || bossController == null)
            {
                SetVisible(false);
                return;
            }

            if (bossHealth.IsDead)
            {
                SetVisible(false);
                return;
            }

            bool isEngaged = bossController.CurrentTarget != null;
            SetVisible(isEngaged);

            if (!isEngaged)
                return;

            Refresh();
        }

        private void OnDestroy()
        {
            if (bossHealth != null)
                bossHealth.Died -= HandleBossDied;
        }

        private void Refresh()
        {
            RefreshHealth();
            RefreshPhase();
        }

        private void RefreshHealth()
        {
            float maxHealth = bossHealth.MaxHealth;
            float currentHealth = bossHealth.CurrentHealth;
            float ratio = maxHealth > 0f ? currentHealth / maxHealth : 0f;

            healthSlider.SetValueWithoutNotify(Mathf.Clamp01(ratio));
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
        }

        private void RefreshPhase()
        {
            phaseText.text = bossCombatController.CurrentPhase == BossPhase.Phase1 ? "Phase 1" : "Phase 2";
        }

        private void HandleBossDied()
        {
            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            if (isVisible == visible)
                return;

            isVisible = visible;
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private bool HasAllReferences()
        {
            return bossHealth != null &&
                   bossCombatController != null &&
                   bossController != null &&
                   bossNameText != null &&
                   phaseText != null &&
                   healthSlider != null &&
                   healthText != null;
        }
    }
}