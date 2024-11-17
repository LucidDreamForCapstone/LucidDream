using UnityEngine;
using TMPro;

public class TextBlink : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro; // TextMeshPro 컴포넌트 참조
    [SerializeField] private Color color1 = new Color(1f, 1f, 1f, 1f); // 기본 색상 (255, 255, 255)
    [SerializeField] private Color color2 = new Color(240 / 255f, 1f, 0f, 1f); //바뀔 색상
    [SerializeField] private float blinkSpeed = 1f; // 깜빡이는 속도

    private bool isBlinking = true; // 깜빡임 상태를 관리

    private void Start() {
        if (textMeshPro == null) {
            textMeshPro = GetComponent<TextMeshPro>(); // 자동으로 TextMeshPro 컴포넌트 찾기
        }

        if (textMeshPro == null) {
            Debug.LogError("TextMeshPro component not found!");
            return;
        }

        StartBlinking();
    }

    private void Update() {
        if (isBlinking) {
            float t = Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1f); // 0~1 사이의 값을 반복
            textMeshPro.color = Color.Lerp(color1, color2, t); // 두 색상 사이를 전환
        }
    }

    public void StartBlinking() {
        isBlinking = true;
    }

    public void StopBlinking() {
        isBlinking = false;
        textMeshPro.color = color1; // 기본 색상으로 초기화
    }
}
