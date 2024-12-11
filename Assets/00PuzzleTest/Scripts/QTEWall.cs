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
    public float blinkDuration = 0.1f; // �����̴� �ð� ����
    [SerializeField]
    private Material tilemapMaterial; // TilemapRenderer�� Material
    [SerializeField]
    private Color originalColor; // TilemapRenderer�� ���� ����
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
            // ������: ������ ����
            tilemapMaterial.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(blinkDuration);

            // ���� �������� ����
            tilemapMaterial.color = originalColor;
            yield return new WaitForSeconds(blinkDuration);

            // ��� �ð� ���
            float elapTime = Time.time - startTime;
            if (elapTime >= totalBlinkingTime)
                break;
        }


        // �������� ���� �� ���� (�ڵ� ���� ����)
        Debug.Log("Blinking finished. Resuming code.");
    }
}
