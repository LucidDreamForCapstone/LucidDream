using UnityEngine;

public class ExplosiveBullet : Bullet { //lastTime 이후에 자멸할 때 터지는 것은 고려하지 않았음 따라서 현재는 Player에 대한 폭발 코드는 의미가 없는 상태임

    [SerializeField] GameObject _shockWaveObj;
    [SerializeField] Color _shockWaveColor;
    [SerializeField] AudioClip _explositionClip;
    protected float _explodeRadius;
    LayerMask _monsterLayer;
    LayerMask _playerLayer;

    private void Start() {
        _monsterLayer = LayerMask.GetMask("Enemy", "CollidableEnemy");
        _playerLayer = LayerMask.GetMask("Player");
    }

    protected override void DamageToMonster(Collider2D collision) {
        var possibleTargets = Physics2D.OverlapCircleAll(collision.transform.position, _explodeRadius, _monsterLayer);
        int i, length = possibleTargets.Length;
        MonsterBase monster;
        if (length > 0)
            for (i = 0; i < length; i++) {
                monster = possibleTargets[i].GetComponent<MonsterBase>();
                if (_stunDebuff)
                    monster.Stun(_stunTime).Forget();
                if (_slowDebuff)
                    monster.Slow(_slowRate, _slowTime).Forget();
                _playerScript.NormalMagicalAttack(monster, _multiplier);
            }

        GameObject shockWave = ObjectPool.Instance.GetObject(_shockWaveObj);
        ShockWave shockWaveScript = shockWave.GetComponent<ShockWave>();
        shockWaveScript.SetRadius(_explodeRadius);
        shockWaveScript.SetColor(_shockWaveColor);
        shockWave.transform.position = transform.position;
        shockWave.SetActive(true);
        SoundManager.Instance.PlaySFX(_explositionClip.name);
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    protected override void DamageToPlayer(Collider2D collision) {
        if (_stunDebuff)
            _playerScript.Stun(_stunTime);
        if (_slowDebuff)
            _playerScript.Slow(_slowRate, _slowTime);

        var target = Physics2D.OverlapCircle(collision.transform.position, _explodeRadius, _playerLayer);
        if (target != null)
            _playerScript.Damaged(_dmg);

        GameObject shockWave = ObjectPool.Instance.GetObject(_shockWaveObj);
        ShockWave shockWaveScript = shockWave.GetComponent<ShockWave>();
        shockWaveScript.SetRadius(_explodeRadius);
        shockWaveScript.SetColor(_shockWaveColor);
        shockWave.transform.position = transform.position;
        shockWave.SetActive(true);
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    protected override void Die() {
        if (_targetIsMonster) {
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
                    _playerScript.NormalMagicalAttack(monster, _multiplier);
                }
            }
            GameObject shockWave = ObjectPool.Instance.GetObject(_shockWaveObj);
            ShockWave shockWaveScript = shockWave.GetComponent<ShockWave>();
            shockWaveScript.SetRadius(_explodeRadius);
            shockWaveScript.SetColor(_shockWaveColor);
            shockWave.transform.position = transform.position;
            shockWave.SetActive(true);
            SoundManager.Instance.PlaySFX(_explositionClip.name);
            ObjectPool.Instance.ReturnObject(gameObject);
        }
        else {
            if (_stunDebuff)
                _playerScript.Stun(_stunTime);
            if (_slowDebuff)
                _playerScript.Slow(_slowRate, _slowTime);

            var target = Physics2D.OverlapCircle(transform.position, _explodeRadius, _playerLayer);
            if (target != null)
                _playerScript.Damaged(_dmg);

            GameObject shockWave = ObjectPool.Instance.GetObject(_shockWaveObj);
            ShockWave shockWaveScript = shockWave.GetComponent<ShockWave>();
            shockWaveScript.SetRadius(_explodeRadius);
            shockWaveScript.SetColor(_shockWaveColor);
            shockWave.transform.position = transform.position;
            shockWave.SetActive(true);
            //SoundManager.Instance.PlaySFX(_explositionClip.name);
            ObjectPool.Instance.ReturnObject(gameObject);
        }
    }




    public void SetExplodeRadius(float radius) { _explodeRadius = radius; }
}
