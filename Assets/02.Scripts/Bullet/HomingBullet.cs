using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HomingBullet : ExplosiveBullet {
    float _homingStartTime;
    float _homingLastTime;

    protected override void OnEnable() {
        base.OnEnable();
        Homing().Forget();
    }

    public void SetHomingStartTime(float starttime) { _homingStartTime = starttime; }
    public void SetHomingLastTime(float lasttime) { _homingLastTime = lasttime; }

    private async UniTaskVoid Homing() {
        float timer = 0;
        await UniTask.Delay(TimeSpan.FromSeconds(_homingStartTime));
        while (timer < _homingLastTime && !_isDead) {
            if (Time.timeScale > 0) {
                Vector2 lookAt = _playerScript.transform.position - transform.position;
                float t = timer * 0.005f;
                transform.right = Vector3.Slerp(transform.right, lookAt, t);
                _rigid.velocity = transform.right * _fireSpeed;
                timer += Time.deltaTime;
            }
            await UniTask.NextFrame();
        }
    }
}
