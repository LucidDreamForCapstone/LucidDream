using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharcterSetActiveFalse : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameObject;
    public void CharSetActiveFalse() {
        _gameObject[0].SetActive(false);
    }

    public void FakePortalSetTrue() {
        _gameObject[1].SetActive(true);    
    }

    public void FakePortalSetFalse() {
        if (_gameObject[1]) {
            _gameObject[1].SetActive(false);
        }
    }
}
