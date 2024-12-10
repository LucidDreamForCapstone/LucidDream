using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DeathUIController : MonoBehaviour {
    [SerializeField] private Image _deadImage;      // �׾��� �� ������ �̹���
    [SerializeField] private TextMeshProUGUI _dyingMessage; // �׾��� �� ������ �޽���
    [SerializeField] private TextMeshProUGUI _dyingMessage1; // �׾��� �� ������ �޽���

    private void Start() {
        _deadImage.gameObject.SetActive(false);
    }

    public async UniTaskVoid ShowDeathUI() {
        // �÷��̾ ������ �̹����� �޽��� ���̵� ȿ�� ����
        _deadImage.DOFade(0, 0f).ToUniTask().Forget();
        _dyingMessage.DOFade(0, 0f).ToUniTask().Forget();
        _dyingMessage1.DOFade(0, 0f).ToUniTask().Forget();

        _deadImage.gameObject.SetActive(true);
        _deadImage.DOFade(1, 1.5f).ToUniTask().Forget();
        await _dyingMessage.DOFade(1, 1.5f);
        _dyingMessage1.DOFade(1, 0.5f).ToUniTask().Forget();
    }
}


