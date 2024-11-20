using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Charger : MonsterBase {
    [SerializeField] int _chargerIndex;
    [SerializeField] int _hpRecoverAmount;
    [SerializeField] BossBondrewd _boss;
    [SerializeField] Slider _hpSlider;
    CapsuleCollider2D _chargerCollider;
    CircleCollider2D _shieldCollider;
    SpriteRenderer _shieldSr;
    Animator _shieldAnimator;
    bool _isShieldActivated;

    private void Start() {
        _chargerCollider = GetComponent<CapsuleCollider2D>();
        _shieldCollider = GetComponent<CircleCollider2D>();
        _shieldAnimator = transform.GetChild(0).GetComponent<Animator>();
        _shieldSr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        InitializeCharger(_chargerIndex + 1).Forget();
        UpdateHpSlider();
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            //shockwave
        }
    }

    public override void Damaged(int dmg, bool isCrit) {
        if (_isShieldActivated) {
            dmg = 0;
        }
        FloatingDamageManager.Instance.ShowDamage(this.gameObject, dmg, false, isCrit, false);
        if (_hp <= dmg) {
            Die().Forget();
        }
        else {
            ChangeColor().Forget();
            _hp -= dmg;
            UpdateHpSlider();
            PlayRandomSound();
        }
    }

    protected async override UniTaskVoid ChangeColor() {
        if (!_isColorChanged) {
            _isColorChanged = true;
            Color32 originColor = _spriteRenderer.color;
            if (_isShieldActivated)
                _shieldAnimator.SetTrigger("Damaged");
            else
                _spriteRenderer.color = _damagedColor;
            await UniTask.Delay(TimeSpan.FromSeconds(_colorChanageLastTime));
            _spriteRenderer.color = originColor;
            _shieldSr.color = originColor;
            _isColorChanged = false;
        }
    }

    protected async override UniTaskVoid Die() {
        _hp = 0;
        UpdateHpSlider();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        ChargerOff();
        GenerateShield();
    }

    public void CallBackRemoveShield() {
        RemoveShield().Forget();
    }

    private async UniTaskVoid RemoveShield() { //after called by lab
        _shieldAnimator.SetTrigger("Pop");
        await UniTask.Delay(TimeSpan.FromSeconds(0.8f));
        _shieldSr.gameObject.SetActive(false);
        _chargerCollider.enabled = true;
        _shieldCollider.enabled = false;
        _isShieldActivated = false;
    }

    private void GenerateShield() {
        _shieldSr.gameObject.SetActive(true);
        _isShieldActivated = true;
        _chargerCollider.enabled = false;
        _shieldCollider.enabled = true;
    }

    private void ChargerOn() {
        _animator.SetTrigger("Up");
        _boss.ChargerCntIncrease(_chargerIndex);
    }

    private void ChargerOff() {
        _animator.SetTrigger("Down");
        _boss.ChargerCntDecrease(_chargerIndex);
        RecoverHp().Forget();
    }

    private async UniTaskVoid RecoverHp() {
        while (_hp < _maxHp) {
            _hp += _hpRecoverAmount;
            UpdateHpSlider();
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
        _hp = _maxHp;
        ChargerOn();
    }

    private async UniTaskVoid InitializeCharger(float delay) {
        _hp = 1;
        GenerateShield();
        while (_hp < _maxHp) {
            _hp += _hpRecoverAmount;
            UpdateHpSlider();
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }
        _hp = _maxHp;
        ChargerOn();
    }

    private void UpdateHpSlider() {
        _hpSlider.value = (float)_hp / (float)_maxHp;
    }

    protected override void OnCollisionStay2D(Collision2D collision) {
        //Nothing
    }
}
