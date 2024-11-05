using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HomingBullet : ExplosiveBullet {
    float _homingStartTime;
    float _homingLastTime;

    new private void OnEnable() {
        base.OnEnable();
        Homing().Forget();
    }

    public void SetHomingStartTime(float starttime) { _homingStartTime = starttime; }
    public void SetHomingLastTime(float lasttime) { _homingLastTime = lasttime; }


    private async UniTaskVoid Homing() {
        float timer = 0;
        await UniTask.Delay(TimeSpan.FromSeconds(_homingStartTime));
        while (timer < _homingLastTime) {
            Vector2 lookAt = _playerScript.transform.position - transform.position;
            transform.right = Vector3.Slerp(transform.right, lookAt, timer * 0.003f);
            _rigid.velocity = transform.right * _fireSpeed;
            timer += Time.deltaTime;
            await UniTask.NextFrame();
        }
    }
}
