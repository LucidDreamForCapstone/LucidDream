using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    #region private variable

    private static ObjectPool _instance;

    #endregion //private variable





    #region serialize field
    [SerializeField] List<ObjectPoolData> _poolingObjectDatas;
    [SerializeField] public Transform _parent;

    #endregion //serialize field




    #region private variable

    Dictionary<string, Queue<GameObject>> _pooledObjects = new Dictionary<string, Queue<GameObject>>();

    #endregion //private variable





    #region property

    public static ObjectPool Instance { get { return _instance; } }

    #endregion //propery





    #region mono funcs


    private void Start() {
        InitialPooling();
    }
    private void Awake() {
        _instance = this;
    }

    #endregion //mono funcs





    #region public funcs

    private void InitialPooling() {
        _pooledObjects = new Dictionary<string, Queue<GameObject>>();
        for (int i = 0; i < _poolingObjectDatas.Count; i++) {
            if (!_pooledObjects.ContainsKey(_poolingObjectDatas[i]._poolObj.name) || _pooledObjects[_poolingObjectDatas[i]._poolObj.name].Count == 0) {
                Queue<GameObject> newQueue = new Queue<GameObject>();
                _pooledObjects.Add(_poolingObjectDatas[i]._poolObj.name, newQueue);
                for (int j = 0; j < _poolingObjectDatas[i]._poolAmount; j++) {
                    GameObject newObject = Instantiate(_poolingObjectDatas[i]._poolObj, transform);
                    newObject.transform.SetParent(_parent);
                    int index = newObject.name.IndexOf("(Clone)");
                    if (index > 0)
                        newObject.name = newObject.name.Substring(0, index);
                    newObject.SetActive(false);
                    _pooledObjects[_poolingObjectDatas[i]._poolObj.name].Enqueue(newObject);
                }
            }
        }
    }

    public GameObject GetObject(GameObject getObject) {
        GameObject returnObject;
        if (_pooledObjects.ContainsKey(getObject.name)) {
            if (_pooledObjects[getObject.name].Count != 0) {
                returnObject = _pooledObjects[getObject.name].Dequeue();
            }
            else {
                returnObject = Instantiate(getObject);
                returnObject.SetActive(false);
            }
            //returnObject.transform.SetParent(null);
            return returnObject;
        }
        else {
            returnObject = AddPool(getObject);
            return returnObject;
        }
    }

    public void ReturnObject(GameObject returnObject) {
        if (_pooledObjects.ContainsKey(returnObject.name)) {
            returnObject.transform.SetParent(_parent);
            returnObject.SetActive(false);
            _pooledObjects[returnObject.name].Enqueue(returnObject);
        }
        else {
            returnObject.transform.SetParent(_parent);
            returnObject.SetActive(false);
            Queue<GameObject> newQueue = new Queue<GameObject>();
            _pooledObjects.Add(returnObject.name, newQueue);
            _pooledObjects[returnObject.name].Enqueue(returnObject);
        }
    }

    public void DisableAllObjects() {
        foreach (Queue<GameObject> q in _pooledObjects.Values)
            foreach (GameObject poolObj in q)
                poolObj.SetActive(false);
    }

    #endregion //public funcs





    #region private funcs

    private GameObject AddPool(GameObject addObject) {
        Debug.Log("*******DO NOT USE OBJECT POOL WITH UNAUTOHRIZED OBJECT********");
        Queue<GameObject> newQueue = new Queue<GameObject>();
        _pooledObjects.Add(addObject.name, newQueue);
        for (int i = 0; i < 10; i++) {
            GameObject newObject = Instantiate(addObject);
            int index = newObject.name.IndexOf("(Clone)");
            if (index > 0)
                newObject.name = newObject.name.Substring(0, index);
            newObject.transform.SetParent(_parent);
            newObject.SetActive(false);
            _pooledObjects[addObject.name].Enqueue(newObject);
        }
        return _pooledObjects[addObject.name].Dequeue();
    }

    #endregion //private funcs
}



