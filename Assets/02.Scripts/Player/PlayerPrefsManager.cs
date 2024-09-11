using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {
    #region private variable

    private static PlayerPrefsManager _instance;

    private static string _stage = "Stage";
    private static string _masterVolume = "MasterVolume";
    private static string _bgmVolume = "BGMVolume";
    private static string _sfxVolume = "SFXVolume";

    #endregion // private variable





    #region properties

    public static PlayerPrefsManager Instance { get { return _instance; } }

    #endregion // properties





    #region mono funcs

    void Awake() {
        _instance = this;
    }

    #endregion // mono funcs





    #region public funcs

    public void SaveAllData() {
        SaveStage(PlayerDataManager.Instance.Stage);
    }

    public void DeleteAllData() {
        //PopupManager.Instance.ShowOKCancelPopup("진짜로 삭제할겨?" ,() = >{ DeleteAll(); } , null);
        DeleteAll();
    }

    public void SaveMasterVolume(float volume) {
        PlayerPrefs.SetFloat(_masterVolume, volume);
    }

    public float GetMasterVolume() {
        float volume = PlayerPrefs.GetFloat(_masterVolume);
        return volume;
    }

    public void SaveBGMVolume(float volume) {
        PlayerPrefs.SetFloat(_bgmVolume, volume);
    }

    public float GetBGMVolume() {
        return PlayerPrefs.GetFloat(_bgmVolume);
    }

    public void SaveSFXVolume(float volume) {
        PlayerPrefs.SetFloat(_sfxVolume, volume);
    }

    public float GetSFXVolume() {
        return PlayerPrefs.GetFloat(_sfxVolume);
    }

    #endregion // public funcs





    #region private funcs

    private void SaveStage(int stage) {
        PlayerPrefs.SetInt(_stage, stage);
    }

    private void DeleteAll() {
        PlayerPrefs.DeleteAll();
    }

    #endregion //private funcs
}
