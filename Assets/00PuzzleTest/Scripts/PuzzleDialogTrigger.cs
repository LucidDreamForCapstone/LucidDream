using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDialogTrigger : MonoBehaviour {
    private static bool _isUsed;
    private List<string> _messages = new List<string>();

    private void Start() {
        _isUsed = false;
        _messages.Add("���������� ��ġ�� �񰡽ü� �庮�ΰ�...");
        _messages.Add("���� <color=red><size=50>�̻��� ��ġ �� ��</color></size> �߿�\n�ϳ��� �ǵ帮��\n��� �� �ذ�� �� ������\n���ݸ� ��ٷ���");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!_isUsed) {
            _isUsed = true;
            ShowPuzzleDialog().Forget();
        }
    }

    private async UniTaskVoid ShowPuzzleDialog() {
        SystemMessageManager.Instance.ShowDialogBox("???", _messages, 3).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(6.6));
        SystemMessageManager.Instance.PushSystemMessage("F�� ���� ������ �������� ��ȯ�ϸ鼭 ���� ã�� �����ϼ���.", Color.yellow, lastTime: 3);
    }
}
