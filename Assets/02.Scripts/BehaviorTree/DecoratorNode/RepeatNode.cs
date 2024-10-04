public class RepeatNode : DecoratorNode {
    protected override void OnStart() {
        _child._monster = _monster;
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        _child.Update();
        return State.Running;
    }
}
