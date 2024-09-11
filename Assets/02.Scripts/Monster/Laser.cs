using UnityEngine;

public class Laser : MonoBehaviour {
    /*
    [SerializeField] GameObject _laserWarningEffect;
    [SerializeField] GameObject _laserEffect;
    [SerializeField] float _laserWidth;
    SpriteRenderer _laserWarningEffectSr;
    Animator _laserEffectAnimator;

    #region mono funcs

    private void Start() {
        _laserWarningEffectSr = _laserWarningEffect.GetComponent<SpriteRenderer>();
        _laserEffectAnimator = _laserEffect.GetComponent<Animator>();
        _laserWarningEffect.SetActive(false);
        _laserEffect.SetActive(false);
    }

    #endregion

    #region public funcs
    public async UniTaskVoid LaserActivate(GameObject caster, Player playerScript, Vector2 dir, float chargeDelay, float lastTime, int damage, LayerMask layer) {
        _laserWarningEffect.SetActive(true);
        _laserWarningEffectSr.color = Color.red;
        Tween laserWarningTween = _laserWarningEffectSr.DOFade(0, 1).SetLoops(-1, LoopType.Restart);
        await UniTask.Delay(TimeSpan.FromSeconds(chargeDelay));
        laserWarningTween.Kill();
        _laserWarningEffect.SetActive(false);
        _laserEffect.SetActive(true);
        float timer = 0;
        while (timer < lastTime) {
            var hit = Physics2D.BoxCast(transform.position, new Vector2(_laserWidth, 1), 0, dir, 100f, layer);
            if (hit.collider != null) {
                playerScript.Damaged(damage);
            }
            timer += Time.deltaTime;
            await UniTask.NextFrame();
        }
        _laserEffectAnimator.SetTrigger("End");
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        _laserEffect.SetActive(false);
        transform.DOMove(caster.transform.position, 2);
        transform.DORotate(Vector3.zero, 2);
        ObjectPool.Instance.ReturnObject(gameObject);
    }
    #endregion
    */
}
