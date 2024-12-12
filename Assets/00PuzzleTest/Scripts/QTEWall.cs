using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class QTEWall : MonoBehaviour
{
    [SerializeField]
    ButtonPressQTEPuzzle buttonpressQTE;
    [SerializeField]
    PritoQTEPuzzle pritoQTEPuzzle;
    [SerializeField]
    PuzzleBase targetPuzzle;
    [SerializeField]
    bool visible = false;
    [SerializeField]
    TilemapRenderer tilemapRenderer;


    [SerializeField]
    public float blinkDuration = 0.1f; // 깜빡이는 시간 간격
    [SerializeField]
    private Material tilemapMaterial; // TilemapRenderer의 Material
    [SerializeField]
    private Color originalColor; // TilemapRenderer의 원래 색상
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        tilemapRenderer = gameObject.GetComponent<TilemapRenderer>();
        tilemapRenderer.enabled = false;
        visible = false;

        buttonpressQTE = GameObject.Find("ButtonPressQTEPuzzle").GetComponent<ButtonPressQTEPuzzle>();
        pritoQTEPuzzle = GameObject.Find("PritoQTEPuzzle").GetComponent<PritoQTEPuzzle>();
        SpriteRenderer pritoRenderer = pritoQTEPuzzle.gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer buttonpressRenderer = buttonpressQTE.gameObject.GetComponent<SpriteRenderer>();
        MakeTransParent(pritoRenderer, 1.0f);
        MakeTransParent(buttonpressRenderer, 1.0f);
        int type = Random.Range(0, 2);
        Debug.Log("type is : " + type);
        switch (type)
        {
            case 0:
                targetPuzzle = buttonpressQTE;
                buttonpressQTE.Interactable = true;
                MakeTransParent(pritoRenderer, 0.3f);
                pritoQTEPuzzle.Interactable = false;
                break;
            case 1:
                targetPuzzle = pritoQTEPuzzle;
                pritoQTEPuzzle.Interactable = true;
                MakeTransParent(buttonpressRenderer, 0.3f);
                buttonpressQTE.Interactable = false;
                break;
        }
        tilemapMaterial = tilemapRenderer.material;
        originalColor = tilemapMaterial.color;
        pritoQTEPuzzle.BlinkWall = null;
        pritoQTEPuzzle.BlinkWall += StartBlinkAndPause;
        buttonpressQTE.BlinkWall = null;
        buttonpressQTE.BlinkWall += StartBlinkAndPause;

    }

    // Update is called once per frame
    void Update()
    {
        visible = targetPuzzle.Cleared;
        if (targetPuzzle == null)
        {
            Debug.LogError("targetPuzzle is null [QTEWall]");
            return;
        }
        tilemapRenderer.enabled = visible;


    }

    public void StartBlinkAndPause(float totalBlinkingTime)
    {
        StartCoroutine(BlinkAndPauseCoroutine(totalBlinkingTime));
    }

    private IEnumerator BlinkAndPauseCoroutine(float t)
    {
        float totalBlinkingTime = t;
        float startTime = Time.time;

        while (true)
        {
            Debug.Log("Blink!");
            // 깜빡임: 색상을 변경
            tilemapMaterial.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(blinkDuration);

            // 원래 색상으로 복원
            tilemapMaterial.color = originalColor;
            yield return new WaitForSeconds(blinkDuration);

            // 경과 시간 계산
            float elapTime = Time.time - startTime;
            if (elapTime >= totalBlinkingTime)
                break;
        }


        // 깜빡임이 끝난 후 동작 (코드 진행 가능)
        Debug.Log("Blinking finished. Resuming code.");
    }

    public void MakeTransParent(SpriteRenderer spriteRenderer, float amount)
    {
        Color color = spriteRenderer.color;
        color.a = amount;
        spriteRenderer.color = color;
    }
}
