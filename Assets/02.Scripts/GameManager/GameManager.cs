using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void InitializeSetting() {
        TimeScaleManager.Instance.ClearTimeStack();
        SoundManager.Instance.SetBGMPitch(1);
        SoundManager.Instance.SetSFXPitch(1);
    }

    public void GameQuit() {
        Application.Quit();
    }

    public void SaveFlagData() {
        ES3File es3File = new ES3File("GameSaveData.es3");
        es3File.Save("isFirst", false);
        es3File.Sync();
    }

    public bool CheckFirstPlay() {
        ES3File es3File = new ES3File("GameSaveData.es3");
        if (es3File.KeyExists("isFirst")) {
            return false;
        }
        else {
            return true;
        }
    }

}
