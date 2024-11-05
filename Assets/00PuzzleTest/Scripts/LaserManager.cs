using Puzzle;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    [SerializeField]
    private int poolSize = 20; // Adjust as needed
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private GameObject dispenser;

    private Queue<GameObject> laserPool = new Queue<GameObject>();
    private List<GameObject> activeLasers = new List<GameObject>();
    private Queue<GameObject> newLasers = new Queue<GameObject>();

    void Start()
    {
        // Initialize the pool with inactive laser objects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject laser = Instantiate(laserPrefab, this.transform);
            laser.SetActive(false);
            laserPool.Enqueue(laser);
        }
    }

    void Update()
    {
        UpdateLasers();
    }

    void SeparateLaser(List<Vector2> reflectDirections, Vector2 reflectorPosition, float magnitude)
    {
        foreach (Vector2 direction in reflectDirections)
        {
            GameObject laser = GetLaserFromPool();
            if (laser != null)
            {
                Vector2 laserStartPos = LaserUtil.ToBoundary(reflectorPosition, direction, magnitude);
                laser.GetComponent<LaserBeam>().InitLaser(laserStartPos, direction, SeparateLaser);
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
        ResetAllLasersToPool();
        GameObject initialLaser = GetLaserFromPool();
        if (initialLaser != null)
        {
            initialLaser.GetComponent<LaserBeam>().InitLaser(dispenser.transform.position, Vector2.up, SeparateLaser);
            activeLasers.Add(initialLaser);
        }
        newLasers.Enqueue(initialLaser);
        while (newLasers.Count > 0)
        {
            LaserBeam laser = newLasers.Dequeue().GetComponent<LaserBeam>();
            laser.UpdateRaser();
        }
    }
}
