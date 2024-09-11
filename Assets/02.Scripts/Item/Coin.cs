using UnityEngine;

public class Coin : ItemBase {
    #region serialize field

    [SerializeField] int coinAmount;

    #endregion //serialize field

    #region mono funcs

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Player>() && _isGround) {
            PlayerDataManager.Instance.SetCoin(PlayerDataManager.Instance.Status._coin + coinAmount);
            ObjectPool.Instance.ReturnObject(gameObject);
        }
    }

    #endregion //mono funcs
}
