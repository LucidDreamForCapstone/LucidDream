using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour {
    public BehaviourTree _tree;

    private void Start() {
        _tree = _tree.Clone();
    }

    private void Update() {
        _tree.Update();
    }
}
