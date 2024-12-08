using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Pillar : MonsterBase {

    [SerializeField] float _layerBorder;

    #region mono funcs

    new private void Update() {
        ChangeLayer();
        UpdatePillarState();
    }
    #endregion


    #region protected funcs

    protected override async UniTaskVoid Die() {
        await UniTask.NextFrame();
        PlaySound(_deathSound);
        BossHades.PillarCntUp();
        _isDead = true;
        _hp = 0;
        _animator.SetTrigger("Die");
    }

    #endregion




    #region public funcs

    public void FadeEnd() {
        Color32 endColor = new Color32(255, 255, 255, 0);
        _spriteRenderer.DOColor(endColor, 1.5f)
            .OnComplete(() => {
                ObjectPool.Instance.ReturnObject(gameObject);
            });
    }

    #endregion



    #region private funcs

    private void ChangeLayer() {
        if (_playerScript.transform.position.y - transform.position.y > _layerBorder)
            _spriteRenderer.sortingOrder = 8;
        else
            _spriteRenderer.sortingOrder = 4;
    }

    private void UpdatePillarState() {
        float hpPercent = GetHpPercent();
        if (hpPercent < 0.25f) {
            _animator.SetTrigger("Phase3");
        }
        else if (hpPercent < 0.5f) {
            _animator.SetTrigger("Phase2");
        }
        else if (hpPercent < 0.75f) {
            _animator.SetTrigger("Phase1");
        }
    }

    private float GetHpPercent() {
        return (float)_hp / (float)_maxHp;
    }
    #endregion

}
