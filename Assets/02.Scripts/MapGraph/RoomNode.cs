using UnityEngine;

public class RoomNode : ScriptableObject {


    public RoomType _roomType;
    [HideInInspector] public int ID;
    [HideInInspector] public Vector2 _position;
    [HideInInspector] public string _roomName;
    public bool _leftLinked = false;
    public bool _rightLinked = false;
    public bool _upLinked = false;
    public bool _downLinked = false;

    public static RoomNode Create(int id, Vector2 pos) {
        RoomNode node = CreateInstance<RoomNode>();
        node.ID = id;
        node._position = pos;
        node._roomName = "Node " + id;
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
        switch (linkType) {
            case RoomLinkType.Left:
                _leftLinked = isLinked;
                break;
            case RoomLinkType.Right:
                _rightLinked = isLinked;
                break;
            case RoomLinkType.Up:
                _upLinked = isLinked;
                break;
            case RoomLinkType.Down:
                _downLinked = isLinked;
                break;
        }
    }

    public bool GetLinkState(RoomLinkType linkType) {
        bool state = false;
        switch (linkType) {
            case RoomLinkType.Left:
                state = _leftLinked;
                break;
            case RoomLinkType.Right:
                state = _rightLinked;
                break;
            case RoomLinkType.Up:
                state = _upLinked;
                break;
            case RoomLinkType.Down:
                state = _downLinked;
                break;
        }
        return state;
    }
}