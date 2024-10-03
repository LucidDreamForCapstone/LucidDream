using UnityEngine;

public abstract class Node : ScriptableObject {
    public enum State {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public State _state = State.Running;
    [HideInInspector] public bool _started = false;
    [HideInInspector] public string _guid;
    [HideInInspector] public Vector2 _position;
    [TextArea] public string description;

    public State Update() {
        if (!_started) {
            OnStart();
            _started = true;
        }

        _state = OnUpdate();

        if (_state == State.Failure || _state == State.Success) {
            OnStop();
            _started = false;
        }

        return _state;
    }

    public virtual Node Clone() {
        return Instantiate(this);
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}
