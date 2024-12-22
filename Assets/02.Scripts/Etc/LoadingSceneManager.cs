using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour {

    static string nextScene;
    [SerializeField] float _forceWaitTime;
    [SerializeField] List<string> _tipTextList;
    [SerializeField] TextMeshProUGUI _tipTM;

    private void Start() {
        LoadingScene().Forget();
    }

    private void Update() {
        if (Input.anyKeyDown) {
            SetRandomTip();
        }
    }

    public static void LoadSceneWithLoading(string sceneName) {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private async UniTaskVoid LoadingScene() {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        SetRandomTip();
        float timer = 0;
        while (!op.isDone) {
            timer += Time.deltaTime;
            if (timer > _forceWaitTime) {
                op.allowSceneActivation = true;
            }
            await UniTask.NextFrame();
        }
    }

    private void SetRandomTip() {
        int randomN = Random.Range(0, _tipTextList.Count);
        _tipTM.text = "Tip. " + _tipTextList[randomN];
    }
}
