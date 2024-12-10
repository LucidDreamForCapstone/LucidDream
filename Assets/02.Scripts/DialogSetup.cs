using UnityEngine;
using System.Collections.Generic;

public class ExampleDialogSetup : MonoBehaviour
{
    public ShowDialogBoxAsset dialogBoxAsset;

    private void Start() {
        // �޽����� ��Ʈ ũ�� ����
        dialogBoxAsset.messages = new List<DialogMessage>
        {
            new DialogMessage("�ȳ��ϼ���!", 40),
            new DialogMessage("�� �޽����� <b>����</b> ���Դϴ�.", 50),
            new DialogMessage("���� �ؽ�Ʈ", 20)
        };

        dialogBoxAsset.name = "������";
        dialogBoxAsset.duration = 3.0f;
    }
}
