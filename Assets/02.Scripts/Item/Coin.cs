using UnityEngine;

public class Coin : ItemBase {
    #region serialize field

    [SerializeField] int coinAmount;
    [SerializeField] AudioClip _coinClip;
    #endregion //serialize field

    #region mono funcs  
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Player>() && _isGround) {
            SoundManager.Instance.PlaySFX(_coinClip.name, false);
            PlayerDataManager.Instance.SetCoin(PlayerDataManager.Instance.Status._coin + coinAmount);
            ObjectPool.Instance.ReturnObject(gameObject);
        }
    }

    #endregion //mono funcs
}
