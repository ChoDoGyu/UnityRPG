using UnityEngine;
using UnityEngine.UI;
using UnityRPG.Core;

namespace UnityRPG.UI
{
    [DisallowMultipleComponent]
    public sealed class AudioSettingsUI : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        private void Start()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[UI] AudioSettingsUI의 참조가 누락되었습니다.", this);
                enabled = false;
                return;
            }

            if (AudioService.Instance == null)
            {
                Debug.LogError("[UI] AudioService를 찾을 수 없습니다.", this);
                enabled = false;
                return;
            }

            RefreshCurrentValues();

            masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChanged);
            bgmVolumeSlider.onValueChanged.AddListener(HandleBgmVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolumeChanged);
        }

        private void OnDestroy()
        {
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.RemoveListener(HandleMasterVolumeChanged);

            if (bgmVolumeSlider != null)
                bgmVolumeSlider.onValueChanged.RemoveListener(HandleBgmVolumeChanged);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.RemoveListener(HandleSfxVolumeChanged);
        }

        private void RefreshCurrentValues()
        {
            masterVolumeSlider.SetValueWithoutNotify(AudioService.Instance.MasterVolume);
            bgmVolumeSlider.SetValueWithoutNotify(AudioService.Instance.BgmVolume);
            sfxVolumeSlider.SetValueWithoutNotify(AudioService.Instance.SfxVolume);
        }

        private void HandleMasterVolumeChanged(float value)
        {
            AudioService.Instance?.SetMasterVolume(value);
        }

        private void HandleBgmVolumeChanged(float value)
        {
            AudioService.Instance?.SetBgmVolume(value);
        }

        private void HandleSfxVolumeChanged(float value)
        {
            AudioService.Instance?.SetSfxVolume(value);
        }

        private bool HasAllReferences()
        {
            return masterVolumeSlider != null &&
                   bgmVolumeSlider != null &&
                   sfxVolumeSlider != null;
        }
    }
}