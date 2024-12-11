using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class QTEWall : MonoBehaviour
{
    [SerializeField]
    ButtonPressQTEPuzzle buttonpressQTE;
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
        tilemapRenderer = gameObject.GetComponent<TilemapRenderer>();
        tilemapRenderer.enabled = false;
        visible = false;
        buttonpressQTE = GameObject.Find("ButtonPressQTEPuzzle").GetComponent<ButtonPressQTEPuzzle>();
        tilemapMaterial = tilemapRenderer.material;
        originalColor = tilemapMaterial.color;
        buttonpressQTE.BlinkWall -= StartBlinkAndPause;
        buttonpressQTE.BlinkWall += StartBlinkAndPause;
        
    }

    // Update is called once per frame
    void Update()
    {
        visible = buttonpressQTE.GetComponent<PuzzleBase>().Cleared;
        if (buttonpressQTE == null)
        {
            Debug.LogError("buttonpressQTE is null [QTEWall]");
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
}
