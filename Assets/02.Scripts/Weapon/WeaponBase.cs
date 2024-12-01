using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;

public abstract class WeaponBase : ItemBase {

    #region protected variable

    protected Player _playerScript;
    protected LayerMask _monsterLayer;
    protected Animator _animator;
    private InGameUIController inGameUIController;
    #endregion //protected variable


    #region serialized field
    [SerializeField] string _weaponName;
    [SerializeField] string _weaponDescription;
    [SerializeField] protected WeaponRank _weaponRank;
    [SerializeField] bool _useCharge;
    [SerializeField] Vector2 _weaponPos;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] protected WeaponBasicData _weaponBasicData;

    [SerializeField] protected float _basicCoolTime;
    [SerializeField] protected float _basicDelay;

    [SerializeField] float _skill1CoolTime;
    [SerializeField] protected float _delay1;

    [SerializeField] float _skill2CoolTime;
    [SerializeField] protected float _delay2;

    [SerializeField] protected float _feverDelay;
    [SerializeField] private Sprite[] skillIcons = new Sprite[4];
    [SerializeField] private Sprite[] skillInvens = new Sprite[4];
    [SerializeField] private string[] skillDescriptions = new string[4];
    [SerializeField] private Sprite[] skillInforms = new Sprite[4];
    [SerializeField] private Sprite[] skillunderlay = new Sprite[4];


    [SerializeField] protected GameObject _basicSkillEffect;
    [SerializeField] protected GameObject _skillEffect1;
    [SerializeField] protected GameObject _skillEffect2;
    [SerializeField] protected GameObject _feverSkillEffect0;
    [SerializeField] protected GameObject _feverSkillEffect1;
    // 피버 스킬 사정거리 추가
    [SerializeField] protected float _feverRange;

    // 피버 스킬이 공격형인지 버프형인지 구분하는 플래그
    [SerializeField] protected bool _isBuffTypeFeverSkill;
    [SerializeField] protected AudioClip _normalAttackSound;     // 노말 어택 사운드

    // 화면 흔들림
    [SerializeField] protected float shakeDuration = 0.3f; // 흔들림 지속 시간
    [SerializeField] protected float shakeAmplitude = 3.0f; // 흔들림 진폭
    [SerializeField] protected float shakeFrequency = 25.0f; // 흔들림 주파수

    #endregion // serialized field

    #region private variable

    private bool _isEquipped = false;
    protected bool _isBasicReady = true;
    private bool _isSkill1Ready = true;
    private bool _isSkill2Ready = true;
    private bool _passive;
    private bool _skill1Lock;
    private bool _skill2Lock;
    private bool _feverLock;

    #endregion // private variable


    #region property

    #endregion // property variable


    #region mono funcs

    protected void Start() {
        LockWeaponSkillByRank();
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _monsterLayer = LayerMask.GetMask("Enemy", "CollidableEnemy");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        inGameUIController = FindObjectOfType<InGameUIController>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player"))
            Equip();
    }

    #endregion // mono funcs





    #region public funcs


    virtual public async UniTaskVoid ActivateBasicAttack() {
        if (_isBasicReady) {
            BasicAttackAnimation();
            _isBasicReady = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_basicCoolTime), ignoreTimeScale: true);
            _isBasicReady = true;
        }
    }

    virtual public async UniTaskVoid ActivateSkill1() {
        if (!_skill1Lock && _isSkill1Ready) {
            _isSkill1Ready = false;
            Skill1Animation();
            inGameUIController.UseSkill(1);

            await UniTask.Delay(TimeSpan.FromSeconds(_skill1CoolTime));
            _isSkill1Ready = true;
        }
    }

    virtual public async UniTaskVoid ActivateSkill2() {
        if (!_skill2Lock && _isSkill2Ready) {
            _isSkill2Ready = false;
            Skill2Animation();
            inGameUIController.UseSkill(2);
            await UniTask.Delay(TimeSpan.FromSeconds(_skill2CoolTime));
            _isSkill2Ready = true;
        }
    }

    virtual public void ActivateFeverSkill() {
        // 피버 스킬이 버프형이면 사정거리에 상관없이 사용 가능
        if (_isBuffTypeFeverSkill) {
            ActivateFeverSkillWithoutTarget(); // 버프형 스킬 실행
            return;
        }

        // 피버 스킬이 공격형일 경우에만 사정거리 체크
        if (!_feverLock && PlayerDataManager.Instance.IsFeverReady()) {
            // 사정거리 내 적 탐지
            Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _feverRange, _monsterLayer);
            int targetCount = possibleTargets.Length;

            // 적이 없으면 스킬 실행 중단 및 문구 출력
            if (targetCount == 0) {
                string message = "범위 내에 적이 없습니다."; //Bum wi nae ae juk e up sup ni da.
                SystemMessageManager.Instance.PushSystemMessage(message, Color.red);
                return; // 스킬 실행 중단, Fever 게이지 유지
            }

            // 피버 게이지 0으로 설정 후 스킬 실행
            PlayerDataManager.Instance.SetFeverGauge(0);
            FeverSkillAnimation();
            //inGameUIController.ResetFeverFill();  // Fever 게이지를 0으로 만듦
        }
        else {
            Debug.Log("아직 피버 게이지가 가득 차지 않았거나 피버 스킬이 잠겨서 피버 스킬을 사용할 수 없습니다.");
        }
    }


    public bool GetEquipState() { return _isEquipped; }
    public void SetEquipState(bool equipState) { _isEquipped = equipState; }
    public void SetWeaponSortingOrder(int layerN) { _spriteRenderer.sortingOrder = layerN; }
    public void SetWeaponSortingLayer(string layerName) { _spriteRenderer.sortingLayerName = layerName; }
    public string GetWeaponName() { return _weaponName; }
    public string GetWeaponDescription() { return _weaponDescription; }
    public bool CheckUseCharging() { return _useCharge; }
    public WeaponRank GetWeaponRank() { return _weaponRank; }
    public WeaponBasicData GetWeaponBasicData() { return _weaponBasicData; }

    public void InitializeWeaponPos() {
        transform.localPosition = _weaponPos;
        transform.localRotation = Quaternion.identity;
    }

    public void SetWeaponColor(Color32 color) {
        _spriteRenderer.color = color;
    }

    public override bool IsInteractBlock() {
        if (_isEquipped) return true;
        else return false;
    }

    public override string GetInteractText() {
        return $"{_weaponName}[{_weaponRank}] (G)";
    }
    public Sprite[] GetSkillIcons() {
        return skillIcons;
    }
    public string[] GetSkillDescriptions() {
        return skillDescriptions;
    }

    public bool[] GetSkillLocks() {
        return new bool[] { _passive, _skill1Lock, _skill2Lock, _feverLock };
    }

    public Sprite[] GetSkillInvens() {
        return skillInvens;
    }

    public Sprite[] GetSkillInformPanel() {
        return skillInforms;
    }

    public Sprite[] GetSkillUnderlay() {
        return skillunderlay;
    }

    public float GetBasicCooldown() { return _basicCoolTime; }
    public float GetSkill1Cooldown() { return _skill1CoolTime; }
    public float GetSkill2Cooldown() { return _skill2CoolTime; }




    abstract protected void BasicAttackAnimation();
    abstract protected void Skill1Animation();


    abstract protected void Skill2Animation();
    abstract protected void FeverSkillAnimation();

    abstract public void BasicAttack();
    abstract public void Skill1();
    abstract public void Skill2();
    abstract public void FeverSkill();

    #endregion //public funcs






    #region protected funcs

    protected double CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }




    #endregion //protected funcs




    #region private funcs

    private void LockWeaponSkillByRank() {
        if (_weaponRank == WeaponRank.Normal) {
            _passive = true;
            _feverLock = true;
            _skill1Lock = true;
            _skill2Lock = true;
        }
        else if (_weaponRank == WeaponRank.Rare) {
            _passive = true;
            _feverLock = false;
            _skill1Lock = true;
            _skill2Lock = true;
        }
        else if (_weaponRank == WeaponRank.Unique) {
            _passive = false;
            _feverLock = false;
            _skill1Lock = false;
            _skill2Lock = true;
        }
        else if (_weaponRank == WeaponRank.Legendary) {
            _passive = false;
            _feverLock = false;
            _skill1Lock = false;
            _skill2Lock = false;
        }
        else {
            Debug.Log("Unknown Weapon Enum Error");
        }
    }

    private void Equip() {
        if (!_isEquipped && _isGround && InteractManager.Instance.CheckInteractable(this)) {
            if (Input.GetKey(KeyCode.G)) {
                InteractManager.Instance.InteractCoolTime().Forget();
                PlayerWeaponManager.Instance.WeaponChanged(this);
            }
        }
    }

    private IEnumerator ShakeCamera() {
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null) {
            var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null) {
                noise.m_AmplitudeGain = shakeAmplitude;
                noise.m_FrequencyGain = shakeFrequency;

                yield return new WaitForSeconds(shakeDuration);

                noise.m_AmplitudeGain = 0;
                noise.m_FrequencyGain = 0;
            }
        }
    }


    // 버프형 스킬 처리
    private void ActivateFeverSkillWithoutTarget() {
        if (!_feverLock && PlayerDataManager.Instance.IsFeverReady()) {
            PlayerDataManager.Instance.SetFeverGauge(0);
            FeverSkillAnimation();  // 피버 스킬 실행
            inGameUIController.ResetFeverFill();  // Fever 게이지를 0으로 만듦
        }
    }
    protected void PlaySound(AudioClip clip, bool timeIgnore = true) {
        if (clip != null) {
            SoundManager.Instance.PlaySFX(clip.name, timeIgnore);
        }
    }
    protected async UniTaskVoid PlaySoundDelay(AudioClip clip, float delay, bool timeIgnore = true) {
        if (clip != null) {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: timeIgnore);
            SoundManager.Instance.PlaySFX(clip.name, timeIgnore);
        }
    }

    #endregion //private funcs
}
