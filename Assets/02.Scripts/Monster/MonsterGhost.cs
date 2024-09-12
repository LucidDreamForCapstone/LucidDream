using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MonsterGhost : MonsterBase {

    #region serialize field

    [SerializeField] private float _searchRange;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackCooltime;
    [SerializeField] private AudioClip attackSound;

    #endregion //serialize field





    #region private variable

    private bool _isAttackReady;
    private bool _isAttacking;
    private float _attackDelay;

    #endregion //private variable


    #region mono funcs 

    new private void OnEnable() {
        base.OnEnable();
        _isAttackReady = true;
        _isAttacking = false;
        _attackDelay = 0.5f;
    }

    private void Update() {
        AttackMove();
    }

    #endregion //mono funcs




    #region protected funcs

    protected override void AttackMove() {
        if (!_isAttacking && !_isDead && !_isStun) {
            double dist = CalculateManhattanDist(transform.position, _playerScript.transform.position);

            if (dist < _attackRange) {//플레이어를 향해 공격
                if (_isAttackReady)
                    AttackTask().Forget();

                _rigid.velocity = Vector2.zero;
            }
            else if (dist < _searchRange) { //플레이어를 발견 후 접근
                Vector2 moveVec = _playerScript.transform.position - transform.position;

                if (moveVec.x < 0)
                    _spriteRenderer.flipX = false;
                else
                    _spriteRenderer.flipX = true;

                _rigid.velocity = moveVec.normalized * _moveSpeed;
                _animator.SetBool("Run", true);
            }
            else { //플레이어를 발견하지 못한 상태
                _rigid.velocity = Vector2.zero;
                _animator.SetBool("Run", false);
            }
        }
        else
            _rigid.velocity = Vector2.zero;
    }

    #endregion //protected funcs




    #region private funcs

    private async UniTaskVoid AttackTask() {
        _isAttackReady = false;
        Attack().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooltime));
        _isAttackReady = true;
    }

    private async UniTaskVoid Attack() {
        _isAttacking = true;
        _animator.SetTrigger("Attack");
        PlaySound(attackSound);
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));//애니메이션 딜레이
        if (CalculateManhattanDist(transform.position, _playerScript.transform.position) < _attackRange && !_isDead)
            _playerScript.Damaged(_damage);
        await UniTask.Delay(TimeSpan.FromSeconds(_attackDelay));
        _isAttacking = false;
    }

    private double CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    #endregion //private funcs
}


