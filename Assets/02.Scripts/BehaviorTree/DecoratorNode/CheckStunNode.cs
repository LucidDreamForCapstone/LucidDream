public class CheckStunNode : DecoratorNode {
    protected override void OnStart() {
        _child._monster = _monster;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (_monster._isStun) {
            _monster._rigid.velocity = UnityEngine.Vector2.zero;
            return State.Failure;
        }
        else {
            _child.Update();
            return State.Success;
        }
    }
}
