using UnityEngine;

public class ExplosiveBullet : Bullet {

    [SerializeField] GameObject _shockWaveObj;
    [SerializeField] Color _shockWaveColor;
    [SerializeField] AudioClip _explositionClip;
    protected float _explodeRadius;
    protected LayerMask _monsterLayer;
    protected LayerMask _playerLayer;
    protected LayerMask _bothLayer;

    private void Start() {
        _monsterLayer = LayerMask.GetMask("Enemy", "CollidableEnemy");
        _playerLayer = LayerMask.GetMask("Player");
        _bothLayer = LayerMask.GetMask("Enemy", "Player");
    }

    protected override void DamageToMonster(Collider2D collision) {
    }

    protected override void DamageToPlayer(Collider2D collision) {
    }

    protected override void Die() {
        if (_targetType == TargetType.Monster) {
            var possibleTargets = Physics2D.OverlapCircleAll(transform.position, _explodeRadius, _monsterLayer);
            int i, length = possibleTargets.Length;
            MonsterBase monster;
            if (length > 0) {
                for (i = 0; i < length; i++) {
                    monster = possibleTargets[i].GetComponent<MonsterBase>();
                    if (_stunDebuff)
                        monster.Stun(_stunTime).Forget();
                    if (_slowDebuff)
                        monster.Slow(_slowRate, _slowTime).Forget();
                    _playerScript.NormalMagicalAttack(monster, _multiplier); //need to be fixed if this is used by physical weapon
                }
            }
        }
        else if (_targetType == TargetType.Player) {
            if (_stunDebuff)
                _playerScript.Stun(_stunTime);
            if (_slowDebuff)
                _playerScript.Slow(_slowRate, _slowTime);

            var target = Physics2D.OverlapCircle(transform.position, _explodeRadius, _playerLayer);
            if (target != null)
                _playerScript.Damaged(_dmg);
        }
        else if (_targetType == TargetType.Both) {
            var possibleTargets = Physics2D.OverlapCircleAll(transform.position, _explodeRadius, _bothLayer);
            int i, length = possibleTargets.Length;
            MonsterBase monster;
            if (length > 0) {
                for (i = 0; i < length; i++) {
                    if (possibleTargets[i].CompareTag("Enemy")) {
                        monster = possibleTargets[i].GetComponent<MonsterBase>();
                        if (_stunDebuff)
                            monster.Stun(_stunTime).Forget();
                        if (_slowDebuff)
                            monster.Slow(_slowRate, _slowTime).Forget();
                        _playerScript.NormalMagicalAttack(monster, _multiplier); //need to be fixed if this is used by physical weapon
                    }
                    else if (possibleTargets[i].CompareTag("Player")) {
                        if (_stunDebuff)
                            _playerScript.Stun(_stunTime);
                        if (_slowDebuff)
                            _playerScript.Slow(_slowRate, _slowTime);

                        _playerScript.Damaged(_dmg);
                    }
                }
            }
        }
        ShockWaveEffect();
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    public void SetExplodeRadius(float radius) { _explodeRadius = radius; }

    protected void ShockWaveEffect() {
        GameObject shockWave = ObjectPool.Instance.GetObject(_shockWaveObj);
        ShockWave shockWaveScript = shockWave.GetComponent<ShockWave>();
        shockWaveScript.SetRadius(_explodeRadius);
        shockWaveScript.SetColor(_shockWaveColor);
        shockWave.transform.position = transform.position;
        shockWave.SetActive(true);
        if (_explositionClip != null)
            SoundManager.Instance.PlaySFX(_explositionClip.name);
    }
}
