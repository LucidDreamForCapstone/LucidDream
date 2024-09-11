using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    #region private variable

    private static InventoryManager _instance;
    private List<ItemType> _items = new();

    #endregion // private variable





    #region properties

    public static InventoryManager Instance { get { return _instance; } }
    public List<ItemType> InvenItem { get { return _items; } }

    #endregion // properties





    #region mono funcs

    void Start() {
        DontDestroyOnLoad(this.gameObject);
        _instance = this;
    }

    #endregion // mono funcs





    #region public funcs

    public void AddItem(ItemType item) {
        _items.Add(item);
    }

    public bool UseItem(ItemType item) {
        if (_items.Contains(item)) {
            _items.Remove(item);
            return true;
        }
        return false;
    }

    public bool HasItem(ItemType item) {
        // 해당 아이템을 가지고있는지 여부 반환
        // *** 사용 예시 ***
        // if ( InventoryManager.Instance.HasItem(ItemType.Key)){
        //     문을 엽니다.
        //     }

        return _items.Contains(item);
    }
    public bool RemoveItem(ItemType item)
    {
        // 단순히 아이템을 제거하는 메서드
        if (_items.Contains(item))
        {
            _items.Remove(item);
            return true;
        }
        return false;
    }
    #endregion // public funcs
}
