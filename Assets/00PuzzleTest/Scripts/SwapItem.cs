using UnityEngine;

public class SwapItem : ItemBase
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayerSwapManager.Instance.SwapFlag = true;
            gameObject.SetActive(false);
            SystemMessageManager.Instance.PushSystemMessage("글리치 아이템을 획득했습니다!", Color.green);
        }
        else if (InteractManager.Instance.CheckInteractable(this))
        {
            SystemMessageManager.Instance.PushSystemMessage("글리치 아이템 획득 ('G')", Color.green);
        }
    }
}
