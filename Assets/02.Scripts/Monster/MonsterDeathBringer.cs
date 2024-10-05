using Cysharp.Threading.Tasks;
using DG.Tweening;
using Edgar.Unity.Examples.Gungeon;
using System;
using UnityEngine;

public class MonsterDeathBringer : MonsterBase {
    #region serialize field
    [SerializeField] GameObject _model;
    [SerializeField] private float _yDiffOffset;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _searchRange;
    [SerializeField] private float _normalAttackRange;//�⺻ ������ �����ϴ� �Ÿ� ����
    [SerializeField] private Vector2 _normalAttackArea;//�⺻ ������ ���� ���� ����
    [SerializeField] private float _normalAttackCooltime;
    [SerializeField] private GameObject _spellCastObj;
    [SerializeField] private Vector2 _spellAttackArea;
    [SerializeField] private float _spellAttackCooltime;
    [SerializeField] private int _spellAttackDamage;
    [SerializeField] private float _spellAttackStunTime;
    [SerializeField] protected AudioClip attack_Thunder_Sound;
    [SerializeField] protected AudioClip basicAttack;
    #endregion //serialize field





    #region private variable

    private bool _isNormalAttackReady;
    private bool _isSpellAttackReady;
    private bool _isAttacking;
    private float _normalAttackDelay;
    private float _spellAttackDelay;
    #endregion //private variable


    #region mono funcs 

    private void Start() {
        AudioSource audioSource = GetComponent<AudioSource>();
    }
    new private void OnEnable() {
        base.OnEnable();
        Spawn().Forget();
        _isNormalAttackReady = true;
        _isSpellAttackReady = true;
        _isAttacking = false;
        _normalAttackDelay = 1f;
        _spellAttackDelay = 0f;
    }

    private void Update() {
        AttackMove();
    }

    #endregion //mono funcs



    #region public funcs
    public override void Damaged(int dmg, bool isCrit)//�÷��̾� ���ݿ� �������� ����
    {
        if (!_isDead && _isSpawnComplete) {
            FloatingDamageManager.Instance.ShowDamage(this.gameObject, dmg, false, isCrit, false);

            if (_hp <= dmg) {
                PlaySound(_deathSound);
                Die().Forget();
            }
            else {
                ChangeColor().Forget();
                _hp -= dmg;
                PlayRandomSound();
            }
        }
    }
    #endregion



    #region protected funcs

    protected override void AttackMove() { //Ž�� ���� �ȿ� �÷��̾ �������� ��, ������ ���������� ������ ���, �װ� �ƴ϶�� ���� �������� �÷��̾�� ����
        if (!_isAttacking && !_isStun && !_isDead && _isSpawnComplete) {
            double dist = CalculateManhattanDist(transform.position, _playerScript.transform.position);
            float yDiff = transform.position.y - _playerScript.transform.position.y;

            if (dist < _normalAttackRange && yDiff < _yDiffOffset) {//�÷��̾ ���� ����
                _rigid.velocity = Vector2.zero;
                _animator.SetBool("Walk", false);

                if (_isNormalAttackReady) {
                    NormalAttackTask().Forget();
                }

            }
            else if (dist < _searchRange) { //�÷��̾ �߰� �� ����
                if (_isSpellAttackReady) {
                    _rigid.velocity = Vector2.zero;
                    SpellAttackTask().Forget();
                }
                else {
                    Vector2 moveVec = _playerScript.transform.position - transform.position;

                    if (moveVec.x < 0) {
                        _model.transform.localPosition = -Vector3.right * 2.2f;
                        _spriteRenderer.flipX = false;
                    }

                    else {
                        _model.transform.localPosition = Vector3.right * 2.2f;
                        _spriteRenderer.flipX = true;
                    }

                    if (dist < _normalAttackRange)
                        _rigid.velocity = Vector2.up * moveVec.normalized.y * _moveSpeed;
                    else
                        _rigid.velocity = moveVec.normalized * _moveSpeed;
                    _animator.SetBool("Walk", true);
                }
            }
            else { //�÷��̾ �߰����� ���� ����
                _rigid.velocity = Vector2.zero;
                _animator.SetBool("Walk", false);
            }
        }
        else {
            _rigid.velocity = Vector2.zero;
            _animator.SetBool("Walk", false);
        }
    }

    protected override async UniTaskVoid ChangeColor() {
        if (!_isColorChanged) {
            _isColorChanged = true;
            Color32 originColor = _spriteRenderer.color;
            _spriteRenderer.color = _damagedColor;
            await UniTask.Delay(TimeSpan.FromSeconds(_colorChanageLastTime));
            _spriteRenderer.color = originColor;
            _isColorChanged = false;
        }
    }

    protected override async UniTaskVoid Die() {

        _isDead = true;
        //Debug.Log("���� ���");
        _hp = 0;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        _animator.SetTrigger("Die");
        DropItems();
        _playerScript.GetExp(_exp);
        PlayerDataManager.Instance.SetFeverGauge(PlayerDataManager.Instance.Status._feverGauge + _feverAmount);
        GetComponent<GungeonEnemy>().RoomManager.OnEnemyKilled(gameObject);//���� asset�� ����
        await UniTask.Delay(TimeSpan.FromSeconds(_dieDelay));
        gameObject.SetActive(false);//���Ŀ� ������Ʈ Ǯ ���Ÿ� ������ ��
    }
    protected override async UniTaskVoid Spawn() {
        Color originColor = _spriteRenderer.color;
        _spriteRenderer.color = new Color(originColor.r, originColor.g, originColor.b, 0);
        await _spriteRenderer.DOFade(1, 1);
        _isSpawnComplete = true;
    }
    #endregion //protected funcs




    #region private funcs

    private async UniTaskVoid NormalAttackTask() {
        _isNormalAttackReady = false;
        NormalAttack().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_normalAttackCooltime));
        _isNormalAttackReady = true;
    }

    private async UniTaskVoid NormalAttack() {
        _isAttacking = true;
        PlaySound(basicAttack);
        _animator.SetTrigger("NormalAttack");
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));//�ִϸ��̼� ������
        Vector3 dir;
        if (!_spriteRenderer.flipX)
            dir = Vector3.left;
        else
            dir = Vector3.right;

        if (Physics2D.BoxCast(transform.position, new Vector2(1, _normalAttackArea.y), 0, dir, _normalAttackArea.x, _playerLayer))
            _playerScript.Damaged(_damage);
        await UniTask.Delay(TimeSpan.FromSeconds(_normalAttackDelay));
        _isAttacking = false;
    }

    private async UniTaskVoid SpellAttackTask() {
        _isSpellAttackReady = false;
        SpellAttack().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_spellAttackCooltime));
        _isSpellAttackReady = true;
    }

    private async UniTaskVoid SpellAttack() {
        _isAttacking = true;
        _animator.SetTrigger("SpellAttack");
        await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
        PlaySound(attack_Thunder_Sound);
        await UniTask.Delay(TimeSpan.FromSeconds(0.9f));//�ִϸ��̼� ������
        if (!_isDead) {
            GameObject spellCast = ObjectPool.Instance.GetObject(_spellCastObj);
            Vector3 offset = Vector3.right * 0.6f;
            Vector3 playerPos = _playerScript.transform.position;
            spellCast.transform.position = playerPos + Vector3.up * 5 + offset;
            spellCast.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(0.7f));//ĳ��Ʈ �ִϸ��̼� ������
            if (Physics2D.OverlapBox(playerPos, _spellAttackArea, 0, _playerLayer)) {
                _playerScript.Damaged(_spellAttackDamage);
                _playerScript.Stun(_spellAttackStunTime);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1));//ĳ��Ʈ �ִϸ��̼� ������
            ObjectPool.Instance.ReturnObject(spellCast);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(_spellAttackDelay));
        _isAttacking = false;
    }

    private double CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    #endregion //private funcs
}
