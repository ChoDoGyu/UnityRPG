using UnityEngine;

namespace UnityRPG.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyContext))]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class BossCombatController : MonoBehaviour
    {
        [Header("Phase")]
        [SerializeField]
        [Range(0.01f, 0.99f)]
        private float phase2HealthRatio = 0.5f;

        [Header("Pattern Interval")]
        [SerializeField]
        [Min(0f)]
        private float patternInterval = 0.6f;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float phase2IntervalMultiplier = 0.85f;

        [Header("Runtime")]
        [SerializeField]
        private BossPhase currentPhase = BossPhase.Phase1;

        private EnemyHealth enemyHealth;

        private BossPatternBase[] patterns;
        private BossPatternBase[] candidateBuffer;

        private BossPatternBase activePattern;
        private BossPatternBase lastPattern;

        private float patternIntervalRemaining;
        private bool isConfigured;

        public BossPhase CurrentPhase => currentPhase;

        public BossPatternType CurrentPattern =>
            activePattern != null ? activePattern.Type : BossPatternType.None;

        public bool HasActivePattern =>
            activePattern != null;

        public bool ShouldTrackTargetRotation =>
            activePattern == null ||
            activePattern.ShouldTrackTargetRotation;

        public bool ShouldStopMotor =>
            activePattern == null ||
            activePattern.ShouldStopMotor;

        public BossPatternPhase CurrentPatternPhase =>
            activePattern != null ? activePattern.CurrentPhase : BossPatternPhase.Ready;

        public float CurrentPatternProgress =>
            activePattern != null ? activePattern.PhaseNormalizedProgress : 0f;

        public bool IsPatternIntervalActive => patternIntervalRemaining > 0f;

        private void Awake()
        {
            EnemyContext context = GetComponent<EnemyContext>();
            enemyHealth = GetComponent<EnemyHealth>();

            patterns = GetComponents<BossPatternBase>();

            if (!context.IsConfigured ||
                patterns.Length == 0)
            {
                Debug.LogError(
                    "[Boss] BossCombatController의 설정이 올바르지 않습니다.",
                    this);

                return;
            }

            candidateBuffer =
                new BossPatternBase[patterns.Length];

            currentPhase = BossPhase.Phase1;

            isConfigured = true;
        }

        public void UpdateCombat(float deltaTime)
        {
            if (!isConfigured || deltaTime <= 0f)
            {
                return;
            }

            UpdateBossPhase();

            for (int i = 0; i < patterns.Length; i++)
            {
                patterns[i].UpdatePattern(deltaTime);
            }

            UpdatePatternInterval(deltaTime);
        }

        public bool TryStartPattern(Transform target)
        {
            if (!isConfigured ||
                target == null ||
                HasActivePattern ||
                patternIntervalRemaining > 0f)
            {
                return false;
            }

            int candidateCount =
                CollectCandidates(target, true);

            if (candidateCount == 0)
            {
                candidateCount =
                    CollectCandidates(target, false);
            }

            if (candidateCount == 0)
            {
                return false;
            }

            BossPatternBase selectedPattern =
                candidateBuffer[
                    Random.Range(0, candidateCount)];

            if (!selectedPattern.TryStartPattern(target))
            {
                return false;
            }

            lastPattern = selectedPattern;

            return true;
        }

        public bool TryBeginPattern(BossPatternBase pattern)
        {
            if (!isConfigured ||
                pattern == null ||
                HasActivePattern ||
                patternIntervalRemaining > 0f)
            {
                return false;
            }

            activePattern = pattern;

            return true;
        }

        public void FinishPattern(BossPatternBase pattern)
        {
            if (!isConfigured ||
                activePattern != pattern)
            {
                return;
            }

            activePattern = null;

            patternIntervalRemaining =
                GetCurrentPatternInterval();
        }

        private int CollectCandidates(
            Transform target,
            bool excludeLastPattern)
        {
            int count = 0;

            for (int i = 0; i < patterns.Length; i++)
            {
                BossPatternBase pattern = patterns[i];

                if (!pattern.CanBeSelected(
                        target,
                        currentPhase))
                {
                    continue;
                }

                if (excludeLastPattern &&
                    pattern == lastPattern)
                {
                    continue;
                }

                candidateBuffer[count] = pattern;
                count++;
            }

            return count;
        }

        private void UpdateBossPhase()
        {
            if (currentPhase == BossPhase.Phase2 ||
                enemyHealth.MaxHealth <= 0f)
            {
                return;
            }

            float healthRatio =
                enemyHealth.CurrentHealth /
                enemyHealth.MaxHealth;

            if (healthRatio <= phase2HealthRatio)
            {
                currentPhase = BossPhase.Phase2;
            }
        }

        private void UpdatePatternInterval(float deltaTime)
        {
            if (patternIntervalRemaining <= 0f)
            {
                return;
            }

            patternIntervalRemaining = Mathf.Max(
                0f,
                patternIntervalRemaining - deltaTime);
        }

        private float GetCurrentPatternInterval()
        {
            if (currentPhase == BossPhase.Phase2)
            {
                return patternInterval *
                       phase2IntervalMultiplier;
            }

            return patternInterval;
        }

        private void OnValidate()
        {
            phase2HealthRatio = Mathf.Clamp(
                phase2HealthRatio,
                0.01f,
                0.99f);

            patternInterval = Mathf.Max(
                0f,
                patternInterval);

            phase2IntervalMultiplier = Mathf.Clamp(
                phase2IntervalMultiplier,
                0.1f,
                1f);
        }
    }
}