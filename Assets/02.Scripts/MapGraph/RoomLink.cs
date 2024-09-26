using UnityEngine;

public class RoomLink : ScriptableObject {
    public RoomNode _room1;
    public RoomNode _room2;
    public RoomLinkDir _linkDir;
    [HideInInspector] RoomLinkType _roomLinkType1;
    [HideInInspector] RoomLinkType _roomLinkType2;
    [HideInInspector] public int ID;

    public static RoomLink Create(int id, RoomLinkDir linkDir) {
        RoomLink link = CreateInstance<RoomLink>();
        link.ID = id;
        link._linkDir = linkDir;
        link.name = "Link " + id;

        return link;
    }

    public void SetLink(RoomNode host, RoomNode target) {
        _roomLinkType1 = GetHostLinkType(host._position, target._position);
        _roomLinkType2 = GetTargetLinkType(host._position, target._position);
        if (!host.GetLinkState(_roomLinkType1) && !target.GetLinkState(_roomLinkType2)) {
            _room1 = host;
            _room2 = target;
            _room1.SetLinkState(_roomLinkType1, true);
            _room2.SetLinkState(_roomLinkType2, true);
        }
        else
            Debug.Log("Host or Target already has same type of link");
    }

    public void DeleteLink() {
        _room1.SetLinkState(_roomLinkType1, false);
        _room2.SetLinkState(_roomLinkType2, false);
    }

    private RoomLinkType GetHostLinkType(Vector2 hostPos, Vector2 targetPos) {
        Vector2 diff = hostPos - targetPos;
        if (_linkDir == RoomLinkDir.Horizontal) {
            if (diff.x > 0)
                return RoomLinkType.Left;
            else
                return RoomLinkType.Right;
        }
        else if (_linkDir == RoomLinkDir.Vertical) {
            if (diff.y > 0)
                return RoomLinkType.Down;
            else
                return RoomLinkType.Up;
        }
        return RoomLinkType.Unknown;
    }

    private RoomLinkType GetTargetLinkType(Vector2 hostPos, Vector2 targetPos) {
        Vector2 diff = hostPos - targetPos;
        if (_linkDir == RoomLinkDir.Horizontal) {
            if (diff.x > 0)
                return RoomLinkType.Right;
            else
                return RoomLinkType.Left;
        }
        else if (_linkDir == RoomLinkDir.Vertical) {
            if (diff.y > 0)
                return RoomLinkType.Up;
            else
                return RoomLinkType.Down;
        }
        return RoomLinkType.Unknown;
    }
}
