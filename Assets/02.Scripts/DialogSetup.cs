using UnityEngine;
using System.Collections.Generic;

public class ExampleDialogSetup : MonoBehaviour
{
    public ShowDialogBoxAsset dialogBoxAsset;

    private void Start() {
        // 메시지와 폰트 크기 설정
        dialogBoxAsset.messages = new List<DialogMessage>
        {
            new DialogMessage("안녕하세요!", 40),
            new DialogMessage("이 메시지는 <b>굵게</b> 보입니다.", 50),
            new DialogMessage("작은 텍스트", 20)
        };

        dialogBoxAsset.name = "조작자";
        dialogBoxAsset.duration = 3.0f;
    }
}
