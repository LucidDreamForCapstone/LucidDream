using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player2DataManager : MonoBehaviour
{
    #region private variable
    private GameObject _player2;
    private Player_2 _player2Script;
    private static Player2DataManager _instance;
    #endregion // private variable


    public static Player2DataManager Instance { get { return _instance; } }
    public Player_2 Player2 { get { return _player2Script; } }

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        _instance = this;
    }

    private void Update() {
        _player2 ??= GameObject.Find("Player_2");
        if (_player2 != null)
            _player2Script ??= _player2.GetComponent<Player_2>();
    }
}
