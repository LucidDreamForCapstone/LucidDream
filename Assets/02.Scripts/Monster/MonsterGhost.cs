using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MonsterGhost : MonsterBase {

    #region serialize field

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
        _attackFuncList.Add(ScratchTask);
        _isAttackReady = true;
        _isAttacking = false;
        _attackDelay = 0.5f;
    }

    #endregion //mono funcs




    #region private funcs

    private async UniTaskVoid ScratchTask() {
        _isAttackReady = false;
        Scratch().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooltime));
        _isAttackReady = true;
        _attackStateList[0] = AttackState.Ready;
    }

    private async UniTaskVoid Scratch() {
        _attackStateList[0] = AttackState.Attacking;
        _isAttacking = true;
        _animator.SetTrigger("Attack");
        PlaySound(attackSound);
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));//�ִϸ��̼� ������
        if (GetDistSquare(transform.position, _playerScript.transform.position) < _attackDist * _attackDist && !_isDead)
            _playerScript.Damaged(_damage);
        await UniTask.Delay(TimeSpan.FromSeconds(_attackDelay));
        _isAttacking = false;
        _attackStateList[0] = AttackState.CoolTime;
    }

    #endregion //private funcs
}
