using UnityEngine;
using UnityRPG.AI;
using UnityRPG.Character.Player;

namespace UnityRPG.VFX
{
    [DisallowMultipleComponent]
    public sealed class DamageFlashFeedback : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] private Renderer[] targetRenderers;

        [Header("Flash")]
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField, Min(0.01f)] private float duration = 0.1f;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private PlayerHealth playerHealth;
        private EnemyHealth enemyHealth;

        private MaterialPropertyBlock propertyBlock;
        private Color[] originalColors;

        private float remainingTime;
        private bool isFlashing;

        private void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
            enemyHealth = GetComponent<EnemyHealth>();

            if ((playerHealth == null && enemyHealth == null) || targetRenderers == null || targetRenderers.Length == 0)
            {
                Debug.LogError("[VFX] DamageFlashFeedback의 Health 또는 Renderer 설정이 올바르지 않습니다.", this);
                enabled = false;
                return;
            }

            propertyBlock = new MaterialPropertyBlock();
            originalColors = new Color[targetRenderers.Length];

            for (int i = 0; i < targetRenderers.Length; i++)
            {
                Renderer targetRenderer = targetRenderers[i];

                if (targetRenderer == null || targetRenderer.sharedMaterial == null)
                    continue;

                if (targetRenderer.sharedMaterial.HasProperty(BaseColorId))
                    originalColors[i] = targetRenderer.sharedMaterial.GetColor(BaseColorId);
            }
        }

        private void OnEnable()
        {
            if (playerHealth != null)
                playerHealth.Damaged += HandleDamaged;

            if (enemyHealth != null)
                enemyHealth.Damaged += HandleDamaged;
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.Damaged -= HandleDamaged;

            if (enemyHealth != null)
                enemyHealth.Damaged -= HandleDamaged;

            RestoreColor();
        }

        private void Update()
        {
            if (!isFlashing)
                return;

            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0f)
                RestoreColor();
        }

        private void HandleDamaged(float damage)
        {
            remainingTime = duration;
            isFlashing = true;

            SetColor(flashColor);
        }

        private void SetColor(Color color)
        {
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                Renderer targetRenderer = targetRenderers[i];

                if (targetRenderer == null)
                    continue;

                propertyBlock.Clear();
                targetRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(BaseColorId, color);
                targetRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        private void RestoreColor()
        {
            if (!isFlashing || propertyBlock == null)
                return;

            for (int i = 0; i < targetRenderers.Length; i++)
            {
                Renderer targetRenderer = targetRenderers[i];

                if (targetRenderer == null)
                    continue;

                propertyBlock.Clear();
                targetRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(BaseColorId, originalColors[i]);
                targetRenderer.SetPropertyBlock(propertyBlock);
            }

            remainingTime = 0f;
            isFlashing = false;
        }
    }
}