using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ItemBase : MonoBehaviour, Interactable {

    #region protected variable

    protected bool _isGround;

    #endregion //protected variable




    #region public funcs
    public void Drop() {
        _isGround = false;
        float duration = 1.5f;
        float moveDistance = 3;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(0, moveDistance, 0);

        transform.DORotate(new Vector3(0, 0, 1080), duration, RotateMode.LocalAxisAdd);
        transform.DOMove(targetPos, duration / 2)
            .OnComplete(() => {
                transform.DOMove(startPos, duration / 2)
                .OnComplete(() => { _isGround = true; })
                .SetEase(Ease.InCubic);
            })
           .SetEase(Ease.OutCubic);
    }

    public void DropRandom() {
        _isGround = false;
        float duration = 1.5f;
        float moveDistance = 3;
        Vector3 randomPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        Vector3 startPos = transform.position + randomPos;
        Vector3 targetPos = startPos + new Vector3(0, moveDistance, 0);

        transform.DORotate(new Vector3(0, 0, 1080), duration, RotateMode.LocalAxisAdd);
        transform.DOMove(targetPos, duration / 2)
            .OnComplete(() => {
                transform.DOMove(startPos, duration / 2)
                .SetEase(Ease.InCubic)
                .OnComplete(() => { _isGround = true; });
            })
           .SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// You MUST Override this function when you want this item as EQUIPMENT.
    /// </summary>
    /// <returns></returns>
    virtual public bool IsInteractBlock() {
        return true;
    }

    virtual public string GetInteractText() {
        return "";
    }

    #endregion //public funcs
}
