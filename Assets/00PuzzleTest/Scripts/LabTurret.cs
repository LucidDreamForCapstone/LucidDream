using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class LabTurret : Turret {
    [SerializeField] TurretPuzzleManager _puzzleManager;

    protected override void Groggy() {
        base.Groggy();
        Die().Forget();
    }

    protected async override UniTaskVoid Die() {
        _isDead = true;
        _hp = 0;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        _animator.SetTrigger("Die");
        _puzzleManager.DecreaseTurretCount();
        await UniTask.Delay(TimeSpan.FromSeconds(_dieDelay));
    }

    protected override void FireHomingExplosive() {
        GameObject projectile = ObjectPool.Instance.GetObject(_missileObj);
        TurretMissile projectileScript = projectile.GetComponent<TurretMissile>();
        Vector2 targetDir = _player2.transform.position - transform.position;
        projectile.transform.right = Vector2.right;
        _firePosObj.transform.localPosition = _firePosVec[0];
        if (targetDir.x < 0) {
            projectile.transform.right *= -1;
            _firePosObj.transform.localPosition = _firePosVec[1];
        }

        projectile.transform.position = _firePosObj.transform.position;
        projectileScript.UseLab();
        projectileScript.SetPlayer2(_player2);
        projectileScript.SetSpeed(_fireSpeed);
        projectileScript.SetDmg(_damage);
        projectileScript.SetExplodeRadius(_explodeRadius);
        projectileScript.SetLastTime(_fireLastTime);
        projectileScript.SetHomingStartTime(_homingStartTime);
        projectileScript.SetHomingLastTime(_homingLastTime);
        projectileScript.SetHomingStrength(_homingStrength);
        projectileScript.SetGroggyDecreaseAmount(_missileGroggyGaugeDecreaseAmount);
        projectile.SetActive(true);
    }
}
