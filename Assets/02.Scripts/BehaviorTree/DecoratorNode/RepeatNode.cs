public class RepeatNode : DecoratorNode {
    protected override void OnStart() {

    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        _child.Update();
        return State.Running;
    }
}
