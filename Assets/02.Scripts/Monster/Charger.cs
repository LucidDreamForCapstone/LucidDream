using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Charger : MonsterBase {
    [SerializeField] int _chargerIndex;
    [SerializeField] int _hpRecoverAmount;
    [SerializeField] BossBondrewd _boss;
    [SerializeField] Slider _hpSlider;
    [SerializeField] ButtonPressQTE _chargerQTE;
    [SerializeField] float _layerBorder;
    [SerializeField] AudioClip _chargerOnSound;
    [SerializeField] AudioClip _chargerOffSound;
    CapsuleCollider2D _chargerCollider;
    CircleCollider2D _shieldCollider;
    SpriteRenderer _sr;
    SpriteRenderer _shieldSr;
    SpriteRenderer _minimapIconSr;
    Animator _shieldAnimator;
    bool _isShieldActivated;
    CancellationTokenSource _cts;

    private void Start() {
        _cts = new CancellationTokenSource();
        _chargerCollider = GetComponent<CapsuleCollider2D>();
        _shieldCollider = GetComponent<CircleCollider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _chargerQTE = GameObject.Find("ButtonPressQTE").GetComponent<ButtonPressQTE>();
        _shieldAnimator = transform.GetChild(0).GetComponent<Animator>();
        _shieldSr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _minimapIconSr = transform.GetChild(2).GetComponent<SpriteRenderer>();
        InitializeCharger(_chargerIndex + 1).Forget();
        UpdateHpSlider();
    }

    new private void Update() {
        ChangeChargerSortingLayer();
    }

    public override void Damaged(int dmg, bool isCrit, bool isPoison = false) {
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
        PlayerDataManager.Instance.SetFeverGauge(_feverAmount);
        UpdateHpSlider();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        ChargerOff();
        GenerateShield();
    }

    public void CallBackRemoveShield() {
        RemoveShield().Forget();
    }

    public async UniTaskVoid Sleep() {
        _cts.Cancel();
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        _cts.Dispose();
        _cts = null;
    }

    private async UniTaskVoid RemoveShield() { //after called by lab
        _shieldAnimator.SetTrigger("Pop");
        await UniTask.Delay(TimeSpan.FromSeconds(0.8f));
        _shieldSr.gameObject.SetActive(false);
        _chargerQTE.UpdateShieldState(_chargerIndex, false);
        _chargerCollider.enabled = true;
        _shieldCollider.enabled = false;
        _isShieldActivated = false;
    }

    private void GenerateShield() {
        _shieldSr.gameObject.SetActive(true);
        _chargerQTE.UpdateShieldState(_chargerIndex, true);
        _isShieldActivated = true;
        _chargerCollider.enabled = false;
        _shieldCollider.enabled = true;
    }

    private void ChargerOn() {
        _animator.SetTrigger("Up");
        _chargerQTE.UpdateChargingState(_chargerIndex, true);
        _boss.ChargerCntIncrease(_chargerIndex);
        _minimapIconSr.color = Color.red;
        SoundManager.Instance.PlaySFX(_chargerOnSound.name, false);
        SystemMessageManager.Instance.PushSystemMessage("영혼 가속기가 활성화 되어 연구소장의 팬텀 게이지가 더욱 빠르게 상승합니다.", Color.red, lastTime: 2);
    }

    private void ChargerOff() {
        _animator.SetTrigger("Down");
        _chargerQTE.UpdateChargingState(_chargerIndex, false);
        _boss.ChargerCntDecrease(_chargerIndex);
        _minimapIconSr.color = Color.blue;
        SoundManager.Instance.PlaySFX(_chargerOffSound.name, false);
        RecoverHp().Forget();
    }

    private async UniTask RecoverHp(float delay = 1.0f) {
        try {
            while (_hp < _maxHp) {
                _hp += _hpRecoverAmount;
                UpdateHpSlider();
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _cts.Token);
            }
            _hp = _maxHp;
            ChargerOn();
        }
        catch (OperationCanceledException) {
            ChargerOff();
            RemoveShield().Forget();
        }
    }

    private async UniTaskVoid InitializeCharger(float delay) {
        _isShieldActivated = true;
        await UniTask.WaitUntil(() => _boss.CheckBossWakeUp());
        _hp = 1;
        GenerateShield();
        await RecoverHp(delay);
    }

    private void UpdateHpSlider() {
        _hpSlider.value = (float)_hp / (float)_maxHp;
    }

    protected override void OnCollisionStay2D(Collision2D collision) {
    }

    private void ChangeChargerSortingLayer() {
        if (_playerScript.transform.position.y - transform.position.y > _layerBorder)
            _sr.sortingOrder = 8;
        else
            _sr.sortingOrder = 4;
    }
}
