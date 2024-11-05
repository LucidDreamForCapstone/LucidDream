using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
    #region private variable

    private static SoundManager _instance;
    [SerializeField] private AudioSource bgmSource; // BGM 재생을 위한 AudioSource
    [SerializeField] private AudioSource sfxSource_timeAffected; // SFX 재생을 위한 AudioSource
    [SerializeField] private AudioSource sfxSource_timeIgnore; // SFX 재생을 위한 AudioSource (시간 영향 X)
    [SerializeField] private AudioMixer m_AudioMixer;
    #endregion // private variable

    #region properties

    public static SoundManager Instance { get { return _instance; } }

    #endregion // properties

    #region mono funcs

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        SetVolume();
    }

    #endregion // mono funcs

    #region public funcs

    public void PlaySFX(string clipName, bool isTimeIgnore = false) {
        // Ensure the SFX folder exists within the Resources folder.
        AudioClip clip = Resources.Load<AudioClip>($"SFX/{clipName}");
        if (clip != null) {
            if (!isTimeIgnore)
                sfxSource_timeAffected.PlayOneShot(clip);
            else
                sfxSource_timeIgnore.PlayOneShot(clip);
        }
        else {
            Debug.LogWarning($"SFX '{clipName}' not found in Resources/SFX/");
        }
    }

    public void PlayBGM(string clipName) {
        AudioClip clip = Resources.Load<AudioClip>($"BGM/{clipName}");
        if (clip != null) {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else {
            Debug.LogWarning($"BGM '{clipName}' not found in Resources/BGM/");
        }
    }



    public void SetMasterVolume(float volume) {
        m_AudioMixer.SetFloat("Master", volume);
    }

    public void SetBGMVolume(float volume) {
        m_AudioMixer.SetFloat("BGM", volume);
    }

    public void SetSFXVolume(float volume) {
        m_AudioMixer.SetFloat("SFX", volume);
    }

    public void SetSFXPitch(float pitch) {
        m_AudioMixer.SetFloat("SFXpitch", pitch);
    }

    public async UniTaskVoid SetSFXPitchLerp(float pitch, float lerpTime) {
        float t, newPitch, timer = 0;
        m_AudioMixer.GetFloat("SFXpitch", out float startPitch);
        while (timer < lerpTime) {
            timer += Time.unscaledDeltaTime;
            t = timer / lerpTime;
            newPitch = Mathf.Lerp(startPitch, pitch, t);

            // Audio Mixer에 새로운 Pitch 값 설정
            m_AudioMixer.SetFloat("SFXpitch", newPitch);
            await UniTask.NextFrame();
        }
    }
    public async UniTaskVoid SetBGMPitchLerp(float pitch, float lerpTime) {
        float t, newPitch, timer = 0;
        m_AudioMixer.GetFloat("BGMpitch", out float startPitch);
        while (timer < lerpTime) {
            timer += Time.unscaledDeltaTime;
            t = timer / lerpTime;
            newPitch = Mathf.Lerp(startPitch, pitch, t);

            // Audio Mixer에 새로운 Pitch 값 설정
            m_AudioMixer.SetFloat("BGMpitch", newPitch);
            await UniTask.NextFrame();
        }
    }

    public async UniTaskVoid SetBGMVolumeLerp(float percent, float lerpTime) {
        float t, newVolume, timer = 0;
        m_AudioMixer.GetFloat("BGM", out float startVolume);
        while (timer < lerpTime) {
            timer += Time.unscaledDeltaTime;
            t = timer / lerpTime;
            newVolume = Mathf.Lerp(startVolume, percent * startVolume, t);

            // Audio Mixer에 새로운 Volume 값 설정
            m_AudioMixer.SetFloat("BGM", newVolume);
            await UniTask.NextFrame();
        }
    }

    public void PauseSFX() {
        sfxSource_timeAffected.Pause();
        sfxSource_timeIgnore.Pause();
    }
    public void UnPauseSFX() {
        sfxSource_timeAffected.UnPause();
        sfxSource_timeIgnore.UnPause();
    }

    #endregion // public funcs

    #region private funcs

    private void SetVolume() {
        //m_AudioMixer.SetFloat("Master", Mathf.Log10(PlayerPrefsManager.Instance.GetMasterVolume()) * 20);
        //m_AudioMixer.SetFloat("BGM", Mathf.Log10(PlayerPrefsManager.Instance.GetBGMVolume()) * 20);
        //m_AudioMixer.SetFloat("SFX", Mathf.Log10(PlayerPrefsManager.Instance.GetSFXVolume()) * 20);
        m_AudioMixer.SetFloat("Master", -0.08f); //Temp setting. Use upper code when save implementation is done.
        m_AudioMixer.SetFloat("SFXpitch", 1);
    }

    #endregion // private funcs
}


