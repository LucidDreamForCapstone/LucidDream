using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class BondrewdMissile : HomingBullet {
    bool _isSelfTargetPrevented;
    float _preventTime = 2.0f;
    protected override void OnEnable() {
        PreventExplosionSelf().Forget();
        base.OnEnable();
    }
    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Wall"))
            Die();
        else if (_targetType == TargetType.Both) {
            if (collision.CompareTag("Player") && !_playerScript.CheckInvincible())
                Die();
            else if (collision.CompareTag("Enemy") && !_isSelfTargetPrevented)
                Die();
        }
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
            if (length > 0) {
                for (i = 0; i < length; i++) {
                    if (possibleTargets[i].GetComponent<BossBondrewd>()) {
                        var boss = possibleTargets[i].GetComponent<BossBondrewd>();
                        if (_stunDebuff)
                            boss.Stun(_stunTime).Forget();
                        if (_slowDebuff)
                            boss.Slow(_slowRate, _slowTime).Forget();

                        boss.DecreaseGroggyGauge();
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

    private async UniTaskVoid PreventExplosionSelf() {
        _isSelfTargetPrevented = true;
        await UniTask.Delay(TimeSpan.FromSeconds(_preventTime));
        _isSelfTargetPrevented = false;
    }
}
