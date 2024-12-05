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
}
