using UnityEngine;
using UnityEngine.UI;

public class OptionUIController : UIBase {
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
        // ���� �����̴��� �� ���� �� ������ ����
        SetVolumeSlider();
        AddSliderListeners();

        base.SetShow();
    }

    public override void SetHide() {
        // ���� ���� �� ������ ����
        SaveVolume();
        RemoveSliderListeners();

        base.SetHide();
    }

    public void SetMasterVolume() {
        _masterVolume = _masterVolumeSlider.value;
        if (_masterVolume < 0.002f)
            SoundManager.Instance.SetMasterVolume(-80);
        else
            SoundManager.Instance.SetMasterVolume(30 * (_masterVolume - 1));
    }

    public void SetBGMVolume() {
        _bgmVolume = _bgmVolumeSlider.value;
        if (_bgmVolume < 0.002f)
            SoundManager.Instance.SetBGMVolume(-80);
        else
            SoundManager.Instance.SetBGMVolume(30 * (_bgmVolume - 1));
    }

    public void SetSFXVolume() {
        _sfxVolume = _sfxVolumeSlider.value;
        if (_sfxVolume < 0.002f)
            SoundManager.Instance.SetSFXVolume(-80);
        else
            SoundManager.Instance.SetSFXVolume(30 * (_sfxVolume - 1));
    }

    #endregion // public funcs

    #region private funcs

    private void SetVolumeSlider() {
        // PlayerPrefs���� ����� ���� ���� ������ �����̴��� ����
        _masterVolumeSlider.value = PlayerPrefsManager.Instance.GetMasterVolume();
        _bgmVolumeSlider.value = PlayerPrefsManager.Instance.GetBGMVolume();
        _sfxVolumeSlider.value = PlayerPrefsManager.Instance.GetSFXVolume();
    }

    private void SaveVolume() {
        // ���� �����̴� ���� PlayerPrefs�� ����
        PlayerPrefsManager.Instance.SaveMasterVolume(_masterVolumeSlider.value);
        PlayerPrefsManager.Instance.SaveBGMVolume(_bgmVolumeSlider.value);
        PlayerPrefsManager.Instance.SaveSFXVolume(_sfxVolumeSlider.value);
    }

    private void AddSliderListeners() {
        // �����̴� ���� ����� ������ ����Ǵ� ������ �߰�
        _masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        _bgmVolumeSlider.onValueChanged.AddListener(delegate { SetBGMVolume(); });
        _sfxVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
    }

    private void RemoveSliderListeners() {
        // �����̴� �����ʸ� �����Ͽ� ���ʿ��� �̺�Ʈ ȣ�� ����
        _masterVolumeSlider.onValueChanged.RemoveAllListeners();
        _bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        _sfxVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    #endregion // private funcs
}

