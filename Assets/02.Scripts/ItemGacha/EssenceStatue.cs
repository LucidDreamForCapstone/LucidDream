using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class EssenceStatue : DropableBase {
    [SerializeField] GameObject _textObj;
    [SerializeField] GameObject _dropPoint;
    [SerializeField] int _cost;
    [SerializeField] string _message;
    [SerializeField] float _layerBorder;
    Animator _animator;
    SpriteRenderer _sr;
    bool _isReady;
    //0.48 0.84 0.96 0.9994
    private void Start() {
        _isReady = true;
        _animator = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _textObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            _textObj.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Pray();
            ChangeStatueSortingLayer(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            _textObj.SetActive(false);
        }
    }

    private void Pray() {
        if (Input.GetKey(KeyCode.G)) {
            int dreamFragCount = PlayerDataManager.Instance.Status._dream;
            if (dreamFragCount >= _cost) {
                if (_isReady) {
                    PlayerDataManager.Instance.SetDream(dreamFragCount - _cost);
                    PrayTask().Forget();
                }
            }
            else {
                SystemMessageManager.Instance.PushSystemMessage(_message, Color.red);
            }
        }
    }

    private async UniTaskVoid PrayTask() {
        _isReady = false;
        _animator.SetTrigger("Activate");
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        DropOneItem(_dropPoint);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        _animator.SetTrigger("Deactivate");
        _isReady = true;
    }

    private void ChangeStatueSortingLayer(Collider2D collision) {
        if (collision.transform.position.y - transform.position.y > _layerBorder)
            _sr.sortingOrder = 8;
        else
            _sr.sortingOrder = 4;
    }
}
