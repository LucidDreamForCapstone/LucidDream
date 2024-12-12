using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PritoQTEPuzzle : PuzzleBase, Interactable {
    [SerializeField] Canvas _keyboardCanvas;
    [SerializeField] Slider _progressSlider;
    [SerializeField] List<GameObject> _keyBoardObjList;
    [SerializeField] List<int> _buttonCountList;
    [SerializeField] float _plusAmount;
    [SerializeField] float _minusAmount;
    [SerializeField] float _eventDelay;
    [SerializeField] AudioClip _successSound;
    [SerializeField] AudioClip _failSound;
    [SerializeField] bool _interactable;

    public bool Interactable { get { return _interactable; } set { _interactable = value; } }
    //BoxCollider2D _triggerCollider;
    [SerializeField] public Action<float> BlinkWall;
    private bool _isPlayerConnected; //check if player2 collider is on the event trigger collider
    private bool _isEventOnProcess;
    private bool _isReady;
    private float _currentGauge;
    private Image _sliderFill;

    private void Start() {
        _currentGauge = 0;
        UpdateSlider();
        _isPlayerConnected = false;
        _isEventOnProcess = false;
        _isReady = true;
        _sliderFill = _progressSlider.fillRect.GetComponent<Image>();
        _progressSlider.gameObject.SetActive(false);
    }

    private void Update() {
        ButtonQTE().Forget();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !IsInteractBlock()) {
            Debug.Log("Prito QTE Zone Entered");
            _isPlayerConnected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("Prito QTE Zone Exited");
            _isPlayerConnected = false;
            _progressSlider.gameObject.SetActive(false);
        }
    }



    private async UniTaskVoid ButtonQTE() {
        if (!_isEventOnProcess && _isPlayerConnected && _isReady) {
            _isEventOnProcess = true;
            _isReady = false;
            _progressSlider.gameObject.SetActive(true);
            _sliderFill.color = Color.red;
            _currentGauge = 0;
            UpdateSlider();
            int i, patternCount = _buttonCountList.Count;
            for (i = 0; i < patternCount; i++) {
                bool isProblemDetected = await ButtonSequence(_buttonCountList[i]);
                if (isProblemDetected) {
                    break;
                }
                else {
                    IncreaseGauge(i).Forget();
                }
            }

            if (i == patternCount) { //Mission Complete
                Cleared = true;
                SoundManager.Instance.PlaySFX(_successSound.name, true);
                _sliderFill.color = Color.green;
                List<string> messages = new List<string>();
                messages.Add("좋아, 대충 두드렸더니 해결된 것 같군.\n 이제 다시 벽이 보일거야.");
                SystemMessageManager.Instance.ShowDialogBox("???", messages, 6).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(6), ignoreTimeScale: true);
                SystemMessageManager.Instance.PushSystemMessage("F를 눌러 주인공 시점으로 전환하여 통로를 탈출하세요.", Color.yellow, lastTime: 5);
                await UniTask.Delay(TimeSpan.FromSeconds(_eventDelay * 0.85));
                BlinkWall.Invoke(_eventDelay * 0.3f);
                await UniTask.Delay(TimeSpan.FromSeconds(_eventDelay * 0.15f));
                Cleared = false;
                _progressSlider.gameObject.SetActive(false);
                DecreaseGauge(false).Forget();
                _isReady = true;
            }
            else {
                DecreaseGauge().Forget();
                _isReady = true;
            }

            DestroyAllKeyObjects();
            _isEventOnProcess = false;
        }
    }

    private async UniTask<bool> ButtonSequence(int count) {
        List<PressKey> keys = GetRandomButtonList(count);
        List<KeyBoard> keyBoards = new List<KeyBoard>();
        List<Image> keyBoardImages = new List<Image>();
        int progressPointer = 0;
        bool canInteract = true;
        for (int j = 0; j < keys.Count; j++) {
            keyBoards.Add(Instantiate(_keyBoardObjList[(int)keys[j]], _keyboardCanvas.gameObject.transform).GetComponent<KeyBoard>());
            keyBoardImages.Add(keyBoards[j].GetComponent<Image>());
        }

        while (_isPlayerConnected && _isEventOnProcess && progressPointer < count && !PlayerSwapManager.Instance.CurrentPlayerIsOne()) {
            KeyCode targetKeycode = ParseKeyCode((int)keys[progressPointer]);
            if (Input.anyKeyDown) {
                if (canInteract && Input.GetKeyDown(targetKeycode)) { //Pressed Right Button
                    keyBoards[progressPointer].Pressed();
                    keyBoardImages[progressPointer].DOFade(0.4f, 0.5f).ToUniTask().Forget();
                    progressPointer++;
                }
                else if (!Input.GetKeyDown(targetKeycode)) { //Pressed Wrong Button
                    progressPointer = 0;
                    canInteract = false;
                    SoundManager.Instance.PlaySFX(_failSound.name, true);
                    keyBoardImages.ForEach(async image => {
                        await image.DOColor(Color.red, 0.2f);
                        await image.DOColor(Color.white, 0.2f);
                        await image.DOFade(1, 0.2f);
                    });
                    canInteract = true;
                }
            }
            await UniTask.NextFrame();
        }

        if (_isPlayerConnected && !PlayerSwapManager.Instance.CurrentPlayerIsOne()) {
            for (int j = 0; j < keyBoards.Count; j++) {
                keyBoards[j].gameObject.SetActive(false);
            }
            return false;
        }
        else {
            return true;
        }
    }

    private List<PressKey> GetRandomButtonList(int count) {
        List<PressKey> buttonList = new List<PressKey>();
        for (int i = 0; i < count; i++) {
            PressKey randomKey = (PressKey)Random.Range(0, (int)PressKey.Space + 1);
            buttonList.Add(randomKey);
        }
        return buttonList;
    }

    private KeyCode ParseKeyCode(int key) {
        KeyCode selectedKey = KeyCode.None;
        switch (key) {
            case 0:
                selectedKey = KeyCode.Q;
                break;
            case 1:
                selectedKey = KeyCode.W;
                break;
            case 2:
                selectedKey = KeyCode.E;
                break;
            case 3:
                selectedKey = KeyCode.R;
                break;
            case 4:
                selectedKey = KeyCode.Space;
                break;
        }
        return selectedKey;
    }

    private async UniTaskVoid IncreaseGauge(int index) {
        float targetGauge = (float)(index + 1) / _buttonCountList.Count * 100.0f;
        while (_currentGauge < targetGauge) {
            _currentGauge += _plusAmount * Time.deltaTime;
            UpdateSlider();
            await UniTask.NextFrame();
        }
        _currentGauge = targetGauge;
    }

    private async UniTaskVoid DecreaseGauge(bool sliderUpdateEnable = true) {
        while (_currentGauge > 0) {
            _currentGauge -= _minusAmount * Time.deltaTime;
            if (sliderUpdateEnable)
                UpdateSlider();
            await UniTask.NextFrame();
        }
        _currentGauge = 0;
    }

    private void DestroyAllKeyObjects() {
        foreach (Transform child in _keyboardCanvas.transform) {
            Destroy(child.gameObject);
        }
    }

    private void UpdateSlider() {
        _progressSlider.value = _currentGauge / 100;
    }

    public bool IsInteractBlock() {
        return !_interactable;
    }

    public string GetInteractText() {
        return "PritoPuzzle";
    }
}
