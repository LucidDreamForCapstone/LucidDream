public class CheckSpawnNode : DecoratorNode {
    protected override void OnStart() {
        _child._monster = _monster;
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        if (!_monster._isSpawnComplete) {
            return State.Failure;
        }
        else {
            _child.Update();
            return State.Success;
        }
    }
}
