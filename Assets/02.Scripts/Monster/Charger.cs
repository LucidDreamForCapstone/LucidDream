using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Charger : MonoBehaviour {
    [SerializeField] Image _chargeStateImage;
    [SerializeField] GameObject _shieldEffect;
    [SerializeField] int _maxHp;
    float _hp;
    Animator _animator;
    Animator _shieldAnimator;
    bool _isShieldActivated;
    BoxCollider2D _chargerCollider;

    private void Start() {
        _isShieldActivated = true;
        _chargerCollider = GetComponent<BoxCollider2D>();
        _chargerCollider.enabled = false;
        _animator = GetComponent<Animator>();
        _shieldAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            //shockwave
        }
    }

    public async UniTaskVoid RemoveShield() {
        _shieldAnimator.SetTrigger("Pop");
        _isShieldActivated = false;
        _chargerCollider.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        _shieldEffect.SetActive(false);
    }

    private void GenerateShield() {
        _shieldEffect.SetActive(true);
        _isShieldActivated = true;
        _chargerCollider.enabled = false;
    }



}
