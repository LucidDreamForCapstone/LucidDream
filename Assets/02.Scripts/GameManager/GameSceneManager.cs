using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour {
    #region private variable

    private static GameSceneManager _instance;

    #endregion // private variable





    #region properties

    public static GameSceneManager Instance { get { return _instance; } }

    #endregion // properties





    #region serializable Field

    [SerializeField] private string _startScene;
    [SerializeField] private string _mainScene;
    [SerializeField] private string _tutorialScene;
    [SerializeField] private List<string> _stageScenes;

    #endregion





    #region mono funcs

    void Start() {
        _instance = this;
    }

    #endregion // mono funcs





    #region public funcs

    public void LoadStartScene() {
        LoadingSceneManager.LoadSceneWithLoading(_startScene);
    }

    public void LoadMainScene() {
        SceneManager.LoadScene(_mainScene);
    }

    public void LoadTutorialScene() {
        LoadingSceneManager.LoadSceneWithLoading(_tutorialScene);
    }

    public void LoadStageScene(int stage) {
        if (null != _stageScenes[stage]) {
            LoadingSceneManager.LoadSceneWithLoading(_stageScenes[stage]);
        }
    }

    #endregion // public funcs
}
