using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossBondrewd : MonsterBase {
    [SerializeField] GameObject _dashEffect;
    [SerializeField] GameObject _rushWarningEffect;
    [SerializeField] GameObject _rushEffect;
    [SerializeField] GameObject _backStepEffect;
    [SerializeField] GameObject _chargingEffect;
    [SerializeField] GameObject _bulletObj;
    [SerializeField] GameObject _missileObj;
    [SerializeField] GameObject _armTargetObj;
    [SerializeField] GameObject _missileTargetObj;
    [SerializeField] GameObject _explosionObj;
    [SerializeField] GameObject _phantomGhostObj;
    [SerializeField] SpriteRenderer _legSr;
    [SerializeField] Slider _hpSlider;
    [SerializeField] Color _phase2HpColor;
    [SerializeField] Color _phase3HpColor;

    [Header("\nBackStep")]
    [SerializeField] float _backStepCooltime;
    [SerializeField] float _backStepDist;
    [Header("\nChase")]
    [SerializeField] float _chaseCooltime;
    [SerializeField] float _chaseDist;
    [Header("\nRush")]
    [SerializeField] float _rushDist;
    [SerializeField] float _rushWarningTime;
    [SerializeField] float _rushCooltime;
    [SerializeField] int _rushDamage;
    [Header("\nShoot")]
    [SerializeField] float _shootCooltime;
    [SerializeField] int _shootCount;
    [SerializeField] float _shootDispersionRate;
    [SerializeField] int _bulletCount;
    [SerializeField] int _bulletDamage;
    [SerializeField] float _bulletSpeed;
    [SerializeField] float _bulletRange;
    [SerializeField] float _bulletInterval;
    [SerializeField] float _sideDist;
    [SerializeField] float _shootInterval;
    [Header("\nMissile")]
    [SerializeField] float _missileCooltime;
    [SerializeField] float _missileInterval;
    [SerializeField] float _missileCount;
    [SerializeField] float _missileSpeed;
    [SerializeField] float _missileLastTime;
    [SerializeField] float _homingStartTime;
    [SerializeField] float _homingLastTime;
    [SerializeField] int _missileDamage;
    [SerializeField] float _explodeRadius;
    [Header("\nChain Explosion")]
    [SerializeField] float _explosionCooltime;
    [SerializeField] int _explosionDamage;
    [SerializeField] float _explosionWarningTime;
    [SerializeField] float _explosionLastTime;
    [SerializeField] float _explosionPosInterval;
    [SerializeField] int _excludeCount;
    [SerializeField] Vector2 _explosionSizeHalf;
    [Header("\nPhantom State")]
    [SerializeField] float _ghostInterval;
    [SerializeField] float _ghostLastTime;
    [SerializeField] Color _ghostColor;

    bool _isBackStepReady;
    bool _isChaseReady;
    bool _isRushReady;
    bool _isShootReady;
    bool _isMissileReady;
    bool _isChainExplosionReady;
    bool _isPhantomActivated;
    bool _isPhase2Reached;
    bool _isPhase3Reached;
    int _currentPhaseNum;
    Vector2[] _shootPos = { new Vector2(3.25f, -0.97f), new Vector2(-3.25f, -0.97f) };
    Vector2[] _missilePos = { new Vector2(-0.76f, 1.04f), new Vector2(0.76f, 1.04f) };
    Vector2 _chainExplosionCenterPos;
    List<Explosion> _explosionCache = new List<Explosion>();

    private int _phantomMultiplier = 1;

    private void Start() {
        _isSpawnComplete = true;
        _attackFuncList.Add(BackStepTask);
        _attackFuncList.Add(ChaseTask);
        _attackFuncList.Add(RushTask);
        _attackFuncList.Add(ShootTask);
        _attackFuncList.Add(MissileTask);
        _attackFuncList.Add(ChainExplosionTask);
        PhantomGhostEffect().Forget();
        _isPhase2Reached = false;
        _isPhase3Reached = false;
        _currentPhaseNum = 1;
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

    public override void Damaged(int dmg, bool isCrit)//플레이어 공격에 데미지를 입음
    {
        base.Damaged(dmg, isCrit);
        if (!_isPhase2Reached && CheckHpRatio(2.0f / 3.0f)) {
            _hpSlider.fillRect.GetComponent<Image>().color = _phase2HpColor;
            _hpSlider.transform.GetChild(0).GetComponent<Image>().color = _phase3HpColor;
            _currentPhaseNum = 2;
            _isPhase2Reached = true;
        }
        else if (!_isPhase3Reached && _isPhase2Reached && CheckHpRatio(1.0f / 3.0f)) {
            _hpSlider.fillRect.GetComponent<Image>().color = _phase3HpColor;
            _hpSlider.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            _currentPhaseNum = 3;
            _isPhase3Reached = true;
        }
        UpdateHpSlider();
    }

    #region BackStep
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
        _backStepEffect.GetComponent<Animator>().SetFloat("Phantom", _phantomMultiplier);
        _animator.SetBool("Run", true);
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration);
        _animator.SetBool("Run", false);
        _backStepEffect.SetActive(false);
        _rigid.velocity = Vector2.zero;
        _attackStateList[0] = AttackState.Finished;
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        _attackStateList[0] = AttackState.CoolTime;
    }
    #endregion

    #region Chase
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
        _dashEffect.GetComponent<Animator>().SetFloat("Phantom", _phantomMultiplier);
        _animator.SetBool("Run", true);
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration);
        _animator.SetBool("Run", false);
        _dashEffect.SetActive(false);
        _rigid.velocity = Vector2.zero;
        _attackStateList[1] = AttackState.Finished;
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        _attackStateList[1] = AttackState.CoolTime;
    }

    #endregion

    #region Rush
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
        _rushEffect.GetComponent<Animator>().SetFloat("Phantom", _phantomMultiplier);

        SetFlipX(rushDir);
        _animator.SetBool("Run", true);
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration);
        _animator.SetBool("Run", false);
        _rushEffect.SetActive(false);
        _rigid.velocity = Vector2.zero;
        _bodyDamage = originBodyDamage;
        _attackStateList[2] = AttackState.Finished;
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        _attackStateList[2] = AttackState.CoolTime;
    }

    #endregion

    #region Shoot
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
                float x = Random.Range(-_shootDispersionRate, _shootDispersionRate);
                float y = Random.Range(-_shootDispersionRate, _shootDispersionRate);
                FireBullet(fireDir, new Vector2(x, y));
                await UniTask.Delay(TimeSpan.FromSeconds(_bulletInterval / _phantomMultiplier));
            }
            cnt++;
            await UniTask.Delay(TimeSpan.FromSeconds(_shootInterval / _phantomMultiplier));
        }
        _attackStateList[3] = AttackState.Finished;
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        _attackStateList[3] = AttackState.CoolTime;
    }
    private async UniTaskVoid MoveSide() {
        Vector2 shootVec = _playerScript.transform.position - transform.position;
        Vector2 sideDir = new Vector2(shootVec.y, -shootVec.x).normalized;
        int randomN = Random.Range(0, 2);
        if (randomN == 1) {
            sideDir *= -1;
        }
        Vector2 endValue = (Vector2)transform.position + sideDir * _sideDist;
        float duration = _sideDist / _moveSpeed * 2;
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration);
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

    #endregion

    #region Missile
    private async UniTaskVoid MissileTask() {
        _isMissileReady = false;
        Missile().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_missileCooltime));
        _isMissileReady = true;
        _attackStateList[4] = AttackState.Ready;
    }

    private async UniTaskVoid Missile() {
        _attackStateList[4] = AttackState.Attacking;
        for (int i = 0; i < _missileCount; i++) {
            _animator.SetTrigger("Missile");
            FireHomingExplosive();
            if (_isPhantomActivated)
                await UniTask.Delay(TimeSpan.FromSeconds(_missileInterval / _phantomMultiplier));
            else
                await UniTask.Delay(TimeSpan.FromSeconds(_missileInterval));
        }
        _attackStateList[4] = AttackState.Finished;
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        _attackStateList[4] = AttackState.CoolTime;
    }

    private void FireHomingExplosive() {
        GameObject projectile = ObjectPool.Instance.GetObject(_missileObj);
        HomingBullet projectileScript = projectile.GetComponent<HomingBullet>();
        Vector2 targetDir = _playerScript.transform.position - transform.position;
        SetFlipX(targetDir);
        projectile.transform.right = Vector2.right;
        if (targetDir.x < 0)
            projectile.transform.right *= -1;
        projectile.transform.position = _missileTargetObj.transform.position;
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(_missileSpeed);
        projectileScript.SetDmg(_missileDamage);
        projectileScript.SetExplodeRadius(_explodeRadius);
        projectileScript.SetLastTime(_missileLastTime);
        projectileScript.SetHomingStartTime(_homingStartTime);
        projectileScript.SetHomingLastTime(_homingLastTime);
        projectile.SetActive(true);
    }

    #endregion

    #region Explosion
    private async UniTaskVoid ChainExplosionTask() {
        _isChainExplosionReady = false;
        ChainExplosion().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_explosionCooltime));
        _isChainExplosionReady = true;
        _attackStateList[5] = AttackState.Ready;
    }

    private async UniTaskVoid ChainExplosion() {
        _attackStateList[5] = AttackState.Attacking;
        float duration = CalculateManhattanDist(transform.position, _chainExplosionCenterPos) / _moveSpeed;
        SetFlipX(_chainExplosionCenterPos - (Vector2)transform.position);
        await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), _chainExplosionCenterPos, duration);
        SetFlipX(Vector2.left);
        _chargingEffect.GetComponent<SpriteRenderer>().color = Color.white;
        _chargingEffect.SetActive(true);
        List<Vector2> excludePosList = new List<Vector2>();
        for (int i = 0; i < _excludeCount; i++) {
            int x = Random.Range(-(int)_explosionSizeHalf.x, (int)_explosionSizeHalf.x + 1);
            int y = Random.Range(-(int)_explosionSizeHalf.y, (int)_explosionSizeHalf.y + 1);
            excludePosList.Add(new Vector2(x, y));
        }

        for (int i = (int)-_explosionSizeHalf.x; i < _explosionSizeHalf.x; i++) {
            for (int j = (int)-_explosionSizeHalf.y; j < _explosionSizeHalf.y; j++) {
                bool isExcludedPos = false;
                for (int k = 0; k < excludePosList.Count; k++) {
                    if (excludePosList[k].x == i && excludePosList[k].y == j) {
                        isExcludedPos = true;
                        break;
                    }
                }

                if (!isExcludedPos) {
                    var explosion = ObjectPool.Instance.GetObject(_explosionObj);
                    _explosionCache.Add(explosion.GetComponent<Explosion>());
                    explosion.transform.position = transform.position + new Vector3(i, j) * _explosionPosInterval;
                    explosion.SetActive(true);
                }
            }
        }
        await UniTask.Delay(TimeSpan.FromSeconds(_explosionWarningTime));
        for (int i = 0; i < _explosionCache.Count; i++) {
            _explosionCache[i].SetPlayer(_playerScript);
            _explosionCache[i].SetDamage(_explosionDamage);
            _explosionCache[i].ExplodeTrigger();
        }
        await UniTask.Delay(TimeSpan.FromSeconds(_explosionLastTime));
        for (int i = 0; i < _explosionCache.Count; i++) {
            _explosionCache[i].FadeTrigger();
        }
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        for (int i = 0; i < _explosionCache.Count; i++) {
            ObjectPool.Instance.ReturnObject(_explosionCache[i].gameObject);
        }
        _chargingEffect.GetComponent<SpriteRenderer>().DOFade(0, 1).ToUniTask().Forget();

        _attackStateList[5] = AttackState.Finished;
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        _attackStateList[5] = AttackState.CoolTime;
    }

    #endregion

    private async UniTaskVoid Groggy() {
        _animator.SetTrigger("Groggy");
    }

    #region Phantom State
    private async UniTaskVoid PhantomGhostEffect() {
        _isPhantomActivated = true;
        _phantomMultiplier = 4;
        _animator.SetFloat("Phantom", _phantomMultiplier);
        _moveSpeed *= _phantomMultiplier;
        while (_isPhantomActivated) {
            SinglePhantomGhost().Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_ghostInterval));
        }
        _phantomMultiplier = 1;
        _animator.SetFloat("Phantom", _phantomMultiplier);
        _moveSpeed *= _phantomMultiplier;
    }

    private async UniTaskVoid SinglePhantomGhost() {
        GameObject ghostEffect = ObjectPool.Instance.GetObject(_phantomGhostObj);
        SpriteRenderer sr = ghostEffect.GetComponent<SpriteRenderer>();
        sr.color = _ghostColor;
        //sr.sprite = _spriteRenderer.sprite;      
        sr.flipX = _spriteRenderer.flipX;
        ghostEffect.transform.position = transform.position;
        ghostEffect.SetActive(true);
        await sr.DOFade(0, _ghostLastTime);
        ObjectPool.Instance.ReturnObject(ghostEffect);
    }

    #endregion

    private float CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void SetFlipX(Vector2 moveDir) {
        if (moveDir.x < 0) {
            _spriteRenderer.flipX = true;
            _legSr.flipX = true;
            _armTargetObj.transform.localPosition = _shootPos[1];
            _missileTargetObj.transform.localPosition = _missilePos[1];
            _dashEffect.GetComponent<SpriteRenderer>().flipX = true;
            _backStepEffect.GetComponent<SpriteRenderer>().flipX = false;
        }

        else {
            _spriteRenderer.flipX = false;
            _legSr.flipX = false;
            _armTargetObj.transform.localPosition = _shootPos[0];
            _missileTargetObj.transform.localPosition = _missilePos[0];
            _dashEffect.GetComponent<SpriteRenderer>().flipX = false;
            _backStepEffect.GetComponent<SpriteRenderer>().flipX = true;
        }

    }

    private void UpdateHpSlider() {
        int phaseCount = 3 - _currentPhaseNum;
        float maxHp = _maxHp / 3;
        float hp = _hp - maxHp * phaseCount;
        _hpSlider.value = hp / maxHp;
    }

    /// <summary>
    /// Check if current Hp ratio is less than input ratio
    /// </summary>
    /// <param name="ratio"></param>
    /// <returns></returns>
    private bool CheckHpRatio(float ratio) {
        if ((float)_hp / (float)_maxHp < ratio)
            return true;
        else
            return false;
    }
}
