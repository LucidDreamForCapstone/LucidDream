using DG.Tweening;
using TMPro;
using UnityEngine;

public class StartSceneManager : MonoBehaviour {
    [SerializeField] TextMeshProUGUI _fadingText;
    [SerializeField] AudioClip _mainBGM;
    private void Start() {
        _fadingText.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
        SoundManager.Instance.PlayBGM(_mainBGM.name);
    }

    public void GoToMain() {
        GameSceneManager.Instance.LoadMainScene();
    }
}
