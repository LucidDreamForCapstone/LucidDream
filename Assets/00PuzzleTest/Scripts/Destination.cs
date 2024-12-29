using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Destination : MonoBehaviour {
    [SerializeField]
    private int TargetCount = 0;
    [SerializeField]
    private int destCount;
    [SerializeField]
    private TextMeshProUGUI _hintTM;

    public int DestCount { get => destCount; set => destCount = value; }
    public int TargetCount1 { get => TargetCount; }
    void Start() {
        destCount = TargetCount;
        _hintTM.text = destCount.ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            TextFade();
        }
    }

    private async void TextFade() {
        _hintTM.color = new Color(1, 1, 1, 0);
        await _hintTM.DOFade(1f, 1);
        await _hintTM.DOFade(0, 1);
    }
}
