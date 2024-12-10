using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class PlayerRevival : MonoBehaviour {
    [SerializeField] int _revivalCount;
    [SerializeField] GameObject _revivalPanel;
    [SerializeField] TextMeshProUGUI _timerTM;
    [SerializeField] TextMeshProUGUI _countTM;
    [SerializeField] GameObject _guardEffect;
    bool _isUsing = false;


    public bool CheckRevivalAvailable() {
        return _revivalCount > 0;
    }

    public void OpenRevivalPanel() {
        if (!_isUsing) {
            _isUsing = true;
            _revivalPanel.SetActive(true);
            TimeScaleManager.Instance.TimeStop();
            UpdateRevivalCountUI();
            Countdown().Forget();
        }
    }

    public void PlayerRevive() {
        if (_revivalCount > 0) {
            _revivalCount--;
            Player player = PlayerDataManager.Instance.Player;
            player.CustomInvincible(3).Forget();
            PlayerDataManager.Instance.HealByMaxPercent(100);
            Vector3 effectPosition = player.transform.position + new Vector3(0, 0.7f, 0);
            GameObject effectInstance = Instantiate(_guardEffect, effectPosition, Quaternion.identity);
            effectInstance.transform.SetParent(player.transform);
            CloseRevivalPanel();
        }
        else {
            Debug.Log("Something Wrong. Check the revival button logic");
        }
    }

    public async UniTaskVoid Countdown() {
        int i;
        for (i = 10; i > 0; i--) {
            _timerTM.text = $"<color=red>{i}</color>초 후 자동으로 부활";
            await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: true);
            if (!_revivalPanel.activeSelf)
                break;
        }
        if (_revivalPanel.activeSelf)
            PlayerRevive();
    }
    private void UpdateRevivalCountUI() {
        _countTM.text = $"남은 부활 횟수\n<color=red><size=180>{_revivalCount}</color></size>";
    }

    private void CloseRevivalPanel() {
        _revivalPanel.SetActive(false);
        TimeScaleManager.Instance.TimeRestore();
        _isUsing = false;
    }
}
