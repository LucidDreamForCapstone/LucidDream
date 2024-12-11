using Puzzle;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : PuzzleBase
{
    [SerializeField]
    private int poolSize = 20; // Adjust as needed
    [SerializeField]
    private int initialTargetLaserCount = 3;
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private GameObject dispenser;
    [SerializeField]
    private List<Destination> destinations;
    [SerializeField]
    private float maxLaserLength = 10;
    [SerializeField]
    DoorControl doorController;





    private Queue<GameObject> laserPool = new Queue<GameObject>();
    private List<GameObject> activeLasers = new List<GameObject>();
    private Queue<GameObject> newLasers = new Queue<GameObject>();
    private List<ReflectorInformation> reflectors = new List<ReflectorInformation>();

    void Start()
    {
        // Initialize the pool with inactive laser objects
        ReflectorInformation[] infos = GetComponentsInChildren<ReflectorInformation>();
        foreach (ReflectorInformation info in infos)
        {
            reflectors.Add(info);
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject laser = Instantiate(laserPrefab, this.transform);
            laser.SetActive(false);
            laserPool.Enqueue(laser);
        }
        Debug.Log("LaserManager Initialized------!!!");
    }

    void Update()
    {
        UpdateLasers();
        cleared = isGameClear();
        doorController.DoorOpenOnGameClear(cleared);
        ResetDestinations();
    }

    void SeparateLaser(List<Vector2> reflectDirections, Vector2 reflectorPosition, float magnitude)
    {
        foreach (Vector2 direction in reflectDirections)
        {
            GameObject laser = GetLaserFromPool();
            if (laser != null)
            {
                Vector2 laserStartPos = LaserUtil.ToBoundary(reflectorPosition, direction, magnitude);
                laser.GetComponent<LaserBeam>().InitLaser(laserStartPos, direction, SeparateLaser, maxLaserLength);
                activeLasers.Add(laser);
                newLasers.Enqueue(laser);
            }
        }
    }

    private GameObject GetLaserFromPool()
    {
        if (laserPool.Count > 0)
        {
            GameObject laser = laserPool.Dequeue();
            laser.SetActive(true);
            return laser;
        }
        else
        {
            Debug.LogWarning("Laser pool exhausted!");
            return null;
        }
    }

    public void ReturnLaserToPool(GameObject laser)
    {
        laser.SetActive(false);
        laserPool.Enqueue(laser);
        activeLasers.Remove(laser);
    }

    public void ResetAllLasersToPool()
    {
        List<GameObject> lasersToReturn = new List<GameObject>();

        // activeLasers 리스트를 순회하면서 반환할 레이저를 lasersToReturn에 추가
        foreach (GameObject laser in activeLasers)
        {
            lasersToReturn.Add(laser);
        }

        // 순회가 끝난 후에 lasersToReturn에 있는 레이저들을 풀에 반환
        foreach (GameObject laser in lasersToReturn)
        {
            ReturnLaserToPool(laser);
        }
        activeLasers.Clear();
    }

    public void UpdateLasers()
    {
        for (int i = 0; i < reflectors.Count; i++)
        {
            reflectors[i].IsUsed = false;
        }
        // Debug.Log(activeLasers.Count);
        ResetAllLasersToPool();
        GameObject initialLaser = GetLaserFromPool();
        if (initialLaser != null)
        {
            initialLaser.GetComponent<LaserBeam>().InitLaser(dispenser.transform.position, Vector2.up, SeparateLaser, maxLaserLength);
            activeLasers.Add(initialLaser);
        }
        newLasers.Enqueue(initialLaser);
        while (newLasers.Count > 0)
        {
            LaserBeam laser = newLasers.Dequeue().GetComponent<LaserBeam>();
            laser.UpdateRaser();
        }
        // Debug.Log(destination.DestCount);
    }

    public bool isGameClear()
    {
        for (int i = 0; i < destinations.Count; i++)
        {
            // Debug.Log(i + "th destCnt is " + destinations[i].DestCount);
        }
        for (int i = 0; i < destinations.Count; i++)
        {
            //Debug.Log(i + "th destCnt is " + destinations[i].DestCount);
            //Debug.Log(dest.DestCount);
            if (destinations[i].DestCount >= 1)
            {
                return false;
            }
        }
        return true;
    }

    public void ResetDestinations()
    {
        foreach (Destination dest in destinations)
        {
            dest.DestCount = dest.TargetCount1;
        }
    }
}
