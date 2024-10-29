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
        // ���� �����̴��� �� ���� �� ������ ����
        SetVolumeSlider();
        AddSliderListeners();

        base.SetShow();
    }

    public override void SetHide()
    {
        // ���� ���� �� ������ ����
        SaveVolume();
        RemoveSliderListeners();

        base.SetHide();
    }

    public void SetMasterVolume()
    {
        _masterVolume = _masterVolumeSlider.value;
        SoundManager.Instance.SetMasterVolume(_masterVolume); // ���� ����
    }

    public void SetBGMVolume()
    {
        _bgmVolume = _bgmVolumeSlider.value;
        SoundManager.Instance.SetMusicVolume(_bgmVolume); // ���� ����
    }

    public void SetSFXVolume()
    {
        _sfxVolume = _sfxVolumeSlider.value;
        SoundManager.Instance.SetSFXVolume(_sfxVolume); // ���� ����
    }

    #endregion // public funcs

    #region private funcs

    private void SetVolumeSlider()
    {
        // PlayerPrefs���� ����� ���� ���� ������ �����̴��� ����
        _masterVolumeSlider.value = PlayerPrefsManager.Instance.GetMasterVolume();
        _bgmVolumeSlider.value = PlayerPrefsManager.Instance.GetBGMVolume();
        _sfxVolumeSlider.value = PlayerPrefsManager.Instance.GetSFXVolume();
    }

    private void SaveVolume()
    {
        // ���� �����̴� ���� PlayerPrefs�� ����
        PlayerPrefsManager.Instance.SaveMasterVolume(_masterVolumeSlider.value);
        PlayerPrefsManager.Instance.SaveBGMVolume(_bgmVolumeSlider.value);
        PlayerPrefsManager.Instance.SaveSFXVolume(_sfxVolumeSlider.value);
    }

    private void AddSliderListeners()
    {
        // �����̴� ���� ����� ������ ����Ǵ� ������ �߰�
        _masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        _bgmVolumeSlider.onValueChanged.AddListener(delegate { SetBGMVolume(); });
        _sfxVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
    }

    private void RemoveSliderListeners()
    {
        // �����̴� �����ʸ� �����Ͽ� ���ʿ��� �̺�Ʈ ȣ�� ����
        _masterVolumeSlider.onValueChanged.RemoveAllListeners();
        _bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        _sfxVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    #endregion // private funcs
}

