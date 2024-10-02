using UnityEngine;

public class DebugLogNode : ActionNode {
    public string _message;

    protected override void OnStart() {
        Debug.Log($"OnStart{_message}");
    }
    protected override void OnStop() {
        Debug.Log($"OnStop{_message}");
    }
    protected override State OnUpdate() {
        Debug.Log($"OnUpdate{_message}");
        return State.Success;
    }
}
