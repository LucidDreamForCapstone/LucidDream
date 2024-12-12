using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonPressQTEPuzzle : PuzzleBase, Interactable {
    [SerializeField] List<Charger> _chargers;
    [SerializeField] Slider _progressSlider;
    [SerializeField] List<KeyBoard> _keyBoards;
    [SerializeField] float _minusAmount;
    [SerializeField] float _plusAmount;
    [SerializeField] float _eventDelay;
    [SerializeField] AudioClip _successSound;
    [SerializeField] bool _interactable;
    PressKey randomKey;
    //[SerializeField] float _activeTime=_MaxTime;

    //private static float _MaxTime = 2.0f;
    public bool Interactable { get { return _interactable; } set { _interactable = value; } }
    //BoxCollider2D _triggerCollider;
    private bool _isPlayerConnected; //check if player2 collider is on the event trigger collider
    private bool _isEventOnProcess;
    private float _currentGauge;
    //private bool _isShieldDestroyReady; // for prevent sequential events when charger count is higher than 1 (Guarantee the interval between event and event)
    //private List<bool> _chargingStateList = new List<bool>();
    //private List<bool> _shieldStateList = new List<bool>();
    private Image _sliderFill;

    [SerializeField]
    public Action<float> BlinkWall;
    public float EventDelay { get { return _eventDelay; } }
    //public bool Cleared { get { return base.Cleared; } }
    private void Awake() {
        _currentGauge = 0;
        UpdateSlider();
        _keyBoards.ForEach(key => key.gameObject.SetActive(false));
        //_isShieldDestroyReady = true;
        _isPlayerConnected = false;
        _isEventOnProcess = false;
        _sliderFill = _progressSlider.fillRect.GetComponent<Image>();
        _progressSlider.gameObject.SetActive(false);
        //for (int i = 0; i < 3; i++)
        //{
        //    _chargingStateList.Add(false);
        //    _shieldStateList.Add(true);
        //}
    }

    private void Update() {
        GaugeDecrease();
        ButtonQTE().Forget();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !IsInteractBlock()) {
            Debug.Log("ButtonPress QTE Zone Entered");
            _isPlayerConnected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("ButtonPress QTE Zone Exited");
            _isPlayerConnected = false;
            _keyBoards[(int)randomKey].gameObject.SetActive(false);
            _progressSlider.gameObject.SetActive(false);
        }
    }


    public void GaugeDecrease() {
        if (_currentGauge > 0) {
            _currentGauge -= _minusAmount * Time.unscaledDeltaTime;
            UpdateSlider();
        }
        if (_currentGauge < 10) {
            Debug.Log("good");
        }
    }

    private async UniTaskVoid ButtonQTE() {
        if (!_isEventOnProcess && _isPlayerConnected) {
            _keyBoards.ForEach(key => key.gameObject.SetActive(false));
            _isEventOnProcess = true;
            _sliderFill.color = Color.red;
            _progressSlider.gameObject.SetActive(true);
            randomKey = (PressKey)Random.Range(0, (int)PressKey.Space + 1);
            Debug.Log(randomKey.ToString());
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
                Cleared = true;
                SoundManager.Instance.PlaySFX(_successSound.name, true);
                _currentGauge = 0;
                _sliderFill.color = Color.green;
                List<string> messages = new List<string>();
                messages.Add("좋아, 대충 두드렸더니 해결된 것 같군.\n 이제 다시 벽이 보일거야.");
                SystemMessageManager.Instance.ShowDialogBox("???", messages, 6).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(6), ignoreTimeScale: true);
                SystemMessageManager.Instance.PushSystemMessage("F를 눌러 주인공 시점으로 전환하여 통로를 탈출하세요.", Color.yellow, lastTime: 5);
                _keyBoards[(int)randomKey].gameObject.SetActive(false);
                await UniTask.Delay(TimeSpan.FromSeconds(_eventDelay * 0.85f));
                BlinkWall.Invoke(_eventDelay * 0.3f);
                await UniTask.Delay(TimeSpan.FromSeconds(_eventDelay * 0.15f));
                Cleared = false;
                UpdateSlider();
                _progressSlider.gameObject.SetActive(false);
                _isEventOnProcess = false;
            }
            else {
                _isEventOnProcess = false;
            }
        }
    }

    private void UpdateSlider() {
        _progressSlider.value = _currentGauge / 100;
    }

    public bool IsInteractBlock() {
        return !_interactable;
    }

    public string GetInteractText() {
        return "ButtonPuzzle";
    }
}
