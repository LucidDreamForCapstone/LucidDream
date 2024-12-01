using UnityEngine;

public class KeyBoard : MonoBehaviour {
    Animator _animator;
    private void Start() {
        _animator = GetComponent<Animator>();
    }
    public void Pressed() {
        _animator.SetTrigger("Pressed");
    }
}
