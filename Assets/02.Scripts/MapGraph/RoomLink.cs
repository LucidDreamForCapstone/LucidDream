using UnityEngine;

public class RoomLink : ScriptableObject {
    public RoomNode _room1;
    public RoomNode _room2;
    public RoomLinkType _linkType;

    public void SetLink(RoomNode host, RoomNode target) {
        if (!host.GetLinkState(_linkType) && !target.GetLinkState(_linkType)) {
            _room1 = host;
            _room2 = target;
        }
        else
            Debug.Log("Host or Target already has same type of link");
    }

    public void DeleteLink() {
        _room1.SetLinkState(_linkType, false);
        _room2.SetLinkState(_linkType, false);
    }
}
