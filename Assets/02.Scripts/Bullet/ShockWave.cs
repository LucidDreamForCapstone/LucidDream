using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class ShockWave : MonoBehaviour {

    float _radius;
    float _fadeStartTime;
    float _fadeDuration;
    Color _color;
    SpriteRenderer _spriteRenderer;


    #region public funcs

    public void SetRadius(float radius) { _radius = radius; }
    public void SetColor(Color color) { _color = color; }

    #endregion
    private void OnEnable() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = _color;
        _fadeStartTime = 0.3f;
        _fadeDuration = 0.3f;
        ShockWaveEffect().Forget();
    }

    private async UniTaskVoid ShockWaveEffect() {
        transform.DOScale(_radius, _fadeStartTime + _fadeDuration).ToUniTask().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_fadeStartTime));
        await _spriteRenderer.DOFade(0, _fadeDuration);
        transform.localScale = Vector3.one;
        ObjectPool.Instance.ReturnObject(gameObject);
    }
}
