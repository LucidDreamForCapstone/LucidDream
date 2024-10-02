using UnityEngine;

public abstract class Node : ScriptableObject {
    public enum State {
        Running,
        Failure,
        Success
    }

    public State _state = State.Running;
    public bool _started = false;
    public string _guid;
    public Vector2 _position;

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

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}
