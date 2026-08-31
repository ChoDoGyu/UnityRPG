using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace UnityRPG.Core
{
    [DisallowMultipleComponent]
    public sealed class AudioService : MonoBehaviour
    {
        private const string MasterVolumeParameter = "MasterVolume";
        private const string BgmVolumeParameter = "BGMVolume";
        private const string SfxVolumeParameter = "SFXVolume";

        public static AudioService Instance { get; private set; }

        [Header("Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("BGM")]
        [SerializeField] private AudioClip mainMenuBgm;
        [SerializeField] private AudioClip hubBgm;
        [SerializeField] private AudioClip dungeonBgm;
        [SerializeField, Min(0f)] private float bgmFadeDuration = 0.5f;

        private Coroutine bgmRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                enabled = false;
                return;
            }

            Instance = this;

            if (audioMixer == null || bgmSource == null || sfxSource == null)
                Debug.LogError("[Audio] Audio 설정이 필요합니다.", this);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void PlayBgm(AudioClip clip)
        {
            if (clip == null || bgmSource == null)
                return;

            if (bgmSource.clip == clip && bgmSource.isPlaying)
                return;

            if (bgmRoutine != null)
                StopCoroutine(bgmRoutine);

            bgmRoutine = StartCoroutine(ChangeBgmRoutine(clip));
        }

        public void StopBgm()
        {
            if (bgmSource == null)
                return;

            if (bgmRoutine != null)
            {
                StopCoroutine(bgmRoutine);
                bgmRoutine = null;
            }

            bgmSource.Stop();
            bgmSource.clip = null;
            bgmSource.volume = 1f;
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
                sfxSource.PlayOneShot(clip);
        }

        public void SetMasterVolume(float volume)
        {
            SetVolume(MasterVolumeParameter, volume);
        }

        public void SetBgmVolume(float volume)
        {
            SetVolume(BgmVolumeParameter, volume);
        }

        public void SetSfxVolume(float volume)
        {
            SetVolume(SfxVolumeParameter, volume);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            AudioClip clip = scene.name switch
            {
                SceneNames.MainMenu => mainMenuBgm,
                SceneNames.Hub => hubBgm,
                SceneNames.Dungeon => dungeonBgm,
                _ => null
            };

            if (clip != null)
                PlayBgm(clip);
        }

        private IEnumerator ChangeBgmRoutine(AudioClip clip)
        {
            if (bgmSource.isPlaying && bgmFadeDuration > 0f)
            {
                float startVolume = bgmSource.volume;
                float elapsed = 0f;

                while (elapsed < bgmFadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / bgmFadeDuration);
                    yield return null;
                }
            }

            bgmSource.clip = clip;
            bgmSource.volume = bgmFadeDuration > 0f ? 0f : 1f;
            bgmSource.Play();

            if (bgmFadeDuration > 0f)
            {
                float elapsed = 0f;

                while (elapsed < bgmFadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    bgmSource.volume = Mathf.Lerp(0f, 1f, elapsed / bgmFadeDuration);
                    yield return null;
                }
            }

            bgmSource.volume = 1f;
            bgmRoutine = null;
        }

        private void SetVolume(string parameter, float volume)
        {
            if (audioMixer == null)
                return;

            volume = Mathf.Clamp01(volume);
            float decibels = volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;

            audioMixer.SetFloat(parameter, decibels);
        }
    }
}