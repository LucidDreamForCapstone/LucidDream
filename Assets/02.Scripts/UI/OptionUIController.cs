/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionUIController : UIBase
{
    #region serializable field

    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    #endregion // serializable field





    #region private variables

    private float _masterVolume;
    private float _bgmVolume;
    private float _sfxVolume;

    #endregion // private variables





    #region public funcs

    public override void SetShow() {
        SetVolumeSlider();

        base.SetShow();
    }

    public override void SetHide() {
        SaveVolume();

        base.SetHide();
    }

    public void SetMasterVolume() {
        _masterVolume = _masterVolumeSlider.value;
        SoundManager.Instance.SetMasterVolume(_masterVolumeSlider.value);
    }

    public void SetBGMVolume() {
        _bgmVolume = _bgmVolumeSlider.value;
        SoundManager.Instance.SetMusicVolume(_bgmVolumeSlider.value);
    }

    public void SetSFXVolume() {
        _sfxVolume = _sfxVolumeSlider.value;
        SoundManager.Instance.SetSFXVolume(_sfxVolumeSlider.value);
    }

    #endregion // public funcs





    #region private funcs

    private void SetVolumeSlider() {
        _masterVolumeSlider.value = PlayerPrefsManager.Instance.GetMasterVolume();
        _bgmVolumeSlider.value = PlayerPrefsManager.Instance.GetBGMVolume();
        _sfxVolumeSlider.value = PlayerPrefsManager.Instance.GetSFXVolume();
    }

    private void SaveVolume() {
        PlayerPrefsManager.Instance.SaveMasterVolume(_masterVolume);
        PlayerPrefsManager.Instance.SaveBGMVolume(_bgmVolume);
        PlayerPrefsManager.Instance.SaveSFXVolume(_sfxVolume);
    }

    #endregion // private funcs
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUIController : UIBase
{
    #region serializable field

    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    #endregion // serializable field

    #region private variables

    private float _masterVolume;
    private float _bgmVolume;
    private float _sfxVolume;

    #endregion // private variables

    #region public funcs

    public override void SetShow()
    {
        // 볼륨 슬라이더의 값 설정 및 리스너 연결
        SetVolumeSlider();
        AddSliderListeners();

        base.SetShow();
    }

    public override void SetHide()
    {
        // 볼륨 저장 및 리스너 해제
        SaveVolume();
        RemoveSliderListeners();

        base.SetHide();
    }

    public void SetMasterVolume()
    {
        _masterVolume = _masterVolumeSlider.value;
        SoundManager.Instance.SetMasterVolume(_masterVolume); // 볼륨 설정
    }

    public void SetBGMVolume()
    {
        _bgmVolume = _bgmVolumeSlider.value;
        SoundManager.Instance.SetMusicVolume(_bgmVolume); // 볼륨 설정
    }

    public void SetSFXVolume()
    {
        _sfxVolume = _sfxVolumeSlider.value;
        SoundManager.Instance.SetSFXVolume(_sfxVolume); // 볼륨 설정
    }

    #endregion // public funcs

    #region private funcs

    private void SetVolumeSlider()
    {
        // PlayerPrefs에서 저장된 볼륨 값을 가져와 슬라이더에 설정
        _masterVolumeSlider.value = PlayerPrefsManager.Instance.GetMasterVolume();
        _bgmVolumeSlider.value = PlayerPrefsManager.Instance.GetBGMVolume();
        _sfxVolumeSlider.value = PlayerPrefsManager.Instance.GetSFXVolume();
    }

    private void SaveVolume()
    {
        // 현재 슬라이더 값을 PlayerPrefs에 저장
        PlayerPrefsManager.Instance.SaveMasterVolume(_masterVolumeSlider.value);
        PlayerPrefsManager.Instance.SaveBGMVolume(_bgmVolumeSlider.value);
        PlayerPrefsManager.Instance.SaveSFXVolume(_sfxVolumeSlider.value);
    }

    private void AddSliderListeners()
    {
        // 슬라이더 값이 변경될 때마다 실행되는 리스너 추가
        _masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        _bgmVolumeSlider.onValueChanged.AddListener(delegate { SetBGMVolume(); });
        _sfxVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
    }

    private void RemoveSliderListeners()
    {
        // 슬라이더 리스너를 제거하여 불필요한 이벤트 호출 방지
        _masterVolumeSlider.onValueChanged.RemoveAllListeners();
        _bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        _sfxVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    #endregion // private funcs
}

