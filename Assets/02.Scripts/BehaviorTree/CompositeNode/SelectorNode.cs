public class SelectorNode : CompositeNode {

    int _current;
    protected override void OnStart() {
        _current = 0;
        int length = _children.Count;
        for (int i = 0; i < length; i++) {
            _children[i]._monster = _monster;
        }
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        Node childNode = _children[_current];
        switch (childNode.Update()) {
            case State.Running:
                return State.Running;
            case State.Success:
                return State.Success;
            case State.Failure:
                _current++;
                break;
        }

        return _current == _children.Count ? State.Failure : State.Running;
    }
}
