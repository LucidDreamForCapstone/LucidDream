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
        // �ش� �������� �������ִ��� ���� ��ȯ
        // *** ��� ���� ***
        // if ( InventoryManager.Instance.HasItem(ItemType.Key)){
        //     ���� ���ϴ�.
        //     }

        return _items.Contains(item);
    }
    public bool RemoveItem(ItemType item)
    {
        // �ܼ��� �������� �����ϴ� �޼���
        if (_items.Contains(item))
        {
            _items.Remove(item);
            return true;
        }
        return false;
    }
    #endregion // public funcs
}
