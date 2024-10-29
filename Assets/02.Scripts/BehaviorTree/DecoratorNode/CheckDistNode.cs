using UnityEngine;
public class CheckDistNode : DecoratorNode {
    public float _min, _max;

    protected override void OnStart() {
        _child._monster = _monster;
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        float sqrDist = ((Vector2)_monster._playerScript.transform.position - (Vector2)_monster.transform.position).sqrMagnitude;
        if (_min * _min < sqrDist && sqrDist < _max * _max) {
            _child.Update();
            return State.Success;
        }
        else
            return State.Failure;
    }
}
