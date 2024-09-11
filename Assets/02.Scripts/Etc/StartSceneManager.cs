using DG.Tweening;
using TMPro;
using UnityEngine;

public class StartSceneManager : MonoBehaviour {
    [SerializeField] TextMeshProUGUI _fadingText;

    private void Start() {
        _fadingText.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
    }
}
