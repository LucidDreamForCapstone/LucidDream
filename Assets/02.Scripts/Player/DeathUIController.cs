using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DeathUIController : MonoBehaviour {
    [SerializeField] private Image _deadImage;      // 죽었을 때 나오는 이미지
    [SerializeField] private TextMeshProUGUI _dyingMessage; // 죽었을 때 나오는 메시지
    [SerializeField] private TextMeshProUGUI _dyingMessage1; // 죽었을 때 나오는 메시지

    private void Start() {
        _deadImage.gameObject.SetActive(false);
    }

    public async UniTaskVoid ShowDeathUI() {
        // 플레이어가 죽으면 이미지와 메시지 페이드 효과 시작
        _deadImage.DOFade(0, 0f).ToUniTask().Forget();
        _dyingMessage.DOFade(0, 0f).ToUniTask().Forget();
        _dyingMessage1.DOFade(0, 0f).ToUniTask().Forget();

        _deadImage.gameObject.SetActive(true);
        _deadImage.DOFade(1, 1.5f).ToUniTask().Forget();
        await _dyingMessage.DOFade(1, 1.5f);
        _dyingMessage1.DOFade(1, 0.5f).ToUniTask().Forget();
    }
}


