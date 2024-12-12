using UnityEngine;

public class SwapItem : ItemBase
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayerSwapManager.Instance.SwapFlag = true;
            gameObject.SetActive(false);
            SystemMessageManager.Instance.PushSystemMessage("�۸�ġ �������� ȹ���߽��ϴ�!", Color.green);
        }
        else if (InteractManager.Instance.CheckInteractable(this))
        {
            SystemMessageManager.Instance.PushSystemMessage("�۸�ġ ������ ȹ�� ('G')", Color.green);
        }
    }
}
