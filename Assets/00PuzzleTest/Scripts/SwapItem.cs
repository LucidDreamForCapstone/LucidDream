using UnityEngine;

public class SwapItem : MonoBehaviour, Interactable {
    private bool _prevState = false;
    public string GetInteractText() {
        return "�۸�ġ ������ 'G'";
    }

    public bool IsInteractBlock() {
        return false;
    }

    private void Update() {
        bool isInteractable = InteractManager.Instance.CheckInteractable(this);
        if (isInteractable) {
            if (_prevState != isInteractable)
                SystemMessageManager.Instance.PushSystemMessage(GetInteractText(), Color.blue, lastTime: 4);
            if (Input.GetKeyDown(KeyCode.G)) {
                PlayerSwapManager.Instance.SwapFlag = true;
                gameObject.SetActive(false);
                SystemMessageManager.Instance.PushSystemMessage("�۸�ġ �������� ȹ���߽��ϴ�!", Color.green);
            }
        }
        _prevState = isInteractable;
    }
}
