using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonPressQTE : MonoBehaviour {

    [SerializeField] BossBondrewd _boss;
    [SerializeField] List<Charger> _chargers;
    [SerializeField] Slider _progressSlider;
    [SerializeField] List<KeyBoard> _keyBoards;
    [SerializeField] float _minusAmount;
    [SerializeField] float _plusAmount;
    [SerializeField] float _eventDelay;
    [SerializeField] AudioClip _successSound;

    //BoxCollider2D _triggerCollider;
    private bool _isPlayerConnected; //check if player2 collider is on the event trigger collider
    private bool _isShieldDestroyReady; // for prevent sequential events when charger count is higher than 1 (Guarantee the interval between event and event)
    private bool _isEventOnProcess;
    private float _currentGauge;
    private List<bool> _chargingStateList = new List<bool>();
    private List<bool> _shieldStateList = new List<bool>();
    private Image _sliderFill;

    private void Awake() {
        _currentGauge = 0;
        UpdateSlider();
        _keyBoards.ForEach(key => key.gameObject.SetActive(false));
        _isShieldDestroyReady = true;
        _isPlayerConnected = false;
        _isEventOnProcess = false;
        _sliderFill = _progressSlider.fillRect.GetComponent<Image>();
        _progressSlider.gameObject.SetActive(false);
        for (int i = 0; i < 3; i++) {
            _chargingStateList.Add(false);
            _shieldStateList.Add(true);
        }
    }

    private void Update() {
        GaugeDecrease();
        ButtonQTE().Forget();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("ButtonPress QTE Zone Entered");
            _isPlayerConnected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("ButtonPress QTE Zone Exited");
            _isPlayerConnected = false;
        }
    }


    public void GaugeDecrease() {
        if (_isShieldDestroyReady && _currentGauge > 0) {
            _currentGauge -= _minusAmount * Time.unscaledDeltaTime;
            UpdateSlider();
        }
    }

    private async UniTaskVoid ButtonQTE() {
        if (!_isEventOnProcess && _isShieldDestroyReady && _isPlayerConnected && CheckTargetExist()) {
            _keyBoards.ForEach(key => key.gameObject.SetActive(false));
            _isEventOnProcess = true;
            _sliderFill.color = Color.red;
            _progressSlider.gameObject.SetActive(true);
            PressKey randomKey = (PressKey)Random.Range(0, (int)PressKey.Space + 1);
            KeyCode selectedKey = KeyCode.None;
            switch (randomKey) {
                case PressKey.Q:
                    selectedKey = KeyCode.Q;
                    break;
                case PressKey.W:
                    selectedKey = KeyCode.W;
                    break;
                case PressKey.E:
                    selectedKey = KeyCode.E;
                    break;
                case PressKey.R:
                    selectedKey = KeyCode.R;
                    break;
                case PressKey.Space:
                    selectedKey = KeyCode.Space;
                    break;
            }
            _keyBoards[(int)randomKey].gameObject.SetActive(true);

            while (_currentGauge < 100 && _isPlayerConnected) {
                if (Input.GetKeyDown(selectedKey)) {
                    _currentGauge += _plusAmount;
                    _keyBoards[(int)randomKey].Pressed();
                }
                await UniTask.NextFrame();
            }

            if (_currentGauge >= 100) { //Mission Complete
                SoundManager.Instance.PlaySFX(_successSound.name, true);
                SelectedChargerShieldRemove();
                _isShieldDestroyReady = false;
                _currentGauge = 0;
                _sliderFill.color = Color.green;
                _keyBoards[(int)randomKey].gameObject.SetActive(false);
                await UniTask.Delay(TimeSpan.FromSeconds(_eventDelay));
                _isShieldDestroyReady = true;
                UpdateSlider();
                _progressSlider.gameObject.SetActive(false);
                _isEventOnProcess = false;
            }
            else {
                _isEventOnProcess = false;
            }
        }
    }

    public void UpdateShieldState(int index, bool state) {
        _shieldStateList[index] = state;
    }

    public void UpdateChargingState(int index, bool state) {
        _chargingStateList[index] = state;
    }

    private void SelectedChargerShieldRemove() {
        for (int i = 0; i < 3; i++) {
            if (_chargingStateList[i] && _shieldStateList[i]) {
                _chargers[i].CallBackRemoveShield();
                return;
            }
        }
    }

    private bool CheckTargetExist() {
        for (int i = 0; i < 3; i++) {
            if (_chargingStateList[i] && _shieldStateList[i])
                return true;
        }
        return false;
    }

    private void UpdateSlider() {
        _progressSlider.value = _currentGauge / 100;
    }
}
