using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MonsterGhost : MonsterBase {

    #region serialize field

    //[SerializeField] private float _searchRange;
    //[SerializeField] private float _attackRange;
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
        Spawn().Forget();
        _isAttackReady = true;
        _isAttacking = false;
        _attackDelay = 0.5f;
    }

    #endregion //mono funcs


    public override async UniTaskVoid Attack() {
        await ScratchTask();
    }


    #region protected funcs

    protected override void AttackMove() {
        /*
        if (!_isAttacking && !_isDead && !_isStun && _isSpawnComplete) {
            double dist = CalculateManhattanDist(transform.position, _playerScript.transform.position);

            if (dist < _attackRange) {
                if (_isAttackReady)
                    AttackTask().Forget();

                _rigid.velocity = Vector2.zero;
            }
            else if (dist < _searchRange) {
                Vector2 moveVec = _playerScript.transform.position - transform.position;

                if (moveVec.x < 0)
                    _spriteRenderer.flipX = false;
                else
                    _spriteRenderer.flipX = true;

                _rigid.velocity = moveVec.normalized * _moveSpeed;
                _animator.SetBool("Run", true);
            }
            else {
                _rigid.velocity = Vector2.zero;
                _animator.SetBool("Run", false);
            }
        }
        else
            _rigid.velocity = Vector2.zero;
        */
    }

    #endregion //protected funcs




    #region private funcs

    private async UniTask ScratchTask() {
        _isAttackReady = false;
        Scratch().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooltime));
        _isAttackReady = true;
        _attackState = AttackState.Ready;
    }

    private async UniTaskVoid Scratch() {
        _attackState = AttackState.Attacking;
        _isAttacking = true;
        _animator.SetTrigger("Attack");
        PlaySound(attackSound);
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));//�ִϸ��̼� ������
        if (GetDistSquare(transform.position, _playerScript.transform.position) < _attackDist * _attackDist && !_isDead)
            _playerScript.Damaged(_damage);
        await UniTask.Delay(TimeSpan.FromSeconds(_attackDelay));
        _isAttacking = false;
        _attackState = AttackState.CoolTime;
    }

    private float GetDistSquare(Vector2 a, Vector2 b) {
        return (a - b).sqrMagnitude;
    }

    #endregion //private funcs
}
