using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class DropableBase : MonoBehaviour, Interactable {
    #region serialize field

    [SerializeField] List<ItemData> dropTable; //<¾ÆÀÌÅÛ, È®·ü>

    #endregion //serialize field





    #region public funcs
    virtual public bool IsInteractBlock() {
        return true;
    }

    virtual public string GetInteractText() {
        return "";
    }

    public Transform GetTransform() {
        return transform;
    }
    #endregion



    #region protected funcs

    protected void DropItems() {
        int i;
        for (i = 0; i < dropTable.Count; i++)
            if (CheckItemDrop(dropTable[i])) {
                GameObject dropItem = ObjectPool.Instance.GetObject(dropTable[i]._item);
                dropItem.transform.position = transform.position;
                dropItem.SetActive(true);
                dropItem.GetComponent<ItemBase>().DropRandom();
            }
    }

    protected void DropOneItem(GameObject dropPoint) {
        int randomItemIndex = GetRandomSelectedItemIndex();
        GameObject dropItem = ObjectPool.Instance.GetObject(dropTable[randomItemIndex]._item);
        dropItem.transform.position = dropPoint.transform.position;
        dropItem.SetActive(true);
        dropItem.GetComponent<ItemBase>().DropRandom();
    }

    #endregion //protected funcs





    #region private funcs

    private bool CheckItemDrop(ItemData itemData) {
        float randomN = Random.Range(0, 1.0f);
        if (randomN <= itemData._dropChance * 0.01f)
            return true;
        else
            return false;
    }

    private List<float> GetAccumulatedChanceList() {
        List<float> accumulatedList = new List<float>();
        accumulatedList.Add(0);
        for (int i = 0; i < dropTable.Count; i++) {
            accumulatedList.Add(accumulatedList[i] + dropTable[i]._dropChance * 0.01f);
        }
        return accumulatedList;
    }

    private int GetRandomSelectedItemIndex() {
        List<float> accumChanceList = GetAccumulatedChanceList();
        float randomN = Random.Range(0, 1.0f);
        int i;
        for (i = 1; i < accumChanceList.Count; i++) {
            if (accumChanceList[i - 1] <= randomN && randomN < accumChanceList[i])
                return i - 1;
        }

        if (randomN == accumChanceList.Last())
            return i - 2;

        return 0;
    }
    #endregion //private funcs

}
