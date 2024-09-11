using System;
using UnityEngine;

[Serializable]
public class ItemData {
    public GameObject _item;
    public float _dropChance;

    public ItemData(GameObject item, float dropChance) {
        _item = item;
        _dropChance = dropChance;
    }
}
