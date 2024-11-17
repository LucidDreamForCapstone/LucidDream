using UnityEngine;
using TMPro;

public class UIBlink : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProGUI; // TextMeshPro ������Ʈ ����
    [SerializeField] private Color color1 = new Color(1f, 1f, 1f, 1f); // �⺻ ���� (255, 255, 255)
    [SerializeField] private Color color2 = new Color(240 / 255f, 1f, 0f, 1f); //�ٲ� ����
    [SerializeField] private float blinkSpeed = 1f; // �����̴� �ӵ�

    private bool isBlinking = true; // ������ ���¸� ����

    private void Start() {
        if (textMeshProGUI == null) {
            textMeshProGUI = GetComponent<TextMeshProUGUI>(); // �ڵ����� TextMeshPro ������Ʈ ã��
        }

        if (textMeshProGUI == null) {
            Debug.LogError("TextMeshPro component not found!");
            return;
        }

        StartBlinking();
    }

    private void Update() {
        if (isBlinking) {
            float t = Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1f); // 0~1 ������ ���� �ݺ�
            textMeshProGUI.color = Color.Lerp(color1, color2, t); // �� ���� ���̸� ��ȯ
        }
    }

    public void StartBlinking() {
        isBlinking = true;
    }

    public void StopBlinking() {
        isBlinking = false;
        textMeshProGUI.color = color1; // �⺻ �������� �ʱ�ȭ
    }
}
