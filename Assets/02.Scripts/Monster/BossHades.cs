using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossHades : MonsterBase {

    #region serialize field

    [SerializeField] Transform _cameraTarget;
    [SerializeField] private GameObject inGameUI; // InGameUI 오브젝트
    [SerializeField] Animator _ballAnimator;
    [SerializeField] float _patternCooltime;
    [SerializeField] SpriteRenderer _headSr;
    [SerializeField] SpriteRenderer _jawSr;
    [SerializeField] SpriteRenderer _insideSr;
    [SerializeField] SpriteRenderer _BodySr;
    [SerializeField] Color _phase2HpColor;
    [SerializeField] private GameObject bossUI; // 전체 보스 UI (bossImage1, bossImage2, bossText 포함)
    [SerializeField] private Image bossImage1;
    [SerializeField] private Image bossImage2;
    [SerializeField] private TMP_Text bossText;
    [SerializeField] private float fadeDuration = 1f; // 페이드 인 시간
    [SerializeField] private float delayBetweenUI = 1f; // 텍스트가 나타나는 딜레이

    [Header("\nFire Pattern Variable")]
    [SerializeField] GameObject _projectileObj1;
    [SerializeField] GameObject _projectileObj1_S;
    [SerializeField] int _projectileDamage1;
    [SerializeField] int _projectileDamage1_S;
    [SerializeField] float _projectileSpeed1;
    [SerializeField] float _projectileSpeed1_S;
    [SerializeField] float _projectileRange1;
    [SerializeField] float _pattern1Delay;
    [SerializeField] float _pattern1Delay_S;

    [Header("\nSpike Pattern Variable")]
    [SerializeField] GameObject _spikeObj;
    [SerializeField] int _damage2;
    [SerializeField] int _damage2_S;
    [SerializeField] float _stunTime2;
    [SerializeField] float _warningTime2_1;
    [SerializeField] float _pattern2Delay_1;

    [Header("\nSpike Around Pattern Variable")]
    [SerializeField] float _stunTime2_2;
    [SerializeField] float _warningTime2_2;
    [SerializeField] List<float> _radiusListX;
    [SerializeField] List<float> _radiusLIstY;
    [SerializeField] List<int> _spikeNumList;
    [SerializeField] float _pattern2Delay_2;
    [SerializeField] float _spikeAroundPatternCooltime;

    [Header("\nLaser Pattern Variable")]
    [SerializeField] GameObject _laserObj;
    [SerializeField] float _laserWidth;
    [SerializeField] int _damage3;
    [SerializeField] int _damage3_S;
    [SerializeField] float _chargeDelay1;
    [SerializeField] float _chargeDelay2;
    [SerializeField] float _laserLastTime1;
    [SerializeField] float _laserLastTime2;
    [SerializeField] float _laserDelay1;
    [SerializeField] float _laserDelay2;
    [SerializeField] List<Vector2> _laserDataList1;
    [SerializeField] List<LaserData> _laserDataList2_1;
    [SerializeField] List<LaserData> _laserDataList2_2;
    [SerializeField] float _laserPatternCooltime;

    [Header("\nSlow Pattern Variable")]
    [SerializeField] GameObject _projectileObj4;
    [SerializeField] GameObject _projectileObj4_S;
    [SerializeField] int _projectileDamage4;
    [SerializeField] int _projectileDamage4_S;
    [SerializeField] float _projectileSpeed4;
    [SerializeField] float _projectileSpeed4_S;
    [SerializeField] float _projectileRange4;
    [SerializeField] float _slowRate4;
    [SerializeField] float _slowRate4_S;
    [SerializeField] float _slowTime4;
    [SerializeField] float _pattern4Delay;
    [SerializeField] float _pattern4Delay_S;

    [Header("\nPillar Pattern Variable")]
    [SerializeField] GameObject _pillarObj5;
    [SerializeField] GameObject _warningEffectObj5;
    [SerializeField] GameObject _patternEffectObj5;
    [SerializeField] GameObject _volumeObj;
    [SerializeField] List<Vector2> _pillarPos5;
    [SerializeField] int _defPlus5;
    [SerializeField] int _damage5;
    [SerializeField] float _range5;
    [SerializeField] float _waitTime5;
    [SerializeField] float _warningTime5;
    [SerializeField] float _pillarPatternCooltime;

    [Header("\nGolem Pattern Variable")]
    [SerializeField] GameObject _golemObj;
    [SerializeField] List<Vector2> _golemPosList;
    [SerializeField] float _golemPatternCooltime;

    [Header("\nSound")]
    [SerializeField] List<AudioClip> _clipList;
    #endregion




    #region private variable

    LayerMask _playerLayer;
    LayerMask _enemyLayer;
    CancellationTokenSource _cts;
    TextMeshProUGUI _timeUI5;
    Collider2D _playerCollider;
    Collider2D _hadesCollider;
    Slider _hpSlider;
    SpriteRenderer _warningEffectSpriteRenderer5;
    static int _pillarCnt;
    int _originDef;
    bool _isSleep;
    bool _HalfTrigger;
    bool _isSpikeAroundReady;
    bool _isLaserReady;
    bool _isGolemReady;
    bool _isPillarReady;
    static List<bool> _isPosUsing = new List<bool>(8);
    float _hpSliderState;

    [SerializeField] private AudioClip audioSource_awakeSound;
    #endregion





    #region mono funcs

    private void Start() {
        _cts = new CancellationTokenSource();
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        _playerLayer = LayerMask.GetMask("Player");
        _enemyLayer = LayerMask.GetMask("Enemy", "CollidableEnemy");
        _hadesCollider = GetComponent<Collider2D>();
        _isSleep = true;
        _HalfTrigger = true;
        _isSpikeAroundReady = false;
        _isLaserReady = false;
        _isGolemReady = true;
        _isPillarReady = false;
        for (int i = 0; i < 8; i++)
            _isPosUsing.Add(false);
        _timeUI5 = GameObject.Find("BossTimer").GetComponent<TextMeshProUGUI>();
        _timeUI5.gameObject.SetActive(false);
        _warningEffectObj5.transform.localScale = new Vector3(_range5, _range5, 1);
        _pillarCnt = -1;
        _playerCollider = _playerScript.GetComponent<Collider2D>();
        _warningEffectSpriteRenderer5 = _warningEffectObj5.GetComponent<SpriteRenderer>();
        _originDef = _def;
        _hpSlider = GameObject.Find("BossHpSlider").GetComponent<Slider>();
        _hpSlider.gameObject.SetActive(false);
        _ballAnimator.SetFloat("Speed", 0.3f);
        RandomPattern(_cts.Token).Forget();
        inGameUI = GameObject.Find("InGameUI");
    }

    #endregion





    #region public funcs

    public static void PillarCntUp() { _pillarCnt++; }

    public static void ReturnGolemPosUsing(int index) { _isPosUsing[index] = false; }

    public override void Damaged(int dmg, bool isCrit)//플레이어 공격에 데미지를 입음
    {
        if (_isSleep) {
            PlaySound(audioSource_awakeSound);
            _animator.SetTrigger("Awake");
            CameraManager.Instance.CameraFocus(_cameraTarget, 9, 4).Forget();
            inGameUI.SetActive(false); // BossUI가 켜질 때 InGameUI 비활성화
            bossUI.SetActive(true); // 보스가 깨어날 때 UI 활성화
            StartCoroutine(FadeInOutBossUI()); // UI Fade In/Out 시작
        }
        else {
            base.Damaged(dmg, isCrit);
            if (_HalfTrigger && CheckHalfHp()) {
                _animator.SetTrigger("Phase2");
                _hpSlider.fillRect.GetComponent<Image>().color = _phase2HpColor;
                _hpSlider.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                _ballAnimator.SetFloat("Speed", 1f);
                _HalfTrigger = false;
            }
        }
        UpdateHpSlider();
    }
    #endregion





    #region protected funcs

    protected override void AttackMove() {

    }

    protected override async UniTaskVoid ChangeColor()//피격 시 붉은 색으로 몬스터 색 변경
    {
        if (!_isColorChanged) {
            _isColorChanged = true;
            _headSr.color = _damagedColor;
            _jawSr.color = _damagedColor;
            _insideSr.color = _damagedColor;
            _BodySr.color = _damagedColor;
            await UniTask.Delay(TimeSpan.FromSeconds(_colorChanageLastTime));
            _headSr.color = Color.white;
            _jawSr.color = Color.white;
            _insideSr.color = Color.white;
            _BodySr.color = Color.white;
            _isColorChanged = false;
        }
    }

    protected override async UniTaskVoid Die() {
        _cts.Cancel();
        _isDead = true;
        _hp = 0;
        _animator.SetTrigger("Die");
        _hpSlider.gameObject.SetActive(false);
        PlaySoundDelay(_deathSound, 1.2f).Forget();
        await CameraManager.Instance.CameraFocus(_cameraTarget, 9, 5.5f);
        _hadesCollider.enabled = false;
        _headSr.sortingLayerName = "Default";
        _jawSr.sortingLayerName = "Default";
        _insideSr.sortingLayerName = "Default";
        _BodySr.sortingLayerName = "Default";
        DropItems();
        _playerScript.GetExp(_exp);
        _cts.Dispose();
        _cts = null;
        PlayerDataManager.Instance.SetFeverGauge(PlayerDataManager.Instance.Status._feverGauge + _feverAmount);
    }
    #endregion





    #region private funcs

    private async UniTaskVoid RandomPattern(CancellationToken cancellationToken) {
        await UniTask.WaitUntil(() => !_isSleep);

        InitialPillarTimer(cancellationToken).Forget();
        InitialLaserTimer(cancellationToken).Forget();
        InitialSpikeAroundTimer(cancellationToken).Forget();

        while (!_isDead) {
            if (_isGolemReady)
                SummonGolems(cancellationToken).Forget();
            else if (_isPillarReady)
                PillarPattern(cancellationToken).Forget();
            else if (_isLaserReady) {
                LaserPatternRandom(cancellationToken).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(7), cancellationToken: cancellationToken);
            }
            else if (_isSpikeAroundReady) {
                SpikeAroundPattern(cancellationToken).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cancellationToken);
            }

            else {
                int randomN;
                UniTask patternTask = UniTask.CompletedTask;
                Random.InitState((int)DateTime.Now.Ticks);
                randomN = Random.Range(0, 3);

                if (!CheckHalfHp()) {
                    switch (randomN) {
                        case 0:
                            patternTask = FirePattern();
                            break;
                        case 1:
                            patternTask = SpikePattern1(cancellationToken);
                            break;
                        case 2:
                            patternTask = SlowPattern();
                            break;
                        default:
                            break;
                    }
                }
                else {
                    switch (randomN) {
                        case 0:
                            patternTask = FirePattern_S(cancellationToken);
                            break;
                        case 1:
                            patternTask = SpikePattern1_S(cancellationToken);
                            break;
                        case 2:
                            patternTask = SlowPattern_S(cancellationToken);
                            break;
                        default:
                            break;
                    }
                }
                await patternTask;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_patternCooltime), cancellationToken: cancellationToken);
        }
    }

    #region Fire Pattern

    private async UniTask FirePattern() {
        _animator.SetTrigger("Open");
        _ballAnimator.SetFloat("Speed", 2f);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        for (int i = 0; i < 8; i++) {
            FireAround(i * 18);
            await UniTask.Delay(TimeSpan.FromSeconds(_pattern1Delay));
        }
        _ballAnimator.SetFloat("Speed", 0.3f);
        _animator.SetTrigger("Close");
    }

    private async UniTask FirePattern_S(CancellationToken cancellationToken) {
        _animator.SetTrigger("Open");
        _ballAnimator.SetFloat("Speed", 3f);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
        for (int i = 0; i < 10; i++) {
            FireAround_S(i * 9);
            await UniTask.Delay(TimeSpan.FromSeconds(_pattern1Delay_S), cancellationToken: cancellationToken);
        }
        _ballAnimator.SetFloat("Speed", 1f);
        _animator.SetTrigger("Close");
    }

    private void FireAround(int offset) {
        int i;
        for (i = 0; i < 10; i++)
            Fire(i * 36 + offset);
        PlaySound(_clipList[0]);
    }

    private void FireAround_S(int offset) {
        int i;
        for (i = 0; i < 20; i++)
            Fire_S(i * 18 + offset);
        PlaySound(_clipList[0]);
    }

    private void Fire(float angle) {
        GameObject projectile = ObjectPool.Instance.GetObject(_projectileObj1);
        Bullet projectileScript = projectile.GetComponent<Bullet>();
        Vector2 fireDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        projectile.transform.right = fireDir;
        projectile.transform.position = transform.position + projectile.transform.right;
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(_projectileSpeed1);
        projectileScript.SetDmg(_projectileDamage1);
        float lastTime = _projectileRange1 / _projectileSpeed1;
        projectileScript.SetLastTime(lastTime);
        projectile.SetActive(true);
    }

    private void Fire_S(float angle) {
        GameObject projectile = ObjectPool.Instance.GetObject(_projectileObj1_S);
        Bullet projectileScript = projectile.GetComponent<Bullet>();
        Vector2 fireDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        projectile.transform.right = fireDir;
        projectile.transform.position = transform.position + projectile.transform.right;
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(_projectileSpeed1_S);
        projectileScript.SetDmg(_projectileDamage1_S);
        float lastTime = _projectileRange1 / _projectileSpeed1_S;
        projectileScript.SetLastTime(lastTime);
        projectile.SetActive(true);
    }

    #endregion //Fire Pattern

    #region Spike Pattern

    private async UniTask SpikePattern1(CancellationToken cancellationToken) {
        _animator.SetTrigger("Spike");
        await UniTask.Delay(TimeSpan.FromSeconds(1.2f), cancellationToken: cancellationToken);
        for (int i = 0; i < 5; i++) {
            Vector2 targetPos = _playerScript.transform.position;
            SummonSpike(targetPos, _warningTime2_1, _damage2, _stunTime2, true, cancellationToken).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_pattern2Delay_1), cancellationToken: cancellationToken);
        }
    }

    private async UniTask SpikePattern1_S(CancellationToken cancellationToken) {
        _animator.SetTrigger("Spike");
        await UniTask.Delay(TimeSpan.FromSeconds(1.2f), cancellationToken: cancellationToken);
        float delay = _pattern2Delay_1;
        for (int i = 0; i < 10; i++) {
            Vector2 targetPos = _playerScript.transform.position;
            SummonSpike(targetPos, _warningTime2_1, _damage2_S, _stunTime2, true, cancellationToken).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            delay -= 0.06f;
        }
    }

    private async UniTaskVoid SpikeAroundPattern(CancellationToken cancellationToken) {
        if (_isSpikeAroundReady) {
            _isSpikeAroundReady = false;
            _animator.SetTrigger("Spike");
            await UniTask.Delay(TimeSpan.FromSeconds(1.2f), cancellationToken: cancellationToken);

            if (!CheckHalfHp())
                await SpikePattern2(cancellationToken);
            else
                await SpikePattern2_S(cancellationToken);

            await UniTask.Delay(TimeSpan.FromSeconds(_spikeAroundPatternCooltime), cancellationToken: cancellationToken);
            _isSpikeAroundReady = true;
        }
    }

    private async UniTaskVoid InitialSpikeAroundTimer(CancellationToken cancellationToken) {
        await UniTask.Delay(TimeSpan.FromSeconds(_spikeAroundPatternCooltime), cancellationToken: cancellationToken);
        _isSpikeAroundReady = true;
    }

    private async UniTask SpikePattern2(CancellationToken cancellationToken) {
        float angle, radiusX, radiusY;
        int spikeNum;
        for (int i = 0; i < 3; i++) {
            //angle = 10 * i;
            angle = 0;
            radiusX = _radiusListX[i];
            radiusY = _radiusLIstY[i];
            spikeNum = _spikeNumList[i];
            for (int j = 0; j < spikeNum; j++) {
                Vector2 spikePos = new Vector2(radiusX * Mathf.Cos(Mathf.Deg2Rad * angle), radiusY * Mathf.Sin(Mathf.Deg2Rad * angle));
                SummonSpike(spikePos + (Vector2)transform.position + Vector2.down * 3, _warningTime2_2, _damage2_S, _stunTime2_2, false, cancellationToken).Forget();
                angle += (float)360 / (float)spikeNum;
            }
            for (int j = 0; j < 4; j++)
                PlaySoundDelay(_clipList[2], 0.8f).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_pattern2Delay_2), cancellationToken: cancellationToken);
        }
    }

    private async UniTask SpikePattern2_S(CancellationToken cancellationToken) {
        float angle, radiusX, radiusY;
        int spikeNum;
        for (int i = 0; i < 4; i++) {
            //angle = 10 * i;
            angle = 0;
            radiusX = _radiusListX[i];
            radiusY = _radiusLIstY[i];
            spikeNum = _spikeNumList[i];
            for (int j = 0; j < spikeNum; j++) {
                Vector2 spikePos = new Vector2(radiusX * Mathf.Cos(Mathf.Deg2Rad * angle), radiusY * Mathf.Sin(Mathf.Deg2Rad * angle));
                SummonSpike(spikePos + (Vector2)transform.position + Vector2.down * 3, _warningTime2_2, _damage2_S, _stunTime2_2, false, cancellationToken).Forget();
                angle += (float)360 / (float)spikeNum;
            }
            for (int j = 0; j < 4; j++)
                PlaySoundDelay(_clipList[2], 0.8f).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_pattern2Delay_2), cancellationToken: cancellationToken);
        }
    }

    private async UniTaskVoid SummonSpike(Vector2 pos, float warningTime, int damage, float stunTime, bool soundIncluded, CancellationToken cancellationToken) {
        GameObject spike = ObjectPool.Instance.GetObject(_spikeObj);
        Animator spikeAnimator = spike.GetComponent<Animator>();
        Collider2D spikeCollider = spike.GetComponent<Collider2D>();
        SpriteRenderer spikeSr = spike.GetComponent<SpriteRenderer>();
        spikeSr.color = Color.white;
        spike.transform.position = pos;
        GameObject warningEffect = spike.transform.GetChild(0).gameObject;
        SpriteRenderer warningEffectSr = warningEffect.GetComponent<SpriteRenderer>();
        warningEffectSr.color = Color.red;
        try {
            spike.SetActive(true);
            warningEffect.SetActive(true);
            Tween warningTween = warningEffectSr.DOFade(0, 0.5f).SetLoops(-1, LoopType.Restart);
            await UniTask.Delay(TimeSpan.FromSeconds(warningTime), cancellationToken: cancellationToken);

            warningTween.Kill();
            warningEffect.SetActive(false);

            spikeAnimator.SetBool("Attack", true);
            if (soundIncluded)
                PlaySound(_clipList[2]);
            if (spikeCollider.IsTouching(_playerCollider)) {
                _playerScript.Stun(stunTime);
                _playerScript.Damaged(damage);
            }
            var targets = Physics2D.OverlapCircleAll(spike.transform.position, 2, _enemyLayer);
            int length = targets.Length;
            if (length > 0) {
                for (int i = 0; i < length; i++) {
                    if (targets[i].GetComponent<MonsterStoneGolem>())
                        targets[i].GetComponent<MonsterBase>().Damaged(damage, true);
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1.3f), cancellationToken: cancellationToken);
            await spikeSr.DOFade(0, 0.5f).ToUniTask(cancellationToken: cancellationToken);
            spikeAnimator.SetBool("Attack", false);
            ObjectPool.Instance.ReturnObject(spike);
        }
        catch (OperationCanceledException) {
            warningEffect.SetActive(false);
            ObjectPool.Instance.ReturnObject(spike);
        }
    }

    #endregion //Spike Pattern

    #region Laser Pattern

    private async UniTaskVoid LaserPatternRandom(CancellationToken cancellationToken) {
        if (_isLaserReady) {
            _isLaserReady = false;
            int randomN = Random.Range(0, 2);
            if (randomN == 0)
                await LaserPattern1(cancellationToken);
            else
                await LaserPattern_S(cancellationToken);

            await UniTask.Delay(TimeSpan.FromSeconds(_laserPatternCooltime), cancellationToken: cancellationToken);
            _isLaserReady = true;
        }
    }

    private async UniTaskVoid InitialLaserTimer(CancellationToken cancellationToken) {
        await UniTask.Delay(TimeSpan.FromSeconds(_laserPatternCooltime), cancellationToken: cancellationToken);
        _isLaserReady = true;
    }

    private async UniTask LaserPattern1(CancellationToken cancellationToken) {
        UniTask[] tasks = new UniTask[8];
        for (int i = 0; i < 8; i++) {
            tasks[i] = LaserShotTargeting(_laserDataList1[i] + (Vector2)transform.position, _chargeDelay1, _laserLastTime1, cancellationToken);
            await UniTask.Delay(TimeSpan.FromSeconds(_laserDelay1), cancellationToken: cancellationToken);
        }
        await UniTask.WhenAll(tasks);
    }

    private async UniTask LaserPattern_S(CancellationToken cancellationToken) {
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 8; j++) {
                LaserShot(_laserDataList2_1[j]._laserDir, _laserDataList2_1[j]._laserPos + (Vector2)transform.position, _chargeDelay2, _laserLastTime2, _damage3_S, cancellationToken).Forget();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_laserDelay2), cancellationToken: cancellationToken);
            for (int j = 0; j < 8; j++) {
                LaserShot(_laserDataList2_2[j]._laserDir, _laserDataList2_2[j]._laserPos + (Vector2)transform.position, _chargeDelay2, _laserLastTime2, _damage3_S, cancellationToken).Forget();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_laserDelay2), cancellationToken: cancellationToken);
        }
    }

    private async UniTask LaserShot(Vector2 dir, Vector2 pos, float chargeDelay, float lastTime, int damage, CancellationToken cancellationToken) {
        GameObject laserObj = ObjectPool.Instance.GetObject(_laserObj);
        GameObject laserWarningEffect = laserObj.transform.GetChild(0).gameObject;
        GameObject laserEffect = laserObj.transform.GetChild(1).gameObject;
        SpriteRenderer warningSr = laserWarningEffect.GetComponent<SpriteRenderer>();
        Animator laserEffectAnimator = laserEffect.GetComponent<Animator>();
        Tween laserWarningTween = DOVirtual.DelayedCall(0.1f, () => { });
        laserObj.transform.position = transform.position;
        float angle = Vector2.SignedAngle(-laserObj.transform.up, dir);
        try {
            laserObj.SetActive(true);
            laserObj.transform.DORotate(new Vector3(0, 0, angle), 2).ToUniTask(cancellationToken: cancellationToken).Forget();
            await laserObj.transform.DOMove(pos, 2).ToUniTask(cancellationToken: cancellationToken);
            laserWarningEffect.SetActive(true);
            warningSr.color = Color.red;
            laserWarningTween = warningSr.DOFade(0, 1).SetLoops(-1, LoopType.Restart);
            await UniTask.Delay(TimeSpan.FromSeconds(chargeDelay), cancellationToken: cancellationToken);
            laserWarningTween.Kill();
            laserWarningEffect.SetActive(false);
            laserEffect.SetActive(true);
            PlaySound(_clipList[3]);
            float timer = 0;
            while (timer < lastTime) {
                var hit = Physics2D.BoxCast(laserObj.transform.position, new Vector2(_laserWidth, _laserWidth), 0, dir, 100f, _playerLayer);
                if (hit.collider != null) {
                    _playerScript.Damaged(damage);
                }
                timer += Time.deltaTime;
                await UniTask.NextFrame(cancellationToken);
            }
            laserEffectAnimator.SetTrigger("End");
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
            laserEffect.SetActive(false);
            laserObj.transform.rotation = Quaternion.identity;
            ObjectPool.Instance.ReturnObject(laserObj);
        }
        catch (OperationCanceledException) {
            laserWarningTween.Kill();
            laserWarningEffect.SetActive(false);
            laserEffect.SetActive(false);
            laserObj.transform.rotation = Quaternion.identity;
            ObjectPool.Instance.ReturnObject(laserObj);
        }
    }


    private async UniTask LaserShotTargeting(Vector2 pos, float chargeDelay, float lastTime, CancellationToken cancellationToken) {
        GameObject laserObj = ObjectPool.Instance.GetObject(_laserObj);
        GameObject laserWarningEffect = laserObj.transform.GetChild(0).gameObject;
        GameObject laserEffect = laserObj.transform.GetChild(1).gameObject;
        SpriteRenderer warningSr = laserWarningEffect.GetComponent<SpriteRenderer>();
        Animator laserEffectAnimator = laserEffect.GetComponent<Animator>();
        Tween laserWarningTween = DOVirtual.DelayedCall(0.1f, () => { });
        laserObj.transform.position = transform.position;
        try {
            laserObj.SetActive(true);
            await laserObj.transform.DOMove(pos, 1.5f).ToUniTask(cancellationToken: cancellationToken);//반드시 목표 지점으로 이동 후 각도 조정할 것
            Vector2 dir = (Vector2)_playerScript.transform.position - pos;
            float angle = Vector2.SignedAngle(-laserObj.transform.up, dir);
            await laserObj.transform.DORotate(new Vector3(0, 0, angle), 0.5f).ToUniTask(cancellationToken: cancellationToken);
            laserWarningEffect.SetActive(true);
            warningSr.color = Color.red;
            laserWarningTween = warningSr.DOFade(0, 1).SetLoops(-1, LoopType.Restart);
            await UniTask.Delay(TimeSpan.FromSeconds(chargeDelay));
            laserWarningTween.Kill();
            laserWarningEffect.SetActive(false);
            laserEffect.SetActive(true);
            PlaySound(_clipList[3]);
            float timer = 0;
            while (timer < lastTime) {
                var hit = Physics2D.BoxCast(laserObj.transform.position, new Vector2(_laserWidth, _laserWidth), 0, dir, 100f, _playerLayer);
                if (hit.collider != null) {
                    _playerScript.Damaged(_damage3);
                }
                timer += Time.deltaTime;
                await UniTask.NextFrame(cancellationToken);
            }
            laserEffectAnimator.SetTrigger("End");
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
            laserEffect.SetActive(false);
            laserObj.transform.rotation = Quaternion.identity;
            ObjectPool.Instance.ReturnObject(laserObj);
        }
        catch (OperationCanceledException) {
            laserWarningTween.Kill();
            laserWarningEffect.SetActive(false);
            laserEffect.SetActive(false);
            laserObj.transform.rotation = Quaternion.identity;
            ObjectPool.Instance.ReturnObject(laserObj);
        }
    }

    #endregion //Laser Pattern

    #region Slow Pattern

    private async UniTask SlowPattern() {
        _animator.SetTrigger("Open");
        _ballAnimator.SetFloat("Speed", 2f);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < 3; j++)
                FireSlowBullet(-10 + 10 * j);

            PlaySound(_clipList[1]);
            await UniTask.Delay(TimeSpan.FromSeconds(_pattern4Delay));
        }
        _ballAnimator.SetFloat("Speed", 0.3f);
        _animator.SetTrigger("Close");
    }

    private async UniTask SlowPattern_S(CancellationToken cancellationToken) {
        _animator.SetTrigger("Open");
        _ballAnimator.SetFloat("Speed", 3f);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 3; j++)
                FireSlowBullet_S(-10 + 10 * j);

            PlaySound(_clipList[1]);
            await UniTask.Delay(TimeSpan.FromSeconds(_pattern4Delay_S), cancellationToken: cancellationToken);
        }
        _ballAnimator.SetFloat("Speed", 1f);
        _animator.SetTrigger("Close");
    }

    private void FireSlowBullet(float offsetAngle) {
        GameObject projectile = ObjectPool.Instance.GetObject(_projectileObj4);
        Bullet projectileScript = projectile.GetComponent<Bullet>();
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(_projectileSpeed4);
        projectileScript.SetDmg(_projectileDamage4);
        projectileScript.SetSlow(_slowRate4, _slowTime4);
        float lastTime = _projectileRange4 / _projectileSpeed4;
        projectileScript.SetLastTime(lastTime);
        Vector2 fireDir = _playerScript.transform.position - transform.position;
        float angle = Vector2.SignedAngle(transform.right, fireDir) + offsetAngle;
        fireDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        projectile.transform.right = fireDir;
        projectile.transform.position = transform.position + projectile.transform.right;
        projectile.SetActive(true);
    }

    private void FireSlowBullet_S(float offsetAngle) {
        GameObject projectile = ObjectPool.Instance.GetObject(_projectileObj4_S);
        Bullet projectileScript = projectile.GetComponent<Bullet>();
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(_projectileSpeed4_S);
        projectileScript.SetDmg(_projectileDamage4_S);
        projectileScript.SetSlow(_slowRate4_S, _slowTime4);
        float lastTime = _projectileRange4 / _projectileSpeed4_S;
        projectileScript.SetLastTime(lastTime);
        Vector2 fireDir = _playerScript.transform.position - transform.position;
        float angle = Vector2.SignedAngle(transform.right, fireDir) + offsetAngle;
        fireDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        projectile.transform.right = fireDir;
        projectile.transform.position = transform.position + projectile.transform.right;
        projectile.SetActive(true);
    }

    #endregion //Slow Pattern

    #region Pillar Pattern
    private async UniTaskVoid PillarPattern(CancellationToken cancellationToken) {
        if (_isPillarReady) {
            _isPillarReady = false;
            SetDef(_defPlus5);
            int i;
            _pillarCnt = 0;
            GameObject[] pillars = new GameObject[4];
            for (i = 0; i < 4; i++) {
                pillars[i] = ObjectPool.Instance.GetObject(_pillarObj5);
                pillars[i].transform.position = _pillarPos5[i] + (Vector2)transform.position;
                pillars[i].SetActive(true);
            }

            float timer = _waitTime5;
            Tween warningTween = DOVirtual.DelayedCall(0.1f, () => { });
            try {
                _timeUI5.gameObject.SetActive(true);
                while (timer > 0) {
                    timer -= Time.deltaTime;
                    int sec = Mathf.FloorToInt(timer);
                    _timeUI5.text = $"00 : {sec:D2}";
                    if (_pillarCnt == 4) {
                        _pillarCnt = -1;
                        _timeUI5.gameObject.SetActive(false);
                        for (i = 0; i < 4; i++) {
                            pillars[i].GetComponent<Pillar>().FadeEnd();
                        }
                        SetDef(_originDef);
                        return;
                    }
                    await UniTask.NextFrame(cancellationToken);
                }

                for (i = 0; i < 4; i++) {
                    pillars[i].GetComponent<Pillar>().FadeEnd();
                }

                _timeUI5.text = $"00 : 00";
                _warningEffectObj5.SetActive(true);
                _volumeObj.SetActive(true);
                warningTween = _warningEffectSpriteRenderer5.DOFade(0, 1).SetLoops(-1, LoopType.Restart);
                await UniTask.Delay(TimeSpan.FromSeconds(_warningTime5), cancellationToken: cancellationToken);
                warningTween.Kill();
                _patternEffectObj5.SetActive(true);
                for (i = 0; i < 3; i++)
                    PlaySound(_clipList[4]);
                await _patternEffectObj5.transform.DOScale(_range5, 0.3f).ToUniTask(cancellationToken: cancellationToken);
                var player = Physics2D.OverlapCircle(transform.position, _range5, _playerLayer);
                _volumeObj.SetActive(false);
                _patternEffectObj5.transform.localScale = Vector3.one;
                _patternEffectObj5.SetActive(false);

                if (player != null)
                    _playerScript.DamagedAbs(_damage5);

                _warningEffectObj5.SetActive(false);
                _pillarCnt = -1;
                _timeUI5.gameObject.SetActive(false);
                SetDef(_originDef);

                await UniTask.Delay(TimeSpan.FromSeconds(_pillarPatternCooltime), cancellationToken: cancellationToken);
                _isPillarReady = true;
            }
            catch (OperationCanceledException) {
                warningTween.Kill();
                _warningEffectObj5.SetActive(false);
                _volumeObj.SetActive(false);
                _patternEffectObj5.transform.localScale = Vector3.one;
                _patternEffectObj5.SetActive(false);
                _timeUI5.gameObject.SetActive(false);
                for (i = 0; i < 4; i++) {
                    pillars[i].GetComponent<Pillar>().FadeEnd();
                }
            }
        }
    }

    private async UniTaskVoid InitialPillarTimer(CancellationToken cancellationToken) {
        await UniTask.Delay(TimeSpan.FromSeconds(_pillarPatternCooltime), cancellationToken: cancellationToken);
        _isPillarReady = true;
    }

    private void SetDef(int def) { _def = def; }

    #endregion //Pillar Pattern

    #region Golem Pattern

    private async UniTaskVoid SummonGolems(CancellationToken cancellationToken) {
        List<GameObject> aliveGolems = new List<GameObject>();
        if (_isGolemReady) {
            try {
                _isGolemReady = false;

                List<int> posList = PickRandomPosIndex();

                for (int i = 0; i < posList.Count; i++) {
                    aliveGolems.Add(SummonGolem(posList[i]));
                    PlaySound(_clipList[5]);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_golemPatternCooltime), cancellationToken: cancellationToken);
                _isGolemReady = true;
            }
            catch (OperationCanceledException) {
                int length = aliveGolems.Count;
                for (int i = 0; i < length; i++) {
                    if (aliveGolems[i] != null)
                        Destroy(aliveGolems[i]);
                }
            }
        }
    }

    private List<int> PickRandomPosIndex() {
        List<int> result = new List<int>();
        int randomN;
        while (result.Count < 3 && GetFreePosNumber() > result.Count) {
            randomN = Random.Range(0, _golemPosList.Count);
            if (!result.Contains(randomN) && !_isPosUsing[randomN])
                result.Add(randomN);
        }

        return result;
    }

    private GameObject SummonGolem(int index) {
        GameObject golem = Instantiate(_golemObj);
        golem.transform.position = _golemPosList[index] + (Vector2)transform.position;
        golem.GetComponent<MonsterStoneGolem>().SetPosIndex(index);
        _isPosUsing[index] = true;
        golem.SetActive(true);
        return golem;
    }

    private int GetFreePosNumber() {
        int cnt = 0;
        for (int i = 0; i < 8; i++) {
            if (!_isPosUsing[i])
                cnt++;
        }
        return cnt;
    }

    #endregion //Golem Pattern



    private void HadesAwake() { _isSleep = false; }

    private void UpdateHpSlider() {
        float hp = _hp;
        float maxHp = _maxHp / 2;
        if (!CheckHalfHp())
            hp = _hp - maxHp;

        _hpSlider.value = hp / maxHp;
    }

    private bool CheckHalfHp() {
        if ((float)_hp / (float)_maxHp < 0.5)
            return true;
        else
            return false;
    }

    // Fade In/Out 코루틴
    private IEnumerator FadeInOutBossUI() {
        // bossImage1과 bossImage2는 동시에 Fade In
        StartCoroutine(FadeUI(bossImage1, true));
        StartCoroutine(FadeUI(bossImage2, true));

        // 일정 딜레이 후에 텍스트가 Fade In
        yield return new WaitForSeconds(delayBetweenUI);
        yield return StartCoroutine(FadeUI(bossText, true));

        // Fade Out (일정 시간 후)
        yield return new WaitForSeconds(2f); // 보스 UI가 켜진 후 유지될 시간

        // 텍스트 먼저 Fade Out
        yield return StartCoroutine(FadeUI(bossText, false));

        // 텍스트 Fade Out 후, bossImage1과 bossImage2가 동시에 Fade Out
        StartCoroutine(FadeUI(bossImage1, false));
        StartCoroutine(FadeUI(bossImage2, false));

        // 모든 Fade Out이 끝나면 UI 비활성화
        bossUI.SetActive(false);
        inGameUI.SetActive(true); // BossUI가 꺼질 때 InGameUI 다시 활성화
        _hpSlider.gameObject.SetActive(true);
    }

    // Fade 효과를 적용하는 코루틴
    private IEnumerator FadeUI(Graphic uiElement, bool fadeIn) {
        Color originalColor = uiElement.color;
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            yield return null;
        }

        uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }

    #endregion

}
