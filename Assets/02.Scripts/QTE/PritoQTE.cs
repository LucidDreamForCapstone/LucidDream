using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PritoQTE : MonoBehaviour { //Maplestory Prito Mini Game
    [SerializeField] Canvas _keyboardCanvas;
    [SerializeField] Slider _progressSlider;
    [SerializeField] List<GameObject> _keyBoardObjList;
    [SerializeField] List<int> _buttonCountList;
    [SerializeField] float _plusAmount;
    [SerializeField] float _minusAmount;
    [SerializeField] AudioClip _successSound;
    [SerializeField] AudioClip _failSound;

    //BoxCollider2D _triggerCollider;
    private bool _isPlayerConnected; //check if player2 collider is on the event trigger collider
    private bool _isEventOnProcess;
    private bool _isExplosionPatternActivated;
    private bool _isReady;
    private float _currentGauge;
    private Image _sliderFill;

    private void Start() {
        _currentGauge = 0;
        UpdateSlider();
        _isPlayerConnected = false;
        _isEventOnProcess = false;
        _isReady = true;
        _isExplosionPatternActivated = true; //temp : must be changed to false
        _sliderFill = _progressSlider.fillRect.GetComponent<Image>();
    }

    private void Update() {
        ButtonQTE().Forget();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("Prito QTE Zone Entered");
            _isPlayerConnected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("Prito QTE Zone Exited");
            _isPlayerConnected = false;
        }
    }


    private async UniTaskVoid ButtonQTE() {
        if (!_isEventOnProcess && _isPlayerConnected && _isReady && _isExplosionPatternActivated) {
            _isEventOnProcess = true;
            _isReady = false;
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
                SoundManager.Instance.PlaySFX(_successSound.name, true);
                _sliderFill.color = Color.green;
                //패턴 눈에 보이게 조치
                await UniTask.Delay(TimeSpan.FromSeconds(2));
                DecreaseGauge(false).Forget();
                MakeReadyWithDelay().Forget();
            }
            else {
                DecreaseGauge(true).Forget();
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

        while (_isPlayerConnected && _isEventOnProcess && _isExplosionPatternActivated && progressPointer < count) {
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

        if (_isPlayerConnected && _isExplosionPatternActivated) {
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

    private async UniTaskVoid MakeReadyWithDelay(float delay = 10) {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        _isReady = true;
        UpdateSlider();
    }

    private void DestroyAllKeyObjects() {
        foreach (Transform child in _keyboardCanvas.transform) {
            Destroy(child.gameObject);
        }
    }

    private void UpdateSlider() {
        _progressSlider.value = _currentGauge / 100;
    }

    public void SetExplosionState(bool state) {
        _isExplosionPatternActivated = state;
    }
}
