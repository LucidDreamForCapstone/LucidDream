using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharcterSetActiveFalse : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    public void CharSetActiveFalse() {
        _gameObject.SetActive(false);
    }
}
