using UnityEngine;

public class Dream : ItemBase {

    #region serialize field

    [SerializeField] int dreamAmount;

    #endregion //serialize field

    #region mono funcs

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Player>() && _isGround) {
            PlayerDataManager.Instance.SetDream(PlayerDataManager.Instance.Status._dream + dreamAmount);
            ObjectPool.Instance.ReturnObject(gameObject);
        }
    }

    #endregion //mono funcs
}
