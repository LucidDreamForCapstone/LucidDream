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
    [HideInInspector] public Rigidbody2D _rigid;
    public SpriteRenderer _spriteRenderer; //manual put
    public Animator _animator;//manual put

    #region serialized field
    [SerializeField] bool _useTree;
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




    #region mono funcs

    protected void OnEnable() {
        _hp = _maxHp;
        _isDead = false;
        _isStun = false;
        _isColorChanged = false;
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
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

    private void Update() {
        if (_useTree) {
            _tree.Update();
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player") && !_isDead && _isSpawnComplete)
            _playerScript.Damaged(_bodyDamage);
    }
    #endregion // mono funcs





    #region public funcs

    virtual public void Damaged(int dmg, bool isCrit) {
        if (!_isDead && _isSpawnComplete) {
            FloatingDamageManager.Instance.ShowDamage(this.gameObject, dmg, false, isCrit, false);

            if (_hp <= dmg) {
                Die().Forget();
            }
            else {
                ChangeColor().Forget();
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

    public async UniTaskVoid Stun(float lastTime) {
        _isStun = true;
        StateEffectManager.Instance.SummonEffect(transform, StateType.Stuned, 1, lastTime).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        _isStun = false;
    }

    public async UniTaskVoid Slow(float minusRate, float lastTime) {
        float minusSpeed = _moveSpeed * minusRate * 0.01f;
        _moveSpeed -= minusSpeed;
        StateEffectManager.Instance.SummonEffect(transform, StateType.SlowDown, 0, lastTime).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        _moveSpeed += minusSpeed;
    }

    public bool CheckBoss() { return _isBoss; }
    public bool CheckDead() { return _isDead; }

    public float GetHpPercent() {
        return (float)_hp / (float)_maxHp;
    }

    //public abstract void Move();

    #endregion //public funcs





    #region protected funcs

    protected abstract void AttackMove();

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