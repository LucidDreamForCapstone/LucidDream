using UnityEngine;

public class CheckAliveNode : DecoratorNode {
    protected override void OnStart() {
        _child._monster = _monster;
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        if (_monster._isDead) {
            _monster._rigid.velocity = Vector2.zero;
            return State.Failure;
        }
        else {
            _child.Update();
            return State.Success;
        }
    }
}
