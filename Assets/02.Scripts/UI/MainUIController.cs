using UnityEngine;

public class MainUIController : UIBase {
    #region serializable field

    [SerializeField] private OptionUIController _optionUIController;
    #endregion //serializable field


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GameManager.Instance.GameQuit();
        }
    }

    #region public funcs

    public void OnClick_GameStart() {
        if (GameManager.Instance.CheckFirstPlay()) {
            GameSceneManager.Instance.LoadTutorialScene();
        }
        else {
            GameSceneManager.Instance.LoadStageScene(0);
        }
    }

    public void OnClick_Option() {
        _optionUIController.SetShow();
    }

    #endregion // public funcs
}
