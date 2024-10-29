using System.Collections.Generic;
using UnityEngine;

public class RoomNode : ScriptableObject {


    public RoomType _roomType;
    [HideInInspector] public int ID;
    [HideInInspector] public Vector2 _position;
    [HideInInspector] public string _roomName;
    private List<bool> _isLinked = new List<bool>();

    private void Awake() {
        for (int i = 0; i < 4; i++)
            _isLinked.Add(false);
    }

    public static RoomNode CreateInstance(int id, Vector2 pos) {
        RoomNode node = CreateInstance<RoomNode>();
        node.ID = id;
        node._position = pos;
        node._roomName = "Node " + id;

        // 이름 설정 (중요)
        node.name = "Node " + id;

        return node;
    }

    public void UpdateRoomName() {
        switch (_roomType) {
            case RoomType.Undefined:
                _roomName = "Undefined";
                break;
            case RoomType.Entrance:
                _roomName = "Entrance";
                break;
            case RoomType.Normal:
                _roomName = "Normal";
                break;
            case RoomType.Boss:
                _roomName = "Boss";
                break;
            case RoomType.Exit:
                _roomName = "Exit";
                break;
            case RoomType.Hub:
                _roomName = "Hub";
                break;
            case RoomType.Reward:
                _roomName = "Reward";
                break;
            case RoomType.Shop:
                _roomName = "Shop";
                break;
            case RoomType.Secret:
                _roomName = "Secret";
                break;
        }
    }

    public void SetLinkState(RoomLinkType linkType, bool isLinked) {
        _isLinked[(int)linkType] = isLinked;
    }

    public bool GetLinkState(RoomLinkType linkType) {
        return _isLinked[(int)linkType];
    }
}