using UnityEngine;
using UnityEngine.UI;

public class OptionUIController : UIBase {
    #region serializable field

    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Slider _brightnessSlider;

    #endregion // serializable field

    #region private variables

    private float _masterVolume;
    private float _bgmVolume;
    private float _sfxVolume;
    private float _brightness;

    #endregion // private variables

    #region public funcs

    public override void SetShow() {
        // ���� �����̴��� �� ���� �� ������ ����
        LoadDataToSlider();
        AddSliderListeners();

        base.SetShow();
    }

    public override void SetHide() {
        //������ ����
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

    public void SetBrightness() {
        _brightness = _brightnessSlider.value;
        OptionManager.Instance.SetBrightness(_brightness);
    }

    #endregion // public funcs

    #region private funcs

    private void LoadDataToSlider() {
        _masterVolumeSlider.value = VolumeNormalize(SoundManager.Instance.GetMasterVolume());
        _bgmVolumeSlider.value = VolumeNormalize(SoundManager.Instance.GetBGMVolume());
        _sfxVolumeSlider.value = VolumeNormalize(SoundManager.Instance.GetSFXVolume());
        _brightnessSlider.value = OptionManager.Instance.GetBrightness();
    }

    private float VolumeNormalize(float volume) {
        return volume / 30 + 1;
    }

    private void AddSliderListeners() {
        // �����̴� ���� ����� ������ ����Ǵ� ������ �߰�
        _masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        _bgmVolumeSlider.onValueChanged.AddListener(delegate { SetBGMVolume(); });
        _sfxVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
        _brightnessSlider.onValueChanged.AddListener(delegate { SetBrightness(); });

    }

    private void RemoveSliderListeners() {
        // �����̴� �����ʸ� �����Ͽ� ���ʿ��� �̺�Ʈ ȣ�� ����
        _masterVolumeSlider.onValueChanged.RemoveAllListeners();
        _bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        _sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        _brightnessSlider.onValueChanged.RemoveAllListeners();
    }

    #endregion // private funcs
}

