using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class BossBondrewd : MonsterBase {
    [SerializeField] GameObject _dashEffect;
    [SerializeField] GameObject _rushWarningEffect;
    [SerializeField] GameObject _rushEffect;
    [SerializeField] GameObject _backStepEffect;
    [SerializeField] float _backStepCooltime;
    [SerializeField] float _backStepDist;
    [SerializeField] float _chaseCooltime;
    [SerializeField] float _chaseDist;
    [SerializeField] float _rushDist;
    [SerializeField] float _rushWarningTime;
    [SerializeField] float _rushCooltime;
    [SerializeField] int _rushDamage;
    bool _isBackStepReady;
    bool _isChaseReady;
    bool _isRushReady;


    private void Start() {
        _isSpawnComplete = true;
        _attackFuncList.Add(BackStepTask);
        _attackFuncList.Add(ChaseTask);
        _attackFuncList.Add(RushTask);
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
        SetFlipX(-backstepDir);
        _backStepEffect.SetActive(true);
        _animator.SetBool("Run", true);
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration);
        _animator.SetBool("Run", false);
        _backStepEffect.SetActive(false);
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
        SetFlipX(dashDir);
        _dashEffect.SetActive(true);
        _animator.SetBool("Run", true);
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration);
        _animator.SetBool("Run", false);
        _dashEffect.SetActive(false);
        _rigid.velocity = Vector2.zero;
        _attackStateList[1] = AttackState.CoolTime;
    }

    private async UniTaskVoid RushTask() {
        _isRushReady = false;
        Rush().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_rushCooltime));
        _isRushReady = true;
        _attackStateList[2] = AttackState.Ready;
    }

    private async UniTaskVoid Rush() {
        _attackStateList[2] = AttackState.Attacking;
        _rushWarningEffect.SetActive(true);
        float timer = 0;
        while (timer < _rushWarningTime - 1) {
            _rushWarningEffect.transform.right = (Vector2)(_playerScript.transform.position - transform.position);
            timer += Time.deltaTime;
            await UniTask.NextFrame();
        }

        Vector2 rushDir = (_playerScript.transform.position - transform.position).normalized;
        Vector2 endValue = (Vector2)transform.position + rushDir * _rushDist;
        float duration = _rushDist / _moveSpeed;
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        _rushWarningEffect.SetActive(false);

        int originBodyDamage = _bodyDamage;
        _bodyDamage = _rushDamage;
        _rushEffect.transform.right = rushDir;
        _rushEffect.SetActive(true);
        SetFlipX(rushDir);
        _animator.SetBool("Run", true);
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration);
        _animator.SetBool("Run", false);
        _rushEffect.SetActive(false);
        _rigid.velocity = Vector2.zero;
        _bodyDamage = originBodyDamage;
        _attackStateList[2] = AttackState.CoolTime;
    }


    private float CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void SetFlipX(Vector2 moveDir) {
        if (moveDir.x < 0) {
            _spriteRenderer.flipX = true;
            _dashEffect.GetComponent<SpriteRenderer>().flipX = true;
            _backStepEffect.GetComponent<SpriteRenderer>().flipX = false;
        }

        else {
            _spriteRenderer.flipX = false;
            _dashEffect.GetComponent<SpriteRenderer>().flipX = false;
            _backStepEffect.GetComponent<SpriteRenderer>().flipX = true;
        }

    }
}
