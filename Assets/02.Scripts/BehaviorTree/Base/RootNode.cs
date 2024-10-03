using UnityEngine;

public class RootNode : Node {
    [HideInInspector] public Node _child;

    protected override void OnStart() {

    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        return _child.Update();
    }

    public override Node Clone() {
        RootNode node = Instantiate(this);
        node._child = _child.Clone();
        return node;
    }
}
