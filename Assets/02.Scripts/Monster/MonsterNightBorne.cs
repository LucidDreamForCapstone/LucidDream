using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class MonsterNightBorne : MonsterBase {

    [SerializeField] GameObject _phantomGhostObj;
    [SerializeField] Color32 _phantomGhostColor;
    [SerializeField] float _phantomLastTime;
    [SerializeField] float _ghostLastTime;
    [SerializeField] float _ghostInterval;
    [SerializeField] float _ghostCoolTime;

    bool _isPhantomReady;
    bool _isPhantomActivated;

    private void Start() {
        _isPhantomReady = true;
        _isPhantomActivated = false;
        ActivatePhantom().Forget();
        Move().Forget();
    }

    private async UniTaskVoid Move() {
        while (true) {
            float randomX = UnityEngine.Random.Range(-1.0f, 1);
            float randomY = UnityEngine.Random.Range(-1.0f, 1);
            Vector2 moveVec = new Vector2(randomX, randomY);
            if (moveVec.x < 0)
                _spriteRenderer.flipX = true;
            else
                _spriteRenderer.flipX = false;
            _rigid.velocity = moveVec.normalized * _moveSpeed;
            await UniTask.Delay(TimeSpan.FromSeconds(1));
        }
    }

    private async UniTaskVoid ActivatePhantom() {
        while (true) {
            if (_isPhantomReady) {
                _animator.SetBool("Dash", true);
                _moveSpeed += 20;
                _isPhantomActivated = true;
                _isPhantomReady = false;
                PhantomGhostEffect().Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(_phantomLastTime));
                _isPhantomActivated = false;
                _animator.SetBool("Dash", false);
                _moveSpeed -= 20;
                await UniTask.Delay(TimeSpan.FromSeconds(_ghostCoolTime));
                _isPhantomReady = true;
            }
        }
    }

    private async UniTaskVoid PhantomGhostEffect() {
        while (_isPhantomActivated) {
            SinglePhantomGhost().Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_ghostInterval), ignoreTimeScale: true);
        }
    }

    private async UniTaskVoid SinglePhantomGhost() {
        GameObject ghostEffect = ObjectPool.Instance.GetObject(_phantomGhostObj);
        SpriteRenderer sr = ghostEffect.GetComponent<SpriteRenderer>();
        sr.color = _phantomGhostColor;
        sr.sprite = _spriteRenderer.sprite;
        if (_rigid.velocity.x < 0)
            sr.flipX = true;
        else
            sr.flipX = false;

        ghostEffect.transform.position = transform.position;
        ghostEffect.SetActive(true);
        await sr.DOFade(0, _ghostLastTime).SetUpdate(true);
        ObjectPool.Instance.ReturnObject(ghostEffect);
    }
}
