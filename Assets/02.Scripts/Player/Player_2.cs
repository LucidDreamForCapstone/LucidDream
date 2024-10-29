using UnityEngine;

public class Player_2 : Player {
    #region serialized field
    [SerializeField] private float _specialSpeed; // Player_2�� Ư���� �ӵ�
    #endregion // serialized field

    #region private variable
    private bool _isSpecialActive; // Ư���� ���¸� ��Ÿ���� ����
    #endregion // private variable

    #region mono funcs

    private new void Start() {
        base.Start(); // �θ� Ŭ������ Start() ȣ��
        _isSpecialActive = false;
    }

    private void FixedUpdate() {
        Move(); // �⺻ ������
        ArmMoveAnimation(); // �� �ִϸ��̼�
        HandleSpecialAbility(); // Ư���� �ɷ� ó��
    }

    private void Update() {
        base.Update(); // �θ� Ŭ������ Update() ȣ��
    }

    #endregion // mono funcs

    #region public funcs

    public void ActivateSpecialAbility() {
        _isSpecialActive = true; // Ư���� �ɷ� Ȱ��ȭ
    }

    public void DeactivateSpecialAbility() {
        _isSpecialActive = false; // Ư���� �ɷ� ��Ȱ��ȭ
    }

    #endregion // public funcs

    #region private funcs

    private void Move() {
        if (!_isRollInvincible && !_isAttacking && !_isStun && !_isPause && !_isDead) {
            Vector2 moveVec = Vector2.zero;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Player_2�� Ư���� �ӵ��� �ݿ��Ͽ� �̵� ���͸� ����
            float moveSpeed = PlayerDataManager.Instance.GetFinalMoveSpeed() * (_isSpecialActive ? _specialSpeed : 1);
            moveVec = new Vector2(horizontal, vertical).normalized * moveSpeed * (100 - _slowRate) * _phantomSpeedRate * 0.01f;

            // �÷��̾��� ������ �ִϸ��̼�
            PlayerMoveAnimation(horizontal, vertical);

            // Rigidbody�� �ӵ� ����
            _rigid.velocity = moveVec;

            if (horizontal != 0 || vertical != 0) {
                MoveDir = moveVec.normalized; // �̵� ���� ������Ʈ
            }
        }
        else {
            _rigid.velocity = Vector2.zero; // ���� ����
            PlayerMoveAnimation(0, 0);
        }
    }

    private void HandleSpecialAbility() {
        // Ư���� �ɷ��� ������ ���⿡ ����
        if (_isSpecialActive) {
            // ��: Ư���� �ɷ� ��� �� �߰� ȿ�� ó��
            Debug.Log("Player_2�� Ư���� �ɷ��� Ȱ��ȭ�Ǿ����ϴ�.");
        }
    }
    #endregion // private funcs
}
