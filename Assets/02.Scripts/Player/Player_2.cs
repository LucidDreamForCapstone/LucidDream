using UnityEngine;

public class Player_2 : MonoBehaviour {
    #region serialized field
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveSpeed; // 기본 이동 속도
    [SerializeField] private float _itemSearchRadius;
    #endregion // serialized field

    #region private variable
    private bool _isDead;
    private bool _isInvincible;
    private bool _isAttacking;
    private Vector2 _moveDir; // 이동 방향
    private bool _playerEnabled;
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
            // 피해 처리 로직
            // 예: 플레이어 HP 감소 및 사망 처리
        }
    }

    public void ReturnToDungeon() {
        // 던전으로 돌아가는 로직 구현
    }

    public bool CheckEnabled() {
        return _playerEnabled;
    }

    public void SetEnable(bool enabled) {
        _playerEnabled = enabled;
    }

    #endregion // public funcs

    #region private funcs

    private void Move() {
        if (!_isAttacking && !_isDead && _playerEnabled) {
            Vector2 moveVec = Vector2.zero;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            moveVec = new Vector2(horizontal, vertical);

            // 이동 방향 업데이트
            if (horizontal != 0 || vertical != 0) {
                MoveDir = moveVec.normalized;
            }

            _rigid.velocity = moveVec.normalized * _moveSpeed; // Rigidbody에 속도 설정
            PlayerMoveAnimation(horizontal, vertical); // 애니메이션 호출
        }
        else {
            _rigid.velocity = Vector2.zero; // 정지 상태
            PlayerMoveAnimation(0, 0); // 정지 애니메이션 호출
        }
    }

    private void PlayerMoveAnimation(float horizontal, float vertical) {
        // 애니메이션 처리
        if (horizontal > 0) {
            _spriteRenderer.flipX = false; // 오른쪽으로 이동 시
        }
        else if (horizontal < 0) {
            _spriteRenderer.flipX = true; // 왼쪽으로 이동 시
        }

        _animator.SetInteger("horizontal", (int)horizontal);
        _animator.SetInteger("vertical", (int)vertical);
    }
    #endregion // private funcs
}
