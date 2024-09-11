using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    #region private variable

    private static SoundManager _instance;
    [SerializeField] private AudioSource sfxSource; // SFX 재생을 위한 AudioSource
    [SerializeField] private AudioMixer m_AudioMixer;

    #endregion // private variable

    #region properties

    public static SoundManager Instance { get { return _instance; } }

    #endregion // properties

    #region mono funcs

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Ensure sfxSource is assigned
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups("SFX")[0];
        }
    }

    void Start()
    {
        SetVolume();
    }

    #endregion // mono funcs

    #region public funcs

    public void PlaySFX(string clipName)
    {
        // Ensure the SFX folder exists within the Resources folder.
        AudioClip clip = Resources.Load<AudioClip>($"SFX/{clipName}");
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{clipName}' not found in Resources/SFX/");
        }
    }

    public void SetMasterVolume(float volume)
    {
        m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        m_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        m_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }

    #endregion // public funcs

    #region private funcs

    private void SetVolume()
    {
        m_AudioMixer.SetFloat("Master", Mathf.Log10(PlayerPrefsManager.Instance.GetMasterVolume()) * 20);
        m_AudioMixer.SetFloat("BGM", Mathf.Log10(PlayerPrefsManager.Instance.GetBGMVolume()) * 20);
        m_AudioMixer.SetFloat("SFX", Mathf.Log10(PlayerPrefsManager.Instance.GetSFXVolume()) * 20);
    }

    #endregion // private funcs
}


