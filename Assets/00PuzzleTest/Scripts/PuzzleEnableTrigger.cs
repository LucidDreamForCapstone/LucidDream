using UnityEngine;

public class PuzzleEnableTrigger : MonoBehaviour {
    [SerializeField]
    QTEWall QteWall;
    [SerializeField]
    SpriteRenderer pritoRenderer;
    [SerializeField]
    SpriteRenderer buttonpressRenderer;

    void Start() {
        pritoRenderer = QteWall.pritoQTEPuzzle.gameObject.GetComponent<SpriteRenderer>();
        buttonpressRenderer = QteWall.buttonpressQTE.gameObject.GetComponent<SpriteRenderer>();
    }

    void Update() {

    }



    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            PlayerSwapManager.Instance.SetInPuzzleQTE(true);
            switch (QteWall.puzzleType) {
                case 0:
                    QteWall.buttonpressQTE.Interactable = true;
                    QteWall.MakeTransParent(buttonpressRenderer, 1.0f);
                    //Debug.Log($"puzzleType{QteWall.puzzleType}");
                    break;
                case 1:
                    QteWall.pritoQTEPuzzle.Interactable = true;
                    QteWall.MakeTransParent(pritoRenderer, 1.0f);
                    //Debug.Log($"puzzleType{QteWall.puzzleType}");
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            PlayerSwapManager.Instance.SetInPuzzleQTE(false);
            QteWall.buttonpressQTE.Interactable = false;
            QteWall.pritoQTEPuzzle.Interactable = false;
            QteWall.MakeTransParent(pritoRenderer, 0.1f);
            QteWall.MakeTransParent(buttonpressRenderer, 0.1f);
        }
    }
}
