using UnityEngine;

public class Destination : MonoBehaviour
{
    [SerializeField]
    private int TargetCount = 0;
    [SerializeField]
    private int destCount;

    public int DestCount { get => destCount; set => destCount = value; }
    public int TargetCount1 { get => TargetCount; }
    void Start()
    {
        destCount = TargetCount;
    }

    void Update()
    {

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SystemMessageManager.Instance.PushSystemMessage(destCount.ToString(), Color.red, lastTime: 2);
        }
    }
}
