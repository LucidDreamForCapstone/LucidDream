using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossBondrewd : MonsterBase {
    [SerializeField] GameObject _dashEffect;
    [SerializeField] GameObject _rushWarningEffect;
    [SerializeField] GameObject _rushEffect;
    [SerializeField] GameObject _backStepEffect;
    [SerializeField] GameObject _bulletObj;
    [SerializeField] GameObject _armTargetObj;
    [SerializeField] SpriteRenderer _legSr;
    [SerializeField] float _backStepCooltime;
    [SerializeField] float _backStepDist;
    [SerializeField] float _chaseCooltime;
    [SerializeField] float _chaseDist;
    [SerializeField] float _rushDist;
    [SerializeField] float _rushWarningTime;
    [SerializeField] float _rushCooltime;
    [SerializeField] int _rushDamage;
    [SerializeField] float _shootCooltime;
    [SerializeField] int _shootCount;
    [SerializeField] int _bulletCount;
    [SerializeField] int _bulletDamage;
    [SerializeField] float _bulletSpeed;
    [SerializeField] float _bulletRange;
    [SerializeField] float _bulletInterval;
    [SerializeField] float _shootInterval;
    bool _isBackStepReady;
    bool _isChaseReady;
    bool _isRushReady;
    bool _isShootReady;
    Vector2[] _shootPos = { new Vector2(3.25f, -0.97f), new Vector2(-3.25f, -0.97f) };

    private void Start() {
        _isSpawnComplete = true;
        _attackFuncList.Add(BackStepTask);
        _attackFuncList.Add(ChaseTask);
        _attackFuncList.Add(RushTask);
        _attackFuncList.Add(ShootTask);
    }

    protected override void AttackMove() {
    }

    protected override async UniTaskVoid ChangeColor()//피격 시 붉은 색으로 몬스터 색 변경
    {
        if (!_isColorChanged) {
            _isColorChanged = true;
            _spriteRenderer.color = _damagedColor;
            _legSr.color = _damagedColor;
            await UniTask.Delay(TimeSpan.FromSeconds(_colorChanageLastTime));
            _spriteRenderer.color = Color.white;
            _legSr.color = Color.white;
            _isColorChanged = false;
        }
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
        _attackStateList[0] = AttackState.Finished;
        await UniTask.NextFrame();
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
        _attackStateList[1] = AttackState.Finished;
        await UniTask.NextFrame();
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
            Vector2 tempDir = _playerScript.transform.position - transform.position;
            SetFlipX(tempDir);
            _rushWarningEffect.transform.right = tempDir;
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
        _attackStateList[2] = AttackState.Finished;
        await UniTask.NextFrame();
        _attackStateList[2] = AttackState.CoolTime;
    }

    private async UniTaskVoid ShootTask() {
        _isShootReady = false;
        Shoot().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_shootCooltime));
        _isShootReady = true;
        _attackStateList[3] = AttackState.Ready;
    }

    private async UniTaskVoid Shoot() {
        _attackStateList[3] = AttackState.Attacking;
        int cnt = 0;
        while (cnt < _shootCount) {
            MoveSide().Forget();
            _animator.SetTrigger("ShootRun");
            Vector3 fireDir = _playerScript.transform.position - _armTargetObj.transform.position;
            SetFlipX(fireDir);
            for (int i = 0; i < _bulletCount; i++) {
                float x = Random.Range(-1f, 1f);
                float y = Random.Range(-1f, 1f);
                FireBullet(fireDir, new Vector2(x, y));
                await UniTask.Delay(TimeSpan.FromSeconds(_bulletInterval));
            }
            cnt++;
            await UniTask.Delay(TimeSpan.FromSeconds(_shootInterval));
        }
        _attackStateList[3] = AttackState.Finished;
        await UniTask.NextFrame();
        _attackStateList[3] = AttackState.CoolTime;
    }
    private async UniTaskVoid MoveSide() {
        Vector2 shootVec = _playerScript.transform.position - transform.position;
        Vector2 sideVec = new Vector2(shootVec.y, -shootVec.x);
        int randomN = Random.Range(0, 2);
        if (randomN == 1) {
            sideVec *= -1;
        }
        _rigid.velocity = sideVec.normalized * _moveSpeed * 0.2f;
        await UniTask.Delay(TimeSpan.FromSeconds(_shootInterval));
        _rigid.velocity = Vector2.zero;
    }

    private void FireBullet(Vector3 dir, Vector3 offsetDir) {
        GameObject projectile = ObjectPool.Instance.GetObject(_bulletObj);
        Bullet projectileScript = projectile.GetComponent<Bullet>();
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(_bulletSpeed);
        projectileScript.SetDmg(_bulletDamage);
        float lastTime = _bulletRange / _bulletSpeed;
        projectileScript.SetLastTime(lastTime);
        Vector2 fireDir = dir + offsetDir;
        projectile.transform.right = fireDir;
        projectile.transform.position = _armTargetObj.transform.position;
        projectile.SetActive(true);
    }


    private float CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void SetFlipX(Vector2 moveDir) {
        if (moveDir.x < 0) {
            _spriteRenderer.flipX = true;
            _legSr.flipX = true;
            _armTargetObj.transform.localPosition = _shootPos[1];
            _dashEffect.GetComponent<SpriteRenderer>().flipX = true;
            _backStepEffect.GetComponent<SpriteRenderer>().flipX = false;
        }

        else {
            _spriteRenderer.flipX = false;
            _legSr.flipX = false;
            _armTargetObj.transform.localPosition = _shootPos[0];
            _dashEffect.GetComponent<SpriteRenderer>().flipX = false;
            _backStepEffect.GetComponent<SpriteRenderer>().flipX = true;
        }

    }
}
