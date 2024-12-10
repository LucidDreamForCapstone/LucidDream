using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

[System.Serializable]
public class ShowDialogBoxAsset : PlayableAsset
{
    public string name;
    public List<DialogMessage> messages = new List<DialogMessage>();
    public float duration = 3.0f;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        var playable = ScriptPlayable<ShowDialogBoxBehaviour>.Create(graph);

        // �����͸� behaviour�� ����
        var behaviour = playable.GetBehaviour();
        behaviour.name = name;
        behaviour.messages = messages;
        behaviour.duration = duration;

        return playable;
    }
}
