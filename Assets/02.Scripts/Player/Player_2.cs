using UnityEngine;

public class Player_2 : MonoBehaviour {
    #region serialized field
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveSpeed; // �⺻ �̵� �ӵ�
    [SerializeField] private float _itemSearchRadius;
    #endregion // serialized field

    #region private variable
    private bool _isDead;
    private bool _isInvincible;
    private bool _isAttacking;
    private Vector2 _moveDir; // �̵� ����
    private bool _playerEnabled;
    #endregion // private variable

    #region property
    public Vector2 MoveDir { get; private set; } // �̵� ����
    #endregion // property

    #region mono funcs
    private void Start() {
        _isDead = false;
        _isInvincible = false;
        _playerEnabled = false;
        MoveDir = Vector2.down;
    }

    private void FixedUpdate() {
        Move(); // �⺻ ������
    }

    private void Update() {
    }
    #endregion // mono funcs

    #region public funcs

    public void Damaged(int dmg) {
        if (!_isInvincible && !_isDead) {
            // ���� ó�� ����
            // ��: �÷��̾� HP ���� �� ��� ó��
        }
    }

    public void ReturnToDungeon() {
        // �������� ���ư��� ���� ����
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

            // �̵� ���� ������Ʈ
            if (horizontal != 0 || vertical != 0) {
                MoveDir = moveVec.normalized;
            }

            _rigid.velocity = moveVec.normalized * _moveSpeed; // Rigidbody�� �ӵ� ����
            PlayerMoveAnimation(horizontal, vertical); // �ִϸ��̼� ȣ��
        }
        else {
            _rigid.velocity = Vector2.zero; // ���� ����
            PlayerMoveAnimation(0, 0); // ���� �ִϸ��̼� ȣ��
        }
    }

    private void PlayerMoveAnimation(float horizontal, float vertical) {
        // �ִϸ��̼� ó��
        if (horizontal > 0) {
            _spriteRenderer.flipX = false; // ���������� �̵� ��
        }
        else if (horizontal < 0) {
            _spriteRenderer.flipX = true; // �������� �̵� ��
        }

        _animator.SetInteger("horizontal", (int)horizontal);
        _animator.SetInteger("vertical", (int)vertical);
    }
    #endregion // private funcs
}
