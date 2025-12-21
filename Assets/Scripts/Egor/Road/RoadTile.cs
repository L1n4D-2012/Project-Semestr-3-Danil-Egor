using System.Collections.Generic;
using UnityEngine;

public class RoadTile : MonoBehaviour
{
    [Header("Points")]
    public Transform[] spawnPoints;
    public Transform[] leftDecorPoints;
    public Transform[] rightDecorPoints;

    [Header("Obstacles")]
    public GameObject[] jumpableObstacles;
    public GameObject[] slidableObstacles;
    public GameObject[] impassableObstacles;

    [Header("Decor")]
    public GameObject[] decorPrefabs;

    [Header("Items")]
    public GameObject coinPrefab;
    public GameObject[] powerupPrefabs;

    [Header("Settings")]
    public float laneDistance = 2f;
    public int coinsPerLine = 8;
    public float coinSpacing = 2.0f;
    [Range(0f, 1f)] public float powerupSpawnChance = 0.05f;

    public float groundCoinHeight = 1f;
    public float airCoinHeight = 7f;

    public void SpawnObstacles(int count)
    {
        SpawnDecor();

        bool isFlying = PlayerJetpack.instance != null && PlayerJetpack.instance.isFlying;

        if (isFlying)
        {
            SpawnCoinSnakeInAir();
        }
        else
        {
            List<int> blockedLanes = new List<int>();
            SpawnBarriers(count, blockedLanes);
            SpawnStraightCoinsOnGround(blockedLanes);
            SpawnPowerups();
        }
    }

    void SpawnBarriers(int count, List<int> blockedLanes)
    {
        List<GameObject> allObstacles = new List<GameObject>();
        if (jumpableObstacles != null) allObstacles.AddRange(jumpableObstacles);
        if (slidableObstacles != null) allObstacles.AddRange(slidableObstacles);
        if (impassableObstacles != null) allObstacles.AddRange(impassableObstacles);

        if (spawnPoints.Length == 0 || allObstacles.Count == 0) return;

        List<Transform> availablePoints = new List<Transform>(spawnPoints);
        int obstaclesToSpawn = Mathf.Clamp(count, 0, availablePoints.Count - 1);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            if (availablePoints.Count == 0) break;

            int rnd = Random.Range(0, availablePoints.Count);
            Transform point = availablePoints[rnd];
            availablePoints.RemoveAt(rnd);

            int lane = GetLaneIndex(point.position);
            if (!blockedLanes.Contains(lane))
            {
                blockedLanes.Add(lane);
            }

            GameObject obsPrefab = allObstacles[Random.Range(0, allObstacles.Count)];
            GameObject obj = Instantiate(obsPrefab, point.position, obsPrefab.transform.rotation, transform);
            obj.transform.localScale = obsPrefab.transform.localScale;
        }
    }

    void SpawnStraightCoinsOnGround(List<int> blockedLanes)
    {
        if (coinPrefab == null) return;

        List<int> freeLanes = new List<int>();
        for (int i = -1; i <= 1; i++)
        {
            if (!blockedLanes.Contains(i)) freeLanes.Add(i);
        }

        if (freeLanes.Count > 0)
        {
            int chosenLane = freeLanes[Random.Range(0, freeLanes.Count)];
            float xPos = chosenLane * laneDistance;

            for (int i = 0; i < coinsPerLine; i++)
            {
                float zPos = 2f + (i * coinSpacing);
                Vector3 coinPos = transform.position + new Vector3(xPos, groundCoinHeight, zPos);

                GameObject coin = Instantiate(coinPrefab, coinPos, coinPrefab.transform.rotation, transform);
                coin.transform.localScale = coinPrefab.transform.localScale;
            }
        }
    }

    void SpawnCoinSnakeInAir()
    {
        if (coinPrefab == null) return;

        int startLane = Random.Range(-1, 2);
        int endLane = Random.Range(-1, 2);

        while (endLane == startLane) endLane = Random.Range(-1, 2);

        for (int i = 0; i < coinsPerLine; i++)
        {
            float t = (float)i / (float)(coinsPerLine - 1);
            float currentLaneX = Mathf.Lerp(startLane * laneDistance, endLane * laneDistance, t);

            float zPos = 5f + (i * coinSpacing);
            Vector3 coinPos = transform.position + new Vector3(currentLaneX, airCoinHeight, zPos);

            GameObject coin = Instantiate(coinPrefab, coinPos, coinPrefab.transform.rotation, transform);
            coin.transform.localScale = coinPrefab.transform.localScale;
        }
    }

    void SpawnPowerups()
    {
        if (powerupPrefabs.Length == 0) return;
        if (Random.value > powerupSpawnChance) return;

        List<Transform> freePoints = new List<Transform>(spawnPoints);
        if (freePoints.Count > 0)
        {
            Transform point = freePoints[Random.Range(0, freePoints.Count)];

            // ѕроверка, не зан€то ли место барьером (груба€)
            Collider[] hitColliders = Physics.OverlapSphere(point.position, 1f);
            bool isOccupied = false;
            foreach (var col in hitColliders)
            {
                if (col.gameObject.transform.parent == transform) isOccupied = true;
            }

            if (!isOccupied)
            {
                GameObject powerup = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
                Vector3 pos = point.position + Vector3.up * 1.5f;
                GameObject obj = Instantiate(powerup, pos, powerup.transform.rotation, transform);
                obj.transform.localScale = powerup.transform.localScale;
            }
        }
    }

    void SpawnDecor()
    {
        if (decorPrefabs.Length == 0) return;
        SpawnDecorAtPoints(leftDecorPoints);
        SpawnDecorAtPoints(rightDecorPoints);
    }

    void SpawnDecorAtPoints(Transform[] points)
    {
        foreach (var p in points)
        {
            if (Random.value > 0.4f)
            {
                GameObject decor = decorPrefabs[Random.Range(0, decorPrefabs.Length)];
                GameObject obj = Instantiate(decor, p.position, decor.transform.rotation, transform);
                obj.transform.localScale = decor.transform.localScale;
            }
        }
    }

    int GetLaneIndex(Vector3 position)
    {
        float localX = position.x - transform.position.x;
        if (localX < -1f) return -1;
        if (localX > 1f) return 1;
        return 0;
    }
}