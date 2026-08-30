using UnityEngine;
using UnityRPG.Core;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioService))]
    public sealed class UISfxService : MonoBehaviour
    {
        public static UISfxService Instance { get; private set; }

        [Header("SFX")]
        [SerializeField] private AudioClip clickSfx;
        [SerializeField] private AudioClip openSfx;
        [SerializeField] private AudioClip closeSfx;
        [SerializeField] private AudioClip equipSfx;
        [SerializeField] private AudioClip unequipSfx;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                enabled = false;
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void PlayClick()
        {
            Play(clickSfx);
        }

        public void PlayOpen()
        {
            Play(openSfx);
        }

        public void PlayClose()
        {
            Play(closeSfx);
        }

        public void PlayEquip()
        {
            Play(equipSfx);
        }

        public void PlayUnequip()
        {
            Play(unequipSfx);
        }

        private void Play(AudioClip clip)
        {
            if (clip != null)
                AudioService.Instance?.PlaySfx(clip);
        }
    }
}