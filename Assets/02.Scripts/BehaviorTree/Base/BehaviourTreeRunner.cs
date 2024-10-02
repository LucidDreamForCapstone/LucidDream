using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour {
    BehaviourTree _tree;

    private void Start() {
        _tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var logNode1 = ScriptableObject.CreateInstance<DebugLogNode>();
        logNode1._message = "111";
        var pause1 = ScriptableObject.CreateInstance<WaitNode>();

        var logNode2 = ScriptableObject.CreateInstance<DebugLogNode>();
        logNode2._message = "222";
        var pause2 = ScriptableObject.CreateInstance<WaitNode>();

        var logNode3 = ScriptableObject.CreateInstance<DebugLogNode>();
        logNode3._message = "333";
        var pause3 = ScriptableObject.CreateInstance<WaitNode>();

        var sequence = ScriptableObject.CreateInstance<SequencerNode>();
        sequence._children.Add(logNode1);
        sequence._children.Add(pause1);
        sequence._children.Add(logNode2);
        sequence._children.Add(pause2);
        sequence._children.Add(logNode3);
        sequence._children.Add(pause3);

        var loop = ScriptableObject.CreateInstance<RepeatNode>();
        loop._child = sequence;
        _tree._rootNode = loop;
    }

    private void Update() {
        _tree.Update();
    }
}
