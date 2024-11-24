using UnityEngine;

public abstract class ShopItemBase : MonoBehaviour, Interactable {
    [Header("아이템 가격")]
    [SerializeField] private int _price;
    [SerializeField] private string _name;
    [SerializeField] protected string _warningMessage;
    protected InGameUIController inGameUIController;
    private bool _isUsed;
    private Animator _animator;

    #region Mono Behaviours

    private void Start() {
        inGameUIController = FindObjectOfType<InGameUIController>();
        _isUsed = false;
        _animator = GetComponent<Animator>();
    }
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            OpenShopItem();
        }
    }

    #endregion

    #region Protected Methods

    protected abstract void ItemEffect();
    protected abstract bool CanPurchase(); // 구매 가능 여부를 판단하는 메서드
    #endregion

    #region Public Methods
    public string GetName() { return _name; }

    public bool IsInteractBlock() {
        return _isUsed;
    }

    public string GetInteractText() {
        return $"[{_name}] 구매 (G)";
    }

    #endregion

    #region Private Methods

    private void OpenShopItem() {
        if (!_isUsed && InteractManager.Instance.CheckInteractable(this)) {
            Debug.Log("Player attempted to buy an item.");
            Debug.Log($"Current Coins: {PlayerDataManager.Instance.Status._coin}, Item Price: {_price}");
            if (Input.GetKey(KeyCode.G)) {
                InteractManager.Instance.InteractCoolTime().Forget();

                if (!CanPurchase()) // 아이템 구매 가능 여부 확인
                {
                    Debug.Log("Cannot purchase item due to unmet conditions.");
                    return; // 구매 중단
                }

                if (PlayerDataManager.Instance.BuyItem(_price)) {
                    Debug.Log("Item purchased successfully.");
                    ItemEffect();
                    _isUsed = true;
                }
                else {
                    Debug.Log("Not enough coins.");
                    string message = "코인이 부족합니다."; //Coin E Bujokhapnida
                    SystemMessageManager.Instance.PushSystemMessage(message, Color.red);
                }
            }
        }

    }
    #endregion
}

