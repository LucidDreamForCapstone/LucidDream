using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player_2 : MonoBehaviour {
    #region serialized field
    [SerializeField] private float _invincibleLastTime;
    [SerializeField] private Color32 _originColor;
    [SerializeField] private Color32 _invincibleColor;
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveSpeed; // 기본 이동 속도
    [SerializeField] private float _itemSearchRadius;
    [SerializeField] Slider _hpSlider;
    [SerializeField] float _maxHp;
    [SerializeField] int _healAmount;
    #endregion // serialized field

    #region private variable
    private bool _isDead;
    private bool _isInvincible;
    private bool _isAttacking;
    private Vector2 _moveDir; // 이동 방향
    private bool _playerEnabled;
    private float _hp;
    #endregion // private variable

    #region property
    public Vector2 MoveDir { get; private set; } // 이동 방향
    #endregion // property

    #region mono funcs
    private void Start() {
        _isDead = false;
        _isInvincible = false;
        _playerEnabled = false;
        MoveDir = Vector2.down;
        _hp = _maxHp;
        UpdateHpSlider();
    }

    private void FixedUpdate() {
        Move(); // 기본 움직임

    }

    private void Update() {
    }
    #endregion // mono funcs

    #region public funcs

    public void Damaged(int dmg) {
        if (!_isInvincible && !_isDead) {
            FloatingDamageManager.Instance.ShowDamage(this.gameObject, dmg, true, false, false);
            if (_hp <= dmg)
                Die();
            else {
                _hp -= dmg;
                Invincibility().Forget();
            }
            UpdateHpSlider();
        }
    }

    public bool CheckEnabled() {
        return _playerEnabled;
    }

    public void SetEnable(bool enabled) {
        _playerEnabled = enabled;
    }

    public bool CheckInvincible() {
        return _isInvincible;
    }

    public async UniTaskVoid HealMax() {
        while (_hp < _maxHp) {
            _hp += _healAmount * Time.deltaTime;
            UpdateHpSlider();
            await UniTask.NextFrame();
        }
        _hp = _maxHp;
    }
    #endregion // public funcs

    #region private funcs

    private void Move() {
        if (!_isAttacking && !_isDead && _playerEnabled) {
            Vector2 moveVec = Vector2.zero;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            PlayerMoveAnimation(horizontal, vertical); // 애니메이션 호출

            moveVec = new Vector2(horizontal, vertical);

            // 이동 방향 업데이트
            if (horizontal != 0 || vertical != 0) {
                MoveDir = moveVec.normalized;
            }

            _rigid.velocity = moveVec.normalized * _moveSpeed; // Rigidbody에 속도 설정
        }
        else {
            _rigid.velocity = Vector2.zero; // 정지 상태
            PlayerMoveAnimation(0, 0); // 정지 애니메이션 호출
        }
    }

    private void PlayerMoveAnimation(float horizontal, float vertical) {
        // 애니메이션 처리
        if (horizontal > 0) {
            _spriteRenderer.flipX = true; // 오른쪽으로 이동 시
        }
        else if (horizontal < 0) {
            _spriteRenderer.flipX = false; // 왼쪽으로 이동 시
        }

        _animator.SetInteger("horizontal", (int)horizontal);
        _animator.SetInteger("vertical", (int)vertical);
    }
    private async UniTaskVoid Invincibility() {
        _isInvincible = true;
        _spriteRenderer.DOColor(_invincibleColor, 0.1f).ToUniTask().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_invincibleLastTime));
        _isInvincible = false;
        _spriteRenderer.DOColor(_originColor, 0.1f).ToUniTask().Forget();
    }
    private void Die() {
        HealMax().Forget();
        List<string> messages = new List<string>();
        messages.Add("이번만 봐드립니다.");
        SystemMessageManager.Instance.ShowDialogBox("개발자Y", messages).Forget();
    }

    private void UpdateHpSlider() {
        _hpSlider.value = _hp / _maxHp;
    }
    #endregion // private funcs
}
