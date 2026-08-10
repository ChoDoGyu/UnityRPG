using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    public sealed class EnemyContext :
        MonoBehaviour
    {
        [Header("Definition")]
        [SerializeField]
        private EnemyDefinition definition;

        public EnemyDefinition Definition =>
            definition;

        public bool IsConfigured =>
            definition != null;

        private void Awake()
        {
            if (definition != null)
            {
                return;
            }

            Debug.LogError(
                "[Enemy] EnemyContext의 Enemy Definition이 설정되지 않았습니다.",
                this);
        }
    }
}