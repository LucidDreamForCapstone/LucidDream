using Cysharp.Threading.Tasks;
using DG.Tweening;
using Edgar.Unity.Examples.Gungeon;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterBase : DropableBase {
    public enum AttackState {
        Ready,
        Attacking,
        Finished,
        CoolTime
    }

    public BehaviourTree _tree;
    public bool _isRightDefault;
    public int _patternNum;
    [HideInInspector] public List<Func<UniTaskVoid>> _attackFuncList = new List<Func<UniTaskVoid>>();
    [HideInInspector] public List<AttackState> _attackStateList = new List<AttackState>();
    [HideInInspector] public bool _isDead;
    [HideInInspector] public bool _isStun;
    [HideInInspector] public bool _isSpawnComplete;
    [HideInInspector] public Player _playerScript;
    [HideInInspector] public Player_2 _player2;
    [HideInInspector] public Rigidbody2D _rigid;
    public SpriteRenderer _spriteRenderer; //manual put
    public Animator _animator;//manual put

    #region serialized field
    [SerializeField] protected bool _useTree;
    [SerializeField] bool _isBoss;

    [Header("Monster Sound")]
    [SerializeField] private AudioClip[] _hitSound;
    [SerializeField] protected AudioClip _deathSound;
    #endregion // serialized field


    #region protected variable
    [Header("Dist must set in Tree")]
    [SerializeField] protected float _searchDist;
    [SerializeField] protected float _attackDist;
    [Header("")]
    [SerializeField] protected int _def;
    [SerializeField] public int _damage;
    [SerializeField] protected int _bodyDamage;
    [SerializeField] public float _moveSpeed;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected float _dieDelay;
    [SerializeField] protected Color32 _damagedColor;
    [SerializeField] protected int _exp;
    [SerializeField] protected int _feverAmount;
    protected int _hp;
    protected static float _colorChanageLastTime = 0.3f;
    protected bool _isColorChanged;
    #endregion //protected variable


    #region private variable

    Stack<bool> _stunCache = new Stack<bool>();
    Stack<float> _slowCache = new Stack<float>();
    Stack<Material> _coldCache = new Stack<Material>();
    Stack<int> _attCache = new Stack<int>();
    Stack<int> _defCache = new Stack<int>();

    #endregion

    #region mono funcs

    protected void OnEnable() {
        _hp = _maxHp;
        _isDead = false;
        _isStun = false;
        _isColorChanged = false;
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _player2 = GameObject.Find("Player_2").GetComponent<Player_2>();
        _rigid = GetComponent<Rigidbody2D>();
        _isSpawnComplete = false;
        if (_useTree) {
            for (int i = 0; i < _patternNum; i++) {
                _attackStateList.Add(AttackState.Ready);
            }
            _tree._monster = this;
            _tree = _tree.Clone();
        }
        if (!_isBoss)
            Spawn().Forget();
    }

    protected void Update() {
        if (_useTree) {
            _tree.Update();
        }
    }

    virtual protected void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player") && !_isDead && _isSpawnComplete)
            _playerScript.Damaged(_bodyDamage);
    }
    #endregion // mono funcs





    #region public funcs

    virtual public void Damaged(int dmg, bool isCrit, bool isPoison = false) {
        if (!_isDead && _isSpawnComplete) {
            FloatingDamageManager.Instance.ShowDamage(this.gameObject, dmg, false, isCrit, isPoison);

            if (_hp <= dmg) {
                Die().Forget();
            }
            else {
                ChangeColor().Forget();
                if (dmg > 0)
                    _animator.SetTrigger("Damaged");
                _hp -= dmg;
                PlayRandomSound();
            }
        }
    }

    public int GetDef() { return _def; }

    public void BodyDamage(Collider2D collision) {
        if (collision.CompareTag("Player") && !_isDead && _isSpawnComplete)
            _playerScript.Damaged(_bodyDamage);
    }

    virtual public async UniTaskVoid Stun(float lastTime, float offsetY = 1, float scale = 1) {
        _isStun = true;
        if (_stunCache.Count > 0)
            _stunCache.Push(true);
        else
            _stunCache.Push(false);
        StateEffectManager.Instance.SummonEffect(transform, StateType.Stuned, offsetY, lastTime, scale).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        _isStun = _stunCache.Pop();
    }

    virtual public async UniTaskVoid Slow(float minusRate, float lastTime, float scale = 1) {
        float minusSpeed = _moveSpeed * minusRate * 0.01f;
        _slowCache.Push(_moveSpeed);
        _moveSpeed -= minusSpeed;
        StateEffectManager.Instance.SummonEffect(transform, StateType.SlowDown, 0, lastTime, scale).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        _moveSpeed = _slowCache.Pop();
    }
    virtual public async UniTaskVoid Cold(float minusRate, float lastTime, float scale = 1) {
        float minusSpeed = _moveSpeed * minusRate * 0.01f;
        _slowCache.Push(_moveSpeed);
        if (_spriteRenderer != null) {
            _coldCache.Push(_spriteRenderer.material);
            _spriteRenderer.material = StateEffectManager.Instance.GetColdMat();
        }
        _moveSpeed -= minusSpeed;
        StateEffectManager.Instance.SummonEffect(transform, StateType.ColdSnow, 0, lastTime, scale).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        if (_spriteRenderer != null) {
            _spriteRenderer.material = _coldCache.Pop();
        }
        _moveSpeed = _slowCache.Pop();
    }


    virtual public async UniTaskVoid AttFear(float minusRate, float lastTime, float offsetY = 1, float scale = 1) {
        float minusAtt = _damage * minusRate * 0.01f;
        _attCache.Push(_damage);
        _damage -= (int)minusAtt;
        StateEffectManager.Instance.SummonEffect(transform, StateType.Fear2, offsetY, lastTime, scale).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        _damage = _attCache.Pop();
    }
    virtual public async UniTaskVoid DefFear(float minusRate, float lastTime, float offsetY = 1, float scale = 1) {
        float minusAtt = _def * minusRate * 0.01f;
        _defCache.Push(_def);
        _def -= (int)minusAtt;
        StateEffectManager.Instance.SummonEffect(transform, StateType.Fear, offsetY, lastTime, scale).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        _def = _defCache.Pop();
    }

    virtual public async UniTaskVoid Poison(int tickDamage, int tickCount, float tickTime, float scale = 1) {
        StateEffectManager.Instance.SummonEffect(transform, StateType.Poison1, 0, tickTime * tickCount, scale).Forget();
        for (int i = 0; i < tickCount; i++) {
            Damaged(tickDamage, false, true);
            await UniTask.Delay(TimeSpan.FromSeconds(tickTime));
        }
    }

    virtual public async UniTaskVoid BloodSuck(float healPercent, float offsetY = 1, float scale = 1) {
        StateEffectManager.Instance.SummonEffect(transform, StateType.BloodRage, offsetY, 1.5f, scale).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        StateEffectManager.Instance.SummonEffect(_playerScript.transform, StateType.SuckBlood, 0, 0.7f, scale).Forget();
        PlayerDataManager.Instance.HealPercent((int)healPercent);
    }

    public void ApplyEssenceFunc(EssenceData data) {
        bool success = UnityEngine.Random.Range(0.0f, 100.0f) < data.chance;
        if (success) {
            switch (data.type) {
                case WeaponEssence.None:
                    break;
                case WeaponEssence.Stun:
                    Stun(data.lastTime).Forget();
                    break;
                case WeaponEssence.Ice:
                    Cold(data.ratio, data.lastTime).Forget();
                    break;
                case WeaponEssence.AttFear:
                    AttFear(data.ratio, data.lastTime).Forget();
                    break;
                case WeaponEssence.DefFear:
                    DefFear(data.ratio, data.lastTime).Forget();
                    break;
                case WeaponEssence.BloodSuck:
                    BloodSuck(data.ratio).Forget();
                    break;
                case WeaponEssence.Poison:
                    Poison(data.tickDamage, data.tickCount, data.tickTime).Forget();
                    break;
            }
        }
    }

    public bool CheckBoss() { return _isBoss; }
    public bool CheckDead() { return _isDead; }

    public float GetHpPercent() {
        return (float)_hp / (float)_maxHp;
    }

    //public abstract void Move();

    #endregion //public funcs





    #region protected funcs

    virtual protected async UniTaskVoid ChangeColor() {
        if (!_isColorChanged) {
            _isColorChanged = true;
            Color32 originColor = _spriteRenderer.color;
            _spriteRenderer.color = _damagedColor;
            await UniTask.Delay(TimeSpan.FromSeconds(_colorChanageLastTime));
            _spriteRenderer.color = originColor;
            _isColorChanged = false;
        }
    }
    virtual protected async UniTaskVoid Die() {
        _isDead = true;
        _hp = 0;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        _animator.SetTrigger("Die");
        PlaySound(_deathSound);
        DropItems();

        // MonsterUIDeathTrigger를 통해 Death UI를 표시
        var deathTrigger = GetComponent<MonsterUIDeathTrigger>();
        if (deathTrigger != null) {
            await deathTrigger.TriggerDeathUI(); // UI 완료 후 다음 동작으로 이동
        }

        // 경험치 획득 로직 호출
        _playerScript.GetExp(_exp);

        PlayerDataManager.Instance.SetFeverGauge(PlayerDataManager.Instance.Status._feverGauge + _feverAmount);
        GetComponent<GungeonEnemy>().RoomManager.OnEnemyKilled(gameObject);
        await UniTask.Delay(TimeSpan.FromSeconds(_dieDelay));
        gameObject.SetActive(false);
    }

    protected void PlaySound(AudioClip clip) {
        if (clip != null) {
            SoundManager.Instance.PlaySFX(clip.name);
        }
    }

    protected async UniTaskVoid PlaySoundDelay(AudioClip clip, float delay) {
        if (clip != null) {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            SoundManager.Instance.PlaySFX(clip.name);
        }
    }

    protected void PlayRandomSound() {
        if (_hitSound != null && _hitSound.Length > 0) {
            int randomIndex = UnityEngine.Random.Range(0, _hitSound.Length);
            PlaySound(_hitSound[randomIndex]);
        }
    }

    virtual protected async UniTaskVoid Spawn() {
        Color originColor = _spriteRenderer.color;
        _spriteRenderer.color = new Color(originColor.r, originColor.g, originColor.b, 0);
        await _spriteRenderer.DOFade(1, 1);
        _isSpawnComplete = true;
    }

    protected float GetDistSquare(Vector2 a, Vector2 b) {
        return (a - b).sqrMagnitude;
    }

    #endregion //protected funcs
}