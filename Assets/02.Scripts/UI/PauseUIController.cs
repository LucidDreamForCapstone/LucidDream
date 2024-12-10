using UnityEngine;

public class PauseUIController : MonoBehaviour {
    [SerializeField] GameObject _pauseUI;
    [SerializeField] OptionUIController _optionUIController;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            if (PlayerDataManager.Instance.Status._hp > 0) {
                if (!_pauseUI.activeSelf)
                    GamePause();
                else
                    GameResume();
            }
        }
    }

    public void GamePause() {
        TimeScaleManager.Instance.TimeStop();
        _pauseUI.SetActive(true);
    }

    public void GameResume() {
        TimeScaleManager.Instance.TimeRestore();
        _pauseUI.SetActive(false);
        _optionUIController.gameObject.SetActive(false);
    }
    public void OnClick_Option() {
        _optionUIController.SetShow();
    }

    public void GoToMain() {
        GameManager.Instance.InitializeSetting();
        GameSceneManager.Instance.LoadStartScene();
    }

    public void GameExit() {
        GameManager.Instance.GameQuit();
    }
}
