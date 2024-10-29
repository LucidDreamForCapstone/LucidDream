using UnityEngine;

public class CheckHp : DecoratorNode {
    [Header("Check if more than this (%)")]
    public float _targetHpPercent;
    protected override void OnStart() {
        _child._monster = _monster;
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        if (_monster.GetHpPercent() < _targetHpPercent * 0.01f) {
            return State.Failure;
        }
        else {
            _child.Update();
            return State.Success;
        }
    }
}
