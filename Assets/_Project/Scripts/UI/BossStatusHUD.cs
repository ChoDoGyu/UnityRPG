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

        private BossPhase lastPhase;
        private bool isVisible;
        private bool hasVisibilityState;
        private bool hasPhaseState;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (!HasViewReferences())
            {
                Debug.LogError("[UI] BossStatusHUD의 View 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            SetVisible(false);

            if (HasBossReferences())
                Bind(bossHealth, bossCombatController, bossController, bossDisplayName);
        }

        private void LateUpdate()
        {
            if (!HasBossReferences() || bossHealth.IsDead)
            {
                SetVisible(false);
                return;
            }

            bool isEngaged = bossController.CurrentTarget != null;
            SetVisible(isEngaged);

            if (isEngaged && (!hasPhaseState || bossCombatController.CurrentPhase != lastPhase))
                RefreshPhase();
        }

        private void OnDestroy()
        {
            Unbind();
        }

        public void Bind(EnemyHealth health, BossCombatController combatController, BossController controller, string displayName)
        {
            Unbind();

            bossHealth = health;
            bossCombatController = combatController;
            bossController = controller;

            if (!string.IsNullOrWhiteSpace(displayName))
                bossDisplayName = displayName;

            if (!HasBossReferences())
            {
                SetVisible(false);
                return;
            }

            bossNameText.text = bossDisplayName;

            bossHealth.Damaged += HandleBossDamaged;
            bossHealth.Died += HandleBossDied;

            RefreshHealth();
            RefreshPhase();
            SetVisible(false);
        }

        public void Unbind()
        {
            if (bossHealth != null)
            {
                bossHealth.Damaged -= HandleBossDamaged;
                bossHealth.Died -= HandleBossDied;
            }

            bossHealth = null;
            bossCombatController = null;
            bossController = null;

            hasPhaseState = false;
            SetVisible(false);
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
            lastPhase = bossCombatController.CurrentPhase;
            hasPhaseState = true;

            phaseText.text = lastPhase == BossPhase.Phase1 ? "Phase 1" : "Phase 2";
        }

        private void HandleBossDamaged(float damage)
        {
            RefreshHealth();
        }

        private void HandleBossDied()
        {
            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            if (hasVisibilityState && isVisible == visible)
                return;

            isVisible = visible;
            hasVisibilityState = true;

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private bool HasBossReferences()
        {
            return bossHealth != null && bossCombatController != null && bossController != null;
        }

        private bool HasViewReferences()
        {
            return canvasGroup != null && bossNameText != null && phaseText != null &&
                   healthSlider != null && healthText != null;
        }
    }
}