using UnityEngine;

public class Bullet : MonoBehaviour {

    #region serialize field

    [SerializeField] protected bool _targetIsMonster;
    [SerializeField] protected bool _stunDebuff;
    [SerializeField] protected bool _slowDebuff;

    #endregion //serialize field




    #region private variable

    protected float _fireSpeed;
    protected float _lastTime;
    protected Rigidbody2D _rigid;
    float _timer;
    protected int _dmg; //only used when target is player
    protected float _multiplier; //only used when target is Monster
    protected Player _playerScript;
    protected float _stunTime;
    protected float _slowTime;
    protected float _slowRate;

    #endregion //private variable





    #region mono funcs

    protected void OnEnable() {
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.velocity = transform.right * _fireSpeed;
        _timer = 0;
    }
    private void Update() {
        Timer();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Wall"))
            Die();
        else {
            if (_targetIsMonster) {
                if (collision.CompareTag("Enemy"))
                    DamageToMonster(collision);
            }
            else {
                if (collision.CompareTag("Player") && !_playerScript.CheckInvincible())
                    DamageToPlayer(collision);
                else if (collision.GetComponent<Pillar>())
                    Die();
                else if (collision.CompareTag("Wall"))
                    Die();
            }
        }
    }

    #endregion //mono funcs





    #region public funcs

    public void SetSpeed(float fireSpeed) { _fireSpeed = fireSpeed; }
    public void SetDmg(int dmg) { _dmg = dmg; }
    public void SetMultiplier(float multiplier) { _multiplier = multiplier; }
    public void SetLastTime(float lastTime) { _lastTime = lastTime; }
    public void SetPlayer(Player player) { _playerScript = player; }
    public void SetStun(float stunTime) { _stunTime = stunTime; }
    public void SetSlow(float slowRate, float slowTime) { _slowRate = slowRate; _slowTime = slowTime; }

    #endregion //public funcs




    #region protected funcs

    virtual protected void DamageToMonster(Collider2D collision) {
        MonsterBase monster = collision.GetComponent<MonsterBase>();
        if (_stunDebuff)
            monster.Stun(_stunTime).Forget();
        if (_slowDebuff)
            monster.Slow(_slowRate, _slowTime).Forget();

        _playerScript.NormalMagicalAttack(monster, _multiplier);
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    virtual protected void DamageToPlayer(Collider2D collision) {
        if (_stunDebuff)
            _playerScript.Stun(_stunTime);
        if (_slowDebuff)
            _playerScript.Slow(_slowRate, _slowTime);

        _playerScript.Damaged(_dmg);
        ObjectPool.Instance.ReturnObject(gameObject);
    }


    virtual protected void Die() {
        ObjectPool.Instance.ReturnObject(gameObject);
    }
    #endregion




    #region private funcs
    private void Timer() {
        _timer += Time.deltaTime;
        if (_timer > _lastTime)
            Die();
    }

    #endregion //private funcs
}
