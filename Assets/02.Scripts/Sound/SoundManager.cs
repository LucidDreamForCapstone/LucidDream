using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
    #region private variable

    private static SoundManager _instance;
    [SerializeField] private AudioSource bgmSource; // BGM ����� ���� AudioSource
    [SerializeField] private AudioSource sfxSource_timeAffected; // SFX ����� ���� AudioSource
    [SerializeField] private AudioSource sfxSource_timeIgnore; // SFX ����� ���� AudioSource (�ð� ���� X)
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
        LoadSoundData();
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
        SaveSoundData();
    }

    public void SetBGMVolume(float volume) {
        m_AudioMixer.SetFloat("BGM", volume);
        SaveSoundData();
    }

    public void SetSFXVolume(float volume) {
        m_AudioMixer.SetFloat("SFX", volume);
        SaveSoundData();
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

            // Audio Mixer�� ���ο� Pitch �� ����
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

            // Audio Mixer�� ���ο� Pitch �� ����
            m_AudioMixer.SetFloat("BGMpitch", newPitch);
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

    private void SaveSoundData() {
        m_AudioMixer.GetFloat("Master", out float master);
        m_AudioMixer.GetFloat("BGM", out float bgm);
        m_AudioMixer.GetFloat("SFX", out float sfx);
        ES3File es3File = new ES3File("SoundData.es3");
        es3File.Save("masterVolume", master);
        es3File.Save("bgmVolume", bgm);
        es3File.Save("sfxVolume", sfx);
        es3File.Sync();
    }

    private void LoadSoundData() {
        float master, bgm, sfx;
        ES3File es3File = new ES3File("SoundData.es3");
        if (es3File.KeyExists("masterVolume"))
            master = es3File.Load<float>("masterVolume");
        else
            master = 0;
        if (es3File.KeyExists("bgmVolume"))
            bgm = es3File.Load<float>("bgmVolume");
        else
            bgm = 0;
        if (es3File.KeyExists("sfxVolume"))
            sfx = es3File.Load<float>("sfxVolume");
        else
            sfx = 0;

        m_AudioMixer.SetFloat("Master", master);
        m_AudioMixer.SetFloat("BGM", bgm);
        m_AudioMixer.SetFloat("SFX", sfx);
    }
    #endregion // private funcs
}


