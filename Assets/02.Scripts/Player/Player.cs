using Cysharp.Threading.Tasks;
using DG.Tweening;
using Edgar.Unity.Examples.Gungeon;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {
    #region public variable

    #endregion // public variable




    #region serialized field

    [SerializeField] private float _invincibleLastTime;
    [SerializeField] private float _phantomLastTime; //ingameTime
    [SerializeField] private float _phantomTimeScale; // 0 ~ 1
    [SerializeField] private float _phantomLerpTime;
    [SerializeField] private float _phantomCoolTime;
    [SerializeField] private float _phantomSpeedBonus;
    [SerializeField] private GameObject _phantomGhostObj;
    [SerializeField] private Animator _phantomVolumeAnimator;
    [SerializeField] private Color _phantomGhostColor;
    [SerializeField] private float _ghostLastTime;
    [SerializeField] private float _ghostInterval;
    //[SerializeField] private float _rollLastTime;
    //[SerializeField] private float _rollCoolTime;
    //[SerializeField] private float _rollDist;
    [SerializeField] private float _itemSearchRadius;
    [SerializeField] private float _itemMagneticSpeed;
    [SerializeField] private LayerMask _itemLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private Color32 _originColor;
    [SerializeField] private Color32 _invincibleColor;
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _armAnimator;
    [SerializeField] private SpriteRenderer _leftArmRenderer;
    [SerializeField] private SpriteRenderer _rightArmRenderer;
    [SerializeField] List<SpriteRenderer> _chargeGauge;
    [SerializeField] GameObject _chargeGuageParent;
    [SerializeField] private Color32 _chargeOriginColor;
    [SerializeField] private Color32 _chargeColor;
    [SerializeField] private Color32 _chargeMaxColor;
    [SerializeField] private GameObject guardEffectPrefab;
    [SerializeField] protected AudioClip avoidSound;
    [SerializeField] protected AudioClip GuardSound;
    [SerializeField] protected AudioClip dyingSound1;
    [SerializeField] protected AudioClip dyingSound2;
    [SerializeField] private AudioClip dyingEffectSound;
    [SerializeField] private DeathUIController deathUIController; // DeathUIController 참조
    #endregion // serialized field 





    #region private variable
    private bool _isDead;
    private bool _isInvincible;//피격 후 무적 상태
    private bool _isRollInvincible;//obsolete
    private bool _isCustomInvincible;//특수 함수에 의한 무적 상태
    //private bool _isRollReady;
    private bool _isPhantomReady;
    private bool _isPhantomActivated;
    private bool _phantomForceCancelTrigger;
    private bool _isAttacking;
    private bool _beforeFlipX;
    private bool _isStun;
    private bool _isPause;
    private float _stunTime;
    private float _slowTime;
    private float _slowRate;
    private float _phantomSpeedRate;
    private List<GameObject> _magneticList = new List<GameObject>();
    private int _chargeCount;
    private InGameUIController _controller;
    #endregion // private variable





    #region property

    public Vector2 MoveDir { get; private set; }

    #endregion //property





    #region mono funcs

    private void Start() {
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        _armAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        MoveDir = Vector2.down;
        _isDead = false;
        _isInvincible = false;
        _phantomVolumeAnimator.gameObject.SetActive(false);
        _isPhantomReady = true;
        _isPhantomActivated = false;
        _phantomForceCancelTrigger = false;
        _phantomSpeedRate = 1;
        _isRollInvincible = false;//obsolete
        //_isRollReady = true;
        _isAttacking = false;
        _beforeFlipX = false;
        _isStun = false;
        _isPause = false;
        _stunTime = 0;
        _slowRate = 0;
        _controller = FindObjectOfType<InGameUIController>();
    }

    private void FixedUpdate() {
        Move();
        ArmMoveAnimation();
    }

    private void Update() {
        //Roll().Forget();
        Phantom().Forget();
        SearchNearItems();
        ItemMagnetic();
        StunState();
        SlowState();
        if (_isDead && Input.GetKeyDown(KeyCode.X)) {
            ReturnToDungeon();
        }
    }

    #endregion // mono funcs





    #region public funcs

    public void Damaged(int dmg) {
        if (!_isInvincible && !_isRollInvincible && !_isCustomInvincible && !_isDead) {
            int currentHP = PlayerDataManager.Instance.Status._hp;
            int def = PlayerDataManager.Instance.Status._def;
            dmg -= def;
            if (dmg < 0)
                dmg = 0;

            FloatingDamageManager.Instance.ShowDamage(this.gameObject, dmg, true, false, false);
            if (currentHP <= dmg)
                Die().Forget();
            else {
                PlayerDataManager.Instance.SetHP(currentHP - dmg);
                Invincibility().Forget();
            }
        }
    }

    public void DamagedAbs(int dmg) {
        if (!_isCustomInvincible) {
            int currentHP = PlayerDataManager.Instance.Status._hp;

            FloatingDamageManager.Instance.ShowDamage(this.gameObject, dmg, true, false, false);
            if (currentHP <= dmg)
                Die().Forget();
            else {
                PlayerDataManager.Instance.SetHP(currentHP - dmg);
                Invincibility().Forget();
            }
        }
    }

    public void PhysicalAttack(MonsterBase monster, float multiplier) {
        int monsterDef = monster.GetDef();
        int ad = PlayerDataManager.Instance.GetFinalAd();
        int critChance = PlayerDataManager.Instance.GetFinalCritChance();
        int critDamage = PlayerDataManager.Instance.GetFinalCritDamage();

        float dmg = ad - monsterDef * 0.2f;
        if (dmg < 0)
            dmg = 0;

        bool isCrit = false;
        if (Random.Range(0, 1.00f) < critChance * 0.01f) {
            dmg *= 1 + critDamage * 0.01f;
            isCrit = true;
        }

        monster.Damaged((int)(dmg * multiplier), isCrit);
    }

    // 씬을 04.Dungeon으로 다시 로드하는 함수
    public void ReturnToDungeon() {
        if (GungeonGameManager.Instance != null) {
            GungeonGameManager.Instance.SetIsGenerating(true);
            GungeonGameManager.Instance.Stage = 0;
        }
        GameSceneManager.Instance.LoadStageScene(1);
    }

    public void NormalMagicalAttack(MonsterBase monster, float multiplier) {
        int monsterDef = monster.GetDef();
        int ap = PlayerDataManager.Instance.GetFinalAp();

        float dmg = ap - monsterDef * 0.4f;
        if (dmg < 0)
            dmg = 0;

        monster.Damaged((int)(dmg * multiplier), false);
    }

    public void SpecialMagicalAttack(MonsterBase monster, float multiplier) {
        int monsterDef = monster.GetDef();
        int ap = PlayerDataManager.Instance.GetFinalAp();
        int critChance = PlayerDataManager.Instance.GetFinalCritChance();
        int critDamage = PlayerDataManager.Instance.GetFinalCritDamage();

        float dmg = 0;
        bool isCrit;

        if (Random.Range(0, 1.00f) < critChance * 0.01f) {
            dmg = (ap - monsterDef * 0.8f) * (1 + critDamage * 0.01f);
            isCrit = true;
        }
        else {
            dmg = ap - monsterDef * 0.4f;
            isCrit = false;
        }

        if (dmg < 0)
            dmg = 0;

        monster.Damaged((int)(dmg * multiplier), isCrit);
    }

    public void GetExp(int gainedExp) {
        int currentExp = PlayerDataManager.Instance.Status._exp;
        int level = PlayerDataManager.Instance.Status._playerLevel;
        int firstLevel = level;
        int levelExp = PlayerDataManager.Instance.Status.GetMaxExp();
        if (levelExp == -1) {
            Debug.Log("잘못된 플레이어 레벨임");
            return;
        }

        PlayerDataManager.Instance.SetExp(currentExp + gainedExp);

        if (level == PlayerDataManager.Instance.Status._maxLevel) {
            Debug.Log("최대 레벨이므로 경험치를 획득할 수 없습니다.");
        }
        else {
            while (PlayerDataManager.Instance.Status._exp >= levelExp) {
                Debug.Log(level + "->" + (level + 1) + "로 레벨업");
                PlayerDataManager.Instance.SetLevel(++level);
                PlayerDataManager.Instance.SetExp(PlayerDataManager.Instance.Status._exp - levelExp);
                PlayerDataManager.Instance.SetMaxFeverGauge();
                levelExp = PlayerDataManager.Instance.Status.GetMaxExp();
            }

            int levelUpCount = 0;

            for (int i = 0; i < level - firstLevel; i++) {
                levelUpCount++;
            }
            PlayerDataManager.Instance.ShowCardSelectUI(levelUpCount);
        }
    }

    public async UniTaskVoid AttackNow(float attackDelay) {
        _isAttacking = true;
        await UniTask.Delay(TimeSpan.FromSeconds(attackDelay), ignoreTimeScale: true);
        _isAttacking = false;
    }

    public bool CheckInvincible() {
        return _isInvincible || _isRollInvincible || _isCustomInvincible;
    }

    public bool CheckStun() {
        return _isStun;
    }

    public bool CheckPause() {
        return _isPause;
    }

    public bool CheckAttacking() {
        return _isAttacking;
    }

    public bool CheckDead() {
        return _isDead;
    }

    public void ArmTrigger(string trigger) {
        _armAnimator.SetTrigger(trigger);
    }

    public void Stun(float lastTime) {
        if (_stunTime < lastTime) {
            StateEffectManager.Instance.SummonEffect(transform, StateType.Stuned, 1, lastTime).Forget();
            _stunTime = lastTime;
        }
    }

    public void Slow(float slowRate, float lastTime) {
        if (slowRate >= _slowRate) {
            StateEffectManager.Instance.SummonEffect(transform, StateType.SlowDown, 0, lastTime).Forget();
            _slowRate = slowRate;
            _slowTime = lastTime;
        }
    }

    public void PlayerPause() { _isPause = true; }
    public void PlayerUnPause() { _isPause = false; }

    public void ChargeGaugeUp() {
        if (_chargeCount < 5) {
            _chargeGauge[_chargeCount++].DOColor(_chargeColor, 0.1f).SetUpdate(true);
            if (_chargeCount == 5) {
                for (int i = 0; i < 5; i++)
                    _chargeGauge[i].DOColor(_chargeMaxColor, 0.1f).SetUpdate(true);
            }

        }
    }

    public void ResetChargeGauge() {
        for (int i = 0; i < 5; i++) {
            _chargeGauge[i].DOColor(_chargeOriginColor, 1).SetUpdate(true);
            _chargeCount = 0;
        }
    }

    public int GetChargetCount() { return _chargeCount; }

    public void ActivateChargeGauge() {
        _chargeGuageParent.SetActive(true);
    }

    public void DeActivateChargeGauge() {
        _chargeGuageParent.SetActive(false);
    }

    public async UniTaskVoid CustomInvincible(float lastTime) {
        _isCustomInvincible = true;
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        _isCustomInvincible = false;
    }

    public void PhantomForceCancel() {
        if (_isPhantomActivated)
            _phantomForceCancelTrigger = true;
    }

    #endregion //public funcs





    #region private funcs

    private void Move() {
        if (!_isRollInvincible && !_isAttacking && !_isStun && !_isPause && !_isDead && TimeScaleManager.IsTimeFlow) {
            Vector2 moveVec = Vector2.zero;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            PlayerMoveAnimation(horizontal, vertical);

            moveVec = new Vector2(horizontal, vertical);

            if (horizontal != 0 || vertical != 0) { //Prevent Update of MoveDir when player stopped
                MoveDir = moveVec.normalized;
            }

            _rigid.velocity = moveVec.normalized * PlayerDataManager.Instance.GetFinalMoveSpeed() * (100 - _slowRate) * _phantomSpeedRate * 0.01f;
            //_rigid.MovePosition((Vector2)transform.position + moveVec.normalized * PlayerDataManager.Instance.GetFinalMoveSpeed() * (100 - _slowRate) * Time.unscaledDeltaTime * 0.01f);
        }
        else {
            _rigid.velocity = Vector2.zero;
            PlayerMoveAnimation(0, 0);
        }
    }

    private async UniTaskVoid Phantom() {
        if (Input.GetKey(KeyCode.Space) && _isPhantomReady && !_isStun && !_isPause && !_isDead) {
            Debug.Log("Phantom ON");
            PhantomGhostEffect().Forget();
            //PlaySound(avoidSound);
            _isPhantomReady = false;
            float timer = 0;
            DOTween.To(() => _phantomSpeedRate, x => _phantomSpeedRate = x, _phantomSpeedBonus * 0.01f, _phantomLerpTime).ToUniTask().Forget();
            _phantomVolumeAnimator.gameObject.SetActive(true);
            await TimeScaleManager.Instance.TimeSlowLerp(_phantomTimeScale, _phantomLerpTime);
            while (!Input.GetKey(KeyCode.Space) && timer < _phantomLastTime && !_phantomForceCancelTrigger) {
                timer += Time.deltaTime;
                await UniTask.NextFrame();
            }
            DOTween.To(() => _phantomSpeedRate, x => _phantomSpeedRate = x, 1, _phantomLerpTime).ToUniTask().Forget();
            _phantomVolumeAnimator.SetTrigger("End");
            await TimeScaleManager.Instance.TimeRestoreLerp(_phantomLerpTime);
            _phantomVolumeAnimator.gameObject.SetActive(false);
            _isPhantomActivated = false;
            _phantomForceCancelTrigger = false;
            Debug.Log("Phantom OFF");
            await UniTask.Delay(TimeSpan.FromSeconds(_phantomCoolTime));
            _isPhantomReady = true;
            Debug.Log("**Phantom Ready**");
        }
    }

    private async UniTaskVoid PhantomGhostEffect() {
        _isPhantomActivated = true;
        while (_isPhantomActivated) {
            SinglePhantomGhost().Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_ghostInterval), ignoreTimeScale: true);
        }
    }

    private async UniTaskVoid SinglePhantomGhost() {
        GameObject ghostEffect = ObjectPool.Instance.GetObject(_phantomGhostObj);
        SpriteRenderer sr = ghostEffect.GetComponent<SpriteRenderer>();
        sr.color = _phantomGhostColor;
        sr.sprite = _spriteRenderer.sprite;
        if (MoveDir.x > 0)
            sr.flipX = true;
        else
            sr.flipX = false;

        ghostEffect.transform.position = transform.position;
        ghostEffect.SetActive(true);
        await sr.DOFade(0, _ghostLastTime).SetUpdate(true);
        ObjectPool.Instance.ReturnObject(ghostEffect);
    }


    /*
    private async UniTaskVoid Roll() {
        if (Input.GetKeyDown(KeyCode.Space) && _isRollReady && !_isStun && !_isPause && !_isDead && !CheckWall()) {
            //Debug.Log("구른다");
            PlaySound(avoidSound);
            _isRollInvincible = true;
            _isRollReady = false;
            Vector3 startPos = transform.position;
            Vector3 moveVec = MoveDir * _rollDist;
            await RollMove(_rigid, startPos + moveVec, _rollLastTime);
            //Debug.Log("구르기 완");
            _isRollInvincible = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_rollCoolTime));
            _isRollReady = true;
            //Debug.Log("구르기 쿨 돌았음");
        }
    }
    */

    private UniTask RollMove(Rigidbody2D rigidbody, Vector2 endValue, float duration) {
        var tcs = new UniTaskCompletionSource();

        DOTween.To(() => rigidbody.position, x => rigidbody.MovePosition(x), endValue, duration)
               .OnComplete(() => tcs.TrySetResult());

        return tcs.Task;
    }

    private void PlayerMoveAnimation(float horizontal, float vertical) {
        if (horizontal > 0)
            _beforeFlipX = true;
        else if (horizontal < 0)
            _beforeFlipX = false;

        _spriteRenderer.flipX = _beforeFlipX;
        _animator.SetInteger("horizontal", (int)horizontal);
        _animator.SetInteger("vertical", (int)vertical);
    }

    private void ArmMoveAnimation() {
        _armAnimator.SetFloat("Horizontal", MoveDir.x);
        _armAnimator.SetFloat("Vertical", MoveDir.y);

        if (MoveDir.y > 0 && MoveDir.x == 0) {
            PlayerWeaponManager.Instance.GetEquippedWeapon().SetWeaponSortingOrder(4);
            SetArmSortingLayer(4);
        }
        else {
            PlayerWeaponManager.Instance.GetEquippedWeapon().SetWeaponSortingOrder(6);
            SetArmSortingLayer(7);
        }
    }

    private async UniTaskVoid Die() {
        // Guard 아이템이 인벤토리에 있는지 확인
        if (InventoryManager.Instance.HasItem(ItemType.Guard)) {
            // 체력 30% 회복
            PlaySound(GuardSound);
            PlayerDataManager.Instance.HealByMaxPercent(30);
            // Guard 아이템 발동 이펙트 생성
            if (guardEffectPrefab != null) {
                Vector3 effect0Position = this.transform.position + new Vector3(0, 0.7f, 0);
                // 이펙트 오브젝트 생성
                GameObject effectInstance = Instantiate(guardEffectPrefab, effect0Position, Quaternion.identity);

                // 이펙트를 플레이어의 자식 오브젝트로 설정
                effectInstance.transform.SetParent(this.transform);
            }
            _controller.offGuard();
            // Guard 아이템 인벤토리에서 제거
            InventoryManager.Instance.RemoveItem(ItemType.Guard);
        }
        else {
            Debug.Log("플레이어 사망");
            _isDead = true;
            AudioClip selectedDyingClip = Random.Range(0, 2) == 0 ? dyingSound1 : dyingSound2;
            PlaySoundDelay(selectedDyingClip, 1.4f).Forget();
            PlayerDataManager.Instance.SetHP(0);
            _spriteRenderer.sortingLayerName = "Default";
            _leftArmRenderer.sortingLayerName = "Default";
            _rightArmRenderer.sortingLayerName = "Default";
            PlayerWeaponManager.Instance.GetEquippedWeapon().SetWeaponSortingLayer("Default");
            CameraManager.Instance.CameraFocus(transform, -12, 30).Forget();
            _animator.SetTrigger("Die");
            _armAnimator.SetTrigger("Die");
            await UniTask.Delay(TimeSpan.FromSeconds(2.5f));
            deathUIController.ShowDeathUI().Forget();
        }
    }

    private async UniTaskVoid Invincibility() {
        _isInvincible = true;
        //Debug.Log("무적 상태 돌입");
        _spriteRenderer.DOColor(_invincibleColor, 0.1f).ToUniTask().Forget();
        _leftArmRenderer.color = _invincibleColor;
        _rightArmRenderer.color = _invincibleColor;
        PlayerWeaponManager.Instance.GetEquippedWeapon().SetWeaponColor(_invincibleColor);
        await UniTask.Delay(TimeSpan.FromSeconds(_invincibleLastTime));
        _isInvincible = false;
        //Debug.Log("무적 상태 해제");
        PlayerWeaponManager.Instance.GetEquippedWeapon().SetWeaponColor(Color.white);
        _rightArmRenderer.color = Color.white;
        _leftArmRenderer.color = Color.white;
        _spriteRenderer.DOColor(_originColor, 0.1f).ToUniTask().Forget();
    }

    private void SearchNearItems() {
        var nearItems = Physics2D.OverlapCircleAll(transform.position, _itemSearchRadius, _itemLayer);
        int i, length = nearItems.Length;
        if (length > 0) {
            for (i = 0; i < length; i++) {
                if (!_magneticList.Contains(nearItems[i].gameObject))
                    _magneticList.Add(nearItems[i].gameObject);
            }
        }
    }

    private void ItemMagnetic() {
        int i, length = _magneticList.Count;
        if (length > 0) {
            for (i = 0; i < length; i++) {
                if (!_magneticList[i].activeSelf) {
                    _magneticList.RemoveAt(i--);
                    length = _magneticList.Count;
                }
                else
                    _magneticList[i].transform.position = Vector2.Lerp(_magneticList[i].transform.position, transform.position, _itemMagneticSpeed * Time.deltaTime);

            }
        }
    }

    private void SetArmSortingLayer(int layerN) {
        _leftArmRenderer.sortingOrder = layerN;
        _rightArmRenderer.sortingOrder = layerN;
    }

    /*
    private bool CheckWall() {
        if (Physics2D.Raycast(transform.position, MoveDir, _rollDist - 1, _wallLayer))
            return true;
        return false;
    }
    */

    private void StunState() {
        if (_stunTime > 0) {
            _isStun = true;
            _stunTime -= Time.deltaTime;
        }
        else {
            _isStun = false;
            _stunTime = 0;
        }
    }

    private void SlowState() {
        if (_slowTime > 0)
            _slowTime -= Time.deltaTime;
        else {
            _slowRate = 0;
            _slowTime = 0;
        }
    }
    private void PlaySound(AudioClip clip) {
        if (clip != null) {
            SoundManager.Instance.PlaySFX(clip.name);
        }
    }

    private async UniTaskVoid PlaySoundDelay(AudioClip clip, float delay) {
        if (clip != null) {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            SoundManager.Instance.PlaySFX(clip.name);
        }
    }
    #endregion //private funcs
}