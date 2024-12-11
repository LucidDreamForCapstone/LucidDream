using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HomingBullet : ExplosiveBullet {
    protected bool _forLab = false;
    protected Player_2 _player2;
    float _homingStartTime;
    float _homingLastTime;

    protected override void OnEnable() {
        base.OnEnable();
        Homing().Forget();
    }

    public void SetHomingStartTime(float starttime) { _homingStartTime = starttime; }
    public void SetHomingLastTime(float lasttime) { _homingLastTime = lasttime; }

    public void SetPlayer2(Player_2 player2) {
        _player2 = player2;
    }
    public void UseLab() {
        _forLab = true;
    }

    private async UniTaskVoid Homing() {
        float timer = 0;
        await UniTask.Delay(TimeSpan.FromSeconds(_homingStartTime));
        while (timer < _homingLastTime && !_isDead) {
            if (Time.timeScale > 0) {
                Vector2 lookAt;
                if (_forLab)
                    lookAt = _player2.transform.position - transform.position;
                else
                    lookAt = _playerScript.transform.position - transform.position;
                float t = timer * 0.005f;
                transform.right = Vector3.Slerp(transform.right, lookAt, t);
                _rigid.velocity = transform.right * _fireSpeed;
                timer += Time.deltaTime;
            }
            await UniTask.NextFrame();
        }
    }
}
