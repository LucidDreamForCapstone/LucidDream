using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDialogTrigger : MonoBehaviour {
    private static bool _isUsed;
    private List<string> _messages = new List<string>();

    private void Start() {
        _isUsed = false;
        _messages.Add("연구소장이 설치한 비가시성 장벽인가...");
        _messages.Add("여기 <color=red><size=50>이상한 장치 두 개</color></size> 중에\n하나를 건드리면\n어떻게 잘 해결될 것 같으니\n조금만 기다려봐");
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
        SystemMessageManager.Instance.PushSystemMessage("F를 눌러 연구원 시점으로 전환하면서 길을 찾아 돌파하세요.", Color.yellow, lastTime: 3);
    }
}
