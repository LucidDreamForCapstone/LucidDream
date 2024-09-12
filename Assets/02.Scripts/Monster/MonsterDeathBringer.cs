using Cysharp.Threading.Tasks;
using Edgar.Unity.Examples.Gungeon;
using System;
using UnityEngine;

public class MonsterDeathBringer : MonsterBase {
    #region serialize field
    [SerializeField] GameObject _model;
    [SerializeField] private float _yDiffOffset;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _searchRange;
    [SerializeField] private float _normalAttackRange;//기본 공격을 시전하는 거리 범위
    [SerializeField] private Vector2 _normalAttackArea;//기본 공격의 실제 공격 범위
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
    new private SpriteRenderer _spriteRenderer;
    new private Animator _animator;
    #endregion //private variable


    #region mono funcs 

    private void Start() {
        AudioSource audioSource = GetComponent<AudioSource>();
    }
    new private void OnEnable() {
        base.OnEnable();
        _spriteRenderer = _model.GetComponent<SpriteRenderer>();
        _animator = _model.GetComponent<Animator>();
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
    public override void Damaged(int dmg, bool isCrit)//플레이어 공격에 데미지를 입음
    {
        if (!_isDead) {
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

    protected override void AttackMove() { //탐색 범위 안에 플레이어가 진입했을 때, 스펠이 돌아있으면 스펠을 사용, 그게 아니라면 근접 범위까지 플레이어에게 접근
        if (!_isAttacking && !_isStun && !_isDead) {
            double dist = CalculateManhattanDist(transform.position, _playerScript.transform.position);
            float yDiff = transform.position.y - _playerScript.transform.position.y;

            if (dist < _normalAttackRange && yDiff < _yDiffOffset) {//플레이어를 향해 공격
                _rigid.velocity = Vector2.zero;
                _animator.SetBool("Walk", false);

                if (_isNormalAttackReady) {
                    NormalAttackTask().Forget();
                }

            }
            else if (dist < _searchRange) { //플레이어를 발견 후 접근
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
            else { //플레이어를 발견하지 못한 상태
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
        //Debug.Log("몬스터 사망");
        _hp = 0;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        _animator.SetTrigger("Die");
        DropItems();
        _playerScript.GetExp(_exp);
        PlayerDataManager.Instance.SetFeverGauge(PlayerDataManager.Instance.Status._feverGauge + _feverAmount);
        GetComponent<GungeonEnemy>().RoomManager.OnEnemyKilled(gameObject);//던전 asset과 연결
        await UniTask.Delay(TimeSpan.FromSeconds(_dieDelay));
        gameObject.SetActive(false);//이후에 오브젝트 풀 쓸거면 변경할 것
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
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));//애니메이션 딜레이
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
        await UniTask.Delay(TimeSpan.FromSeconds(0.9f));//애니메이션 딜레이
        if (!_isDead) {
            GameObject spellCast = ObjectPool.Instance.GetObject(_spellCastObj);
            Vector3 offset = Vector3.right * 0.6f;
            Vector3 playerPos = _playerScript.transform.position;
            spellCast.transform.position = playerPos + Vector3.up * 5 + offset;
            spellCast.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(0.7f));//캐스트 애니메이션 딜레이
            if (Physics2D.OverlapBox(playerPos, _spellAttackArea, 0, _playerLayer)) {
                _playerScript.Damaged(_spellAttackDamage);
                _playerScript.Stun(_spellAttackStunTime);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1));//캐스트 애니메이션 딜레이
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
