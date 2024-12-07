using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossBondrewd : MonsterBase {
    #region Serialize Field
    [SerializeField] GameObject _dashEffect;
    [SerializeField] GameObject _rushWarningEffect;
    [SerializeField] List<GameObject> _rushEffectList;
    [SerializeField] GameObject _backStepEffect;
    [SerializeField] GameObject _chargingEffect;
    [SerializeField] GameObject _groggyEffect;
    [SerializeField] GameObject _bulletObj;
    [SerializeField] GameObject _missileObj;
    [SerializeField] GameObject _armTargetObj;
    [SerializeField] GameObject _missileTargetObj;
    [SerializeField] GameObject _explosionObj;
    [SerializeField] GameObject _explosionCenterFlag;
    [SerializeField] GameObject _phantomGhostObj;
    [SerializeField] List<GameObject> _chargerList;

    //[SerializeField] AudioClip _dashSound;
    [SerializeField] SpriteRenderer _legSr;
    [SerializeField] GameObject _bossUI;
    [SerializeField] Slider _hpSlider;
    [SerializeField] Slider _phantomGaugeSlider;
    [SerializeField] Slider _groggySlider;
    [SerializeField] TextMeshProUGUI _timerTM;
    [SerializeField] List<Image> _chargerUIList;
    [SerializeField] Color _phase2HpColor;
    [SerializeField] Color _phase3HpColor;
    [SerializeField] Color _phantomGaugeActivateColor;
    [SerializeField] Color _phantomGaugeDeactivateColor;
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
    [SerializeField] int _groggyDecreaseAmount;
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
    [SerializeField] int _phantomGaugeIncreaseAmount;
    [SerializeField] int _phantomGaugeDecreaseAmount;
    [SerializeField] float _phantomGaugeUpdateInterval;
    [Header("\nGroggy State")]
    [SerializeField] int _normalGroggyDecreaseAmount;
    [SerializeField] float _groggyLastTime;
    #endregion

    #region Private variables

    CancellationTokenSource _cts;
    bool _isBackStepReady;
    bool _isChaseReady;
    bool _isRushReady;
    bool _isShootReady;
    bool _isMissileReady;
    bool _isRushing;
    bool _isChainExplosionReady;
    bool _isChainExplosionActivated; //for prevent stun when chain explosion pattern
    bool _isChainExplosionWarning;
    bool _isPhase2Reached;
    bool _isPhase3Reached;
    bool _isGroggy;
    int _currentPhaseNum;
    float _phantomGauge;
    float _groggyGauge;
    int _currentActivatedChargerCount;
    BoxCollider2D _bondrewdCollider;
    Vector2[] _shootPos = { new Vector2(3.25f, -0.97f), new Vector2(-3.25f, -0.97f) };
    Vector2[] _missilePos = { new Vector2(-0.76f, 1.04f), new Vector2(0.76f, 1.04f) };
    List<Explosion> _explosionCache = new List<Explosion>();

    int _phantomMultiplier;
    PhantomState _currentPhantomState;
    float _phantomTimer;

    enum PhantomState {
        DeActivated,
        Activated
    }

    #endregion

    #region Mono funcs
    private void Start() {
        _cts = new CancellationTokenSource();
        _bondrewdCollider = GetComponent<BoxCollider2D>();
        _isSpawnComplete = true;
        _attackFuncList.Add(BackStepTask);
        _attackFuncList.Add(ChaseTask);
        _attackFuncList.Add(RushTask);
        _attackFuncList.Add(ShootTask);
        _attackFuncList.Add(MissileTask);
        _attackFuncList.Add(ChainExplosionTask);
        _isPhase2Reached = false;
        _isPhase3Reached = false;
        _isChainExplosionActivated = false;
        _currentPhaseNum = 1;
        _phantomMultiplier = 1;
        _animator.SetFloat("Phantom", _phantomMultiplier);
        _phantomGauge = 0;
        _currentPhantomState = PhantomState.DeActivated;
        _phantomTimer = 0;
        _currentActivatedChargerCount = 0;
        _groggyGauge = 100;
        _isRushing = false;
        _timerTM.gameObject.SetActive(false);
        UpdateGroggySlider();
    }
    new private void Update() {
        base.Update();
        PhantomManage();
        UpdateHpSlider();
        UpdateGroggySlider();
        UpdatePhantomGaugeSlider();
    }
    protected override void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player") && !_isDead && _isSpawnComplete & _isRushing)
            _playerScript.Damaged(_rushDamage);
    }
    #endregion

    #region Override Funcs

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

    public override void Damaged(int dmg, bool isCrit, bool isPoison = false)//플레이어 공격에 데미지를 입음
    {
        base.Damaged(dmg, isCrit);
        DecreaseGroggyGauge(_normalGroggyDecreaseAmount);
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

    public async override UniTaskVoid Stun(float lastTime, float offsetY = 2) {
        if (!_isChainExplosionActivated) {
            _cts.Cancel();
            base.Stun(lastTime, 2).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
            _cts.Dispose();
            _cts = null;
            _cts = new CancellationTokenSource();
        }
        else {
            SystemMessageManager.Instance.PushSystemMessage("보스가 일시적으로 스턴에 걸리지 않습니다.", Color.yellow);
        }
    }

    public async override UniTaskVoid Slow(float minusRate, float lastTime) {
        await UniTask.NextFrame();
    }

    protected override async UniTaskVoid Die() {
        _cts.Cancel();
        _isDead = true;
        _hp = 0;
        _animator.SetTrigger("Die");
        _spriteRenderer.sortingLayerName = "Default";
        _legSr.sortingLayerName = "Default";
        _bondrewdCollider.enabled = false;
        _bossUI.SetActive(false);
        _groggyEffect.SetActive(false);
        _chargerUIList.ForEach((chargeUI) => chargeUI.gameObject.SetActive(false));
        DropItems();
        _playerScript.GetExp(_exp);
        PlayerDataManager.Instance.SetFeverGauge(PlayerDataManager.Instance.Status._feverGauge + _feverAmount);
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        _cts.Dispose();
        _cts = null;

    }

    #endregion

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
        try {
            //SoundManager.Instance.PlaySFX(_dashSound.name);
            await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration).WithCancellation(_cts.Token);
        }
        catch (OperationCanceledException) {
        }
        finally {
            _animator.SetBool("Run", false);
            _backStepEffect.SetActive(false);
            _rigid.velocity = Vector2.zero;
            _attackStateList[0] = AttackState.Finished;
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
            _attackStateList[0] = AttackState.CoolTime;
        }
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
        try {
            //SoundManager.Instance.PlaySFX(_dashSound.name);
            await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration).WithCancellation(_cts.Token);
        }
        catch (OperationCanceledException) {
        }
        finally {
            _animator.SetBool("Run", false);
            _dashEffect.SetActive(false);
            _rigid.velocity = Vector2.zero;
            _attackStateList[1] = AttackState.Finished;
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
            _attackStateList[1] = AttackState.CoolTime;
        }
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
        int currentPhaseNum = _currentPhaseNum;
        for (int i = 0; i < currentPhaseNum; i++) {
            _rushWarningEffect.SetActive(true);
            float rushWarningTime = _rushWarningTime - (_currentPhaseNum - 1) * 0.25f;
            float timer = 0;
            //int originBodyDamage = _bodyDamage;
            try {
                while (timer < rushWarningTime - 1) {
                    Vector2 tempDir = _playerScript.transform.position - transform.position;
                    SetFlipX(tempDir);
                    _rushWarningEffect.transform.right = tempDir;
                    timer += Time.deltaTime;
                    await UniTask.NextFrame(_cts.Token);
                }
                Vector2 rushDir = (_playerScript.transform.position - transform.position).normalized;
                Vector2 endValue = (Vector2)transform.position + rushDir * _rushDist;
                float duration = _rushDist / _moveSpeed;
                await UniTask.Delay(TimeSpan.FromSeconds(1 - (_currentPhaseNum - 1) * 0.35f), cancellationToken: _cts.Token);
                _rushWarningEffect.SetActive(false);
                //_bodyDamage = _rushDamage;
                _rushEffectList[currentPhaseNum - 1].transform.right = rushDir;
                _rushEffectList[currentPhaseNum - 1].SetActive(true);
                _rushEffectList[currentPhaseNum - 1].GetComponent<Animator>().SetFloat("Phantom", _phantomMultiplier);

                SetFlipX(rushDir);
                _animator.SetBool("Run", true);
                _isRushing = true;
                await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration).WithCancellation(_cts.Token);
                _isRushing = false;
            }
            catch (OperationCanceledException) {
                _rushWarningEffect.SetActive(false);
                _attackStateList[2] = AttackState.Finished;
                await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
                _attackStateList[2] = AttackState.CoolTime;
            }
            finally {
                _animator.SetBool("Run", false);
                _rushEffectList[currentPhaseNum - 1].SetActive(false);
                _rigid.velocity = Vector2.zero;
                //_bodyDamage = originBodyDamage;
            }
        }
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
        try {
            while (cnt < _shootCount + _currentPhaseNum - 1) {
                MoveSide().Forget();
                _animator.SetTrigger("ShootRun");
                Vector3 fireDir = _playerScript.transform.position - _armTargetObj.transform.position;
                Vector3 lookAt = _playerScript.transform.position - transform.position;
                SetFlipX(lookAt);
                for (int i = 0; i < _bulletCount; i++) {
                    float x = Random.Range(-_shootDispersionRate, _shootDispersionRate);
                    float y = Random.Range(-_shootDispersionRate, _shootDispersionRate);
                    FireBullet(fireDir, new Vector2(x, y));
                    await UniTask.Delay(TimeSpan.FromSeconds(_bulletInterval / _phantomMultiplier), cancellationToken: _cts.Token);
                }
                cnt++;
                await UniTask.Delay(TimeSpan.FromSeconds(_shootInterval / _phantomMultiplier), cancellationToken: _cts.Token);
            }

        }
        catch (OperationCanceledException) {
        }
        finally {
            _attackStateList[3] = AttackState.Finished;
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
            _attackStateList[3] = AttackState.CoolTime;
        }
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
        try {
            await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), endValue, duration).WithCancellation(_cts.Token);
        }
        catch (OperationCanceledException) {
        }
        finally {
            _rigid.velocity = Vector2.zero;
        }
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
        try {
            for (int i = 0; i < _missileCount; i++) {
                _animator.SetTrigger("Missile");
                FireHomingExplosive();
                await UniTask.Delay(TimeSpan.FromSeconds(_missileInterval / _phantomMultiplier), cancellationToken: _cts.Token);
            }
        }
        catch (OperationCanceledException) {
        }
        finally {
            _attackStateList[4] = AttackState.Finished;
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
            _attackStateList[4] = AttackState.CoolTime;
        }
    }

    private void FireHomingExplosive() {
        GameObject projectile = ObjectPool.Instance.GetObject(_missileObj);
        BondrewdMissile projectileScript = projectile.GetComponent<BondrewdMissile>();
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
        projectileScript.SetGroggyDecreaseAmount(_groggyDecreaseAmount);
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
        _isChainExplosionActivated = true;
        Vector2 chainExplosionCenterPos = _explosionCenterFlag.transform.position;
        float duration = CalculateManhattanDist(transform.position, chainExplosionCenterPos) / _moveSpeed;
        SetFlipX(chainExplosionCenterPos - (Vector2)transform.position);
        _animator.SetBool("Run", true);
        try {
            await DOTween.To(() => _rigid.position, x => _rigid.MovePosition(x), chainExplosionCenterPos, duration).WithCancellation(_cts.Token);
            _animator.SetBool("Run", false);
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

            float timer = _explosionWarningTime;
            _isChainExplosionWarning = true;
            _timerTM.gameObject.SetActive(true);
            while (timer > 0) {
                timer -= Time.deltaTime;
                int sec = Mathf.FloorToInt(timer);
                if (sec < 0)
                    sec = 0;
                _timerTM.text = $"00 : {sec:D2}";
                await UniTask.NextFrame(_cts.Token);
            }
            _isChainExplosionWarning = false;

            for (int i = 0; i < _explosionCache.Count; i++) {
                _explosionCache[i].SetPlayer(_playerScript);
                _explosionCache[i].SetDamage(_explosionDamage);
                _explosionCache[i].ExplodeTrigger();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_explosionLastTime), cancellationToken: _cts.Token);
        }
        catch (OperationCanceledException) {
        }
        finally {
            for (int i = 0; i < _explosionCache.Count; i++) {
                _explosionCache[i].FadeTrigger();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            for (int i = 0; i < _explosionCache.Count; i++) {
                ObjectPool.Instance.ReturnObject(_explosionCache[i].gameObject);
            }
            await _chargingEffect.GetComponent<SpriteRenderer>().DOFade(0, 1);
            _chargingEffect.SetActive(false);
            _isChainExplosionWarning = false;
            _isChainExplosionActivated = false;
            _timerTM.gameObject.SetActive(false);
            _attackStateList[5] = AttackState.Finished;
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
            _attackStateList[5] = AttackState.CoolTime;
        }
    }

    public void ShowExplosionWarning() {
        for (int i = 0; i < _explosionCache.Count; i++) {
            _explosionCache[i].WarningTrigger();
        }
    }

    public bool CheckExplosionWarning() {
        return _isChainExplosionWarning;
    }

    #endregion

    #region Phantom State

    private void PhantomManage() {
        switch (_currentPhantomState) {
            case PhantomState.DeActivated:
                if (_phantomGauge > 100) {
                    _phantomTimer = 0;
                    _currentPhantomState = PhantomState.Activated;
                    _phantomGauge = 100;
                    _phantomGaugeSlider.fillRect.GetComponent<Image>().color = _phantomGaugeActivateColor;

                    _phantomMultiplier = 4;
                    _animator.SetFloat("Phantom", _phantomMultiplier);
                    _moveSpeed *= _phantomMultiplier;

                    PhantomGhostEffect().Forget();
                }

                if (_phantomTimer < _phantomGaugeUpdateInterval) {
                    _phantomTimer += Time.deltaTime;
                }
                else {
                    _phantomGauge += _phantomGaugeIncreaseAmount * _currentActivatedChargerCount * 0.01f;
                    UpdatePhantomGaugeSlider();
                    _phantomTimer = 0;
                }

                break;
            case PhantomState.Activated:
                if (_phantomGauge < 0) {
                    _phantomTimer = 0;
                    _currentPhantomState = PhantomState.DeActivated;
                    _phantomGauge = 0;
                    _phantomGaugeSlider.fillRect.GetComponent<Image>().color = _phantomGaugeDeactivateColor;

                    _moveSpeed /= _phantomMultiplier;
                    _phantomMultiplier = 1;
                    _animator.SetFloat("Phantom", _phantomMultiplier);
                }

                if (_phantomTimer < _phantomGaugeUpdateInterval) {
                    _phantomTimer += Time.deltaTime;
                }
                else {
                    _phantomGauge -= _phantomGaugeDecreaseAmount * 0.01f;
                    UpdatePhantomGaugeSlider();
                    _phantomTimer = 0;
                }
                break;
        }
    }
    private async UniTaskVoid PhantomGhostEffect() {
        while (_currentPhantomState == PhantomState.Activated && !_isDead) {
            SinglePhantomGhost().Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_ghostInterval));
        }
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

    #region Groggy State

    public void DecreaseGroggyGauge(int decreaseAmount) {
        if (!_isGroggy) {
            _groggyGauge -= decreaseAmount;
            UpdateGroggySlider();
            if (_groggyGauge <= 0) {
                _groggyGauge = 0;
                Groggy().Forget();
            }
        }
    }

    private async UniTaskVoid Groggy() {
        Debug.Log("Groggy Start");
        _isGroggy = true;
        _cts.Cancel();
        _useTree = false;
        _groggyEffect.SetActive(true);
        StateEffectManager.Instance.SummonEffect(transform, StateType.Confusion, 3, _groggyLastTime, 3).Forget();
        float timer = 0;
        while (timer < _groggyLastTime) {
            _groggyGauge += 100 / _groggyLastTime * Time.deltaTime;
            UpdateGroggySlider();
            timer += Time.deltaTime;
            await UniTask.NextFrame();
        }
        _groggyGauge = 100;
        _cts.Dispose();
        _cts = null;
        _cts = new CancellationTokenSource();
        _groggyEffect.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        _isGroggy = false;
        _useTree = true;
        Debug.Log("Groggy End");
    }

    #endregion

    #region Charger State

    public void ChargerCntIncrease(int index) {
        _currentActivatedChargerCount++;
        _chargerUIList[index].DOColor(Color.white, 1);
    }

    public void ChargerCntDecrease(int index) {
        _currentActivatedChargerCount--;
        _chargerUIList[index].DOColor(Color.gray, 1);
    }

    #endregion

    #region Util funcs
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

    private void UpdatePhantomGaugeSlider() {
        _phantomGaugeSlider.value = _phantomGauge / 100.0f;
    }
    private void UpdateGroggySlider() {
        _groggySlider.value = _groggyGauge / 100.0f;
    }
    #endregion
}
