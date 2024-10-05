using Cysharp.Threading.Tasks;
using DG.Tweening;
using Edgar.Unity.Examples.Gungeon;
using System;
using UnityEngine;

public abstract class MonsterBase : DropableBase {
    public enum AttackState {
        Ready,
        Attacking,
        CoolTime
    }

    public BehaviourTree _tree;
    public bool _isRightDefault;
    [HideInInspector] public AttackState _attackState;
    public float _searchDist;
    public float _attackDist;
    [HideInInspector] public bool _isDead;
    [HideInInspector] public bool _isSpawnComplete;
    [HideInInspector] public Player _playerScript;
    [HideInInspector] public Rigidbody2D _rigid;
    public SpriteRenderer _spriteRenderer; //manual put
    public Animator _animator;//manual put

    #region serialized field
    [SerializeField] bool _useTree;
    [SerializeField] bool _isBoss;
    [SerializeField] int _bodyDamage;

    [Header("Monster Sound")]
    [SerializeField] private AudioClip[] _hitSound;
    [SerializeField] protected AudioClip _deathSound;
    #endregion // serialized field


    #region protected variable

    [SerializeField] protected int _def;
    [SerializeField] public int _damage;
    [SerializeField] public float _moveSpeed;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected float _dieDelay;
    [SerializeField] protected Color32 _damagedColor;
    [SerializeField] protected int _exp;
    [SerializeField] protected int _feverAmount;
    protected int _hp;
    protected bool _isStun;
    protected static float _colorChanageLastTime = 0.3f;
    protected bool _isColorChanged;
    #endregion //protected variable




    #region private variable

    #endregion // private variable




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
            _tree._monster = this;
            _tree = _tree.Clone();
        }
    }

    private void Update() {
        if (_useTree) {
            _tree.Update();
        }
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

    public virtual async UniTaskVoid Attack() { //after need to change to abstract function
        await UniTask.NextFrame();
    }

    //public abstract void Move();

    #endregion //public funcs





    #region protected funcs

    protected abstract void AttackMove();

    virtual protected async UniTaskVoid ChangeColor()//�ǰ� �� ���� ������ ���� �� ����
    {
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

    #endregion //protected funcs
}