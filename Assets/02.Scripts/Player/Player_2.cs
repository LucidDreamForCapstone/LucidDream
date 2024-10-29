using UnityEngine;

public class Player_2 : Player {
    #region serialized field
    [SerializeField] private float _specialSpeed; // Player_2의 특별한 속도
    #endregion // serialized field

    #region private variable
    private bool _isSpecialActive; // 특별한 상태를 나타내는 변수
    #endregion // private variable

    #region mono funcs

    private new void Start() {
        base.Start(); // 부모 클래스의 Start() 호출
        _isSpecialActive = false;
    }

    private void FixedUpdate() {
        Move(); // 기본 움직임
        ArmMoveAnimation(); // 팔 애니메이션
        HandleSpecialAbility(); // 특별한 능력 처리
    }

    private void Update() {
        base.Update(); // 부모 클래스의 Update() 호출
    }

    #endregion // mono funcs

    #region public funcs

    public void ActivateSpecialAbility() {
        _isSpecialActive = true; // 특별한 능력 활성화
    }

    public void DeactivateSpecialAbility() {
        _isSpecialActive = false; // 특별한 능력 비활성화
    }

    #endregion // public funcs

    #region private funcs

    private void Move() {
        if (!_isRollInvincible && !_isAttacking && !_isStun && !_isPause && !_isDead) {
            Vector2 moveVec = Vector2.zero;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Player_2의 특별한 속도를 반영하여 이동 벡터를 조정
            float moveSpeed = PlayerDataManager.Instance.GetFinalMoveSpeed() * (_isSpecialActive ? _specialSpeed : 1);
            moveVec = new Vector2(horizontal, vertical).normalized * moveSpeed * (100 - _slowRate) * _phantomSpeedRate * 0.01f;

            // 플레이어의 움직임 애니메이션
            PlayerMoveAnimation(horizontal, vertical);

            // Rigidbody에 속도 설정
            _rigid.velocity = moveVec;

            if (horizontal != 0 || vertical != 0) {
                MoveDir = moveVec.normalized; // 이동 방향 업데이트
            }
        }
        else {
            _rigid.velocity = Vector2.zero; // 정지 상태
            PlayerMoveAnimation(0, 0);
        }
    }

    private void HandleSpecialAbility() {
        // 특별한 능력의 로직을 여기에 구현
        if (_isSpecialActive) {
            // 예: 특별한 능력 사용 시 추가 효과 처리
            Debug.Log("Player_2의 특별한 능력이 활성화되었습니다.");
        }
    }
    #endregion // private funcs
}
