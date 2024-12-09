using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour {
    [SerializeField] private TMP_Text _text;
    [SerializeField] TMP_FontAsset _noCritFont;
    [SerializeField] TMP_FontAsset _critFont;
    [SerializeField] TMP_FontAsset _damagedFont;
    [SerializeField] TMP_FontAsset _healFont;
    [SerializeField] TMP_FontAsset _poisonFont;
    [SerializeField] float _lastTime;



    public void ShowDamage(int damage, bool isMine, bool isCrit, bool isHeal, bool isPoison) {
        if (isMine) {
            if (isHeal)
                _text.font = _healFont;
            else
                _text.font = _damagedFont;
        }
        else {
            if (isCrit) {
                _text.font = _critFont;
                gameObject.transform.localScale = Vector3.one * 1.5f;
            }

            else
                _text.font = _noCritFont;
        }

        if (isPoison) {
            _text.font = _poisonFont;
        }

        _text.text = damage.ToString();

    }


    private void Start() {
        _text.DOFade(0, _lastTime);
    }

    void Update() {
        transform.Translate(new Vector3(0, 100 * Time.unscaledDeltaTime, 0));
        _lastTime -= Time.unscaledDeltaTime;
        if (_lastTime <= 0) {
            Destroy(gameObject);
            //ObjectPool.Instance.ReturnObject(gameObject);
        }
    }
}
