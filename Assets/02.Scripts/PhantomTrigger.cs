using UnityEngine;

public class PhantomTrigger : MonoBehaviour {
    public static PhantomTrigger Instance { get; private set; }

    private bool _canUsePhantom = false; // 팬텀 사용 가능 여부

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public bool CanUsePhantom => _canUsePhantom; // 팬텀 사용 가능 상태 반환

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) // 트리거의 태그 확인
        {
            _canUsePhantom = true; // 팬텀 사용 가능 상태로 설정
            Debug.Log("팬텀 사용 가능 상태가 활성화되었습니다.");
        }
    }
}
