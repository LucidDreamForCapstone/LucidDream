using UnityEngine;

public class SwapItem : MonoBehaviour, Interactable {
    private bool _prevState = false;
    public string GetInteractText() {
        return "글리치 아이템 'G'";
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
                SystemMessageManager.Instance.PushSystemMessage("글리치 아이템을 획득했습니다!", Color.green);
            }
        }
        _prevState = isInteractable;
    }
}
