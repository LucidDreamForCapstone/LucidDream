using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class BossBondrewd : MonsterBase {

    [SerializeField] float _backStepCooltime;
    [SerializeField] float _backStepDist;
    [SerializeField] float _chaseCooltime;
    [SerializeField] float _chaseDist;
    bool _isBackStepReady;
    bool _isChaseReady;


    private void Start() {
        _isSpawnComplete = true;
        _attackFuncList.Add(BackStepTask);
        _attackFuncList.Add(ChaseTask);
    }

    protected override void AttackMove() {

    }

    private async UniTaskVoid BackStepTask() {
        _isBackStepReady = false;
        BackStep().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_backStepCooltime));
        _isBackStepReady = true;
        _attackStateList[0] = AttackState.Ready;
    }

    private async UniTaskVoid BackStep() {
        _attackStateList[0] = AttackState.Attacking;
        Vector2 backstepDir = (transform.position - _playerScript.transform.position).normalized;
        Vector2 endValue = (Vector2)transform.position + backstepDir * _backStepDist;
        float duration = _backStepDist / _moveSpeed;
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration);
        _rigid.velocity = Vector2.zero;
        _attackStateList[0] = AttackState.CoolTime;
    }
    private async UniTaskVoid ChaseTask() {
        _isChaseReady = false;
        Chase().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_chaseCooltime));
        _isChaseReady = true;
        _attackStateList[1] = AttackState.Ready;
    }

    private async UniTaskVoid Chase() {
        _attackStateList[1] = AttackState.Attacking;
        Vector2 dashDir = (_playerScript.transform.position - transform.position).normalized;
        Vector2 endValue = (Vector2)transform.position + dashDir * _chaseDist;
        float duration = _chaseDist / _moveSpeed;
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration);
        _rigid.velocity = Vector2.zero;
        _attackStateList[1] = AttackState.CoolTime;
    }



    private float CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
