using System.Collections.Generic;
using UnityEngine;

public class RoadTile : MonoBehaviour
{
    public static int lastJetpackLane = 0;

    [Header("Points")]
    public Transform[] spawnPoints;

    [Header("Obstacles")]
    public GameObject[] jumpableObstacles;
    public GameObject[] slidableObstacles;
    public GameObject[] impassableObstacles;

    [Header("Items")]
    public GameObject coinPrefab;
    public GameObject[] powerupPrefabs;

    [Header("Settings")]
    public float laneDistance = 2f;
    public int coinsPerLine = 20;
    public float coinSpacing = 1.5f;
    public float groundCoinHeight = 1f;
    public float airCoinHeight = 7f;

    private const int AIR_COIN_COUNT = 40;
    private const float TILE_LENGTH = 60f;
    private const float POWERUP_CHANCE = 0.2f;
    private const float POWERUP_SPAWN_CHANCE = 0.15f;
    private const float TURN_CHANCE = 0.5f;

    private readonly List<GameObject> spawnedObjects = new List<GameObject>();
    private List<GameObject> cachedObstacles;

    public void SpawnObstacles(bool isFlying, bool isSafeZone = false)
    {
        ClearOldObjects();

        if (isSafeZone) return;

        if (isFlying)
        {
            SpawnSmartCoinPath();
        }
        else
        {
            var blockedLanes = new List<int>();
            SpawnBarriers(blockedLanes);
            SpawnStraightCoinsOnGround(blockedLanes);
            SpawnPowerups(blockedLanes);
        }
    }

    void ClearOldObjects()
    {
        foreach (var obj in spawnedObjects)
            if (obj != null) Destroy(obj);

        spawnedObjects.Clear();
    }

    void SpawnSmartCoinPath()
    {
        if (coinPrefab == null) return;

        int startLane = lastJetpackLane;
        int targetLane = startLane;

        if (Random.value > TURN_CHANCE)
            targetLane = (startLane == 0) ? (Random.value > 0.5f ? 1 : -1) : 0;

        float step = TILE_LENGTH / AIR_COIN_COUNT;

        for (int i = 0; i < AIR_COIN_COUNT; i++)
        {
            float t = (float)i / (AIR_COIN_COUNT - 1);
            float x = Mathf.Lerp(startLane * laneDistance, targetLane * laneDistance, t);
            float z = -30f + (i * step);

            Vector3 pos = transform.position + new Vector3(x, airCoinHeight, z);
            bool isLast = i == AIR_COIN_COUNT - 1;

            if (isLast && Random.value < POWERUP_CHANCE && powerupPrefabs.Length > 0)
                SpawnPrefab(powerupPrefabs[Random.Range(0, powerupPrefabs.Length)], pos);
            else
                SpawnPrefab(coinPrefab, pos);
        }

        lastJetpackLane = targetLane;
    }

    void SpawnBarriers(List<int> blockedLanes)
    {
        if (spawnPoints.Length == 0) return;

        if (cachedObstacles == null)
        {
            cachedObstacles = new List<GameObject>();
            if (jumpableObstacles != null) cachedObstacles.AddRange(jumpableObstacles);
            if (slidableObstacles != null) cachedObstacles.AddRange(slidableObstacles);
            if (impassableObstacles != null) cachedObstacles.AddRange(impassableObstacles);
        }

        if (cachedObstacles.Count == 0) return;

        var available = new List<Transform>(spawnPoints);
        int count = Random.Range(1, 3);

        for (int i = 0; i < count && available.Count > 0; i++)
        {
            int rnd = Random.Range(0, available.Count);
            Transform point = available[rnd];
            available.RemoveAt(rnd);

            int lane = GetLaneIndex(point.position);
            if (blockedLanes.Contains(lane)) continue;

            blockedLanes.Add(lane);
            SpawnPrefab(cachedObstacles[Random.Range(0, cachedObstacles.Count)], point.position);
        }
    }

    void SpawnStraightCoinsOnGround(List<int> blockedLanes)
    {
        if (coinPrefab == null) return;

        int chosenLane = GetRandomFreeLane(blockedLanes);
        if (chosenLane == int.MinValue) return;

        float x = chosenLane * laneDistance;

        // Центрируем линию монет по тайлу автоматически
        float totalLength = (coinsPerLine - 1) * coinSpacing;
        float zStart = -totalLength / 2f;

        for (int i = 0; i < coinsPerLine; i++)
        {
            float z = zStart + (i * coinSpacing);
            SpawnPrefab(coinPrefab, transform.position + new Vector3(x, groundCoinHeight, z));
        }
    }
    void SpawnPowerups(List<int> blockedLanes)
    {
        if (powerupPrefabs.Length == 0 || Random.value > POWERUP_SPAWN_CHANCE) return;

        int lane = GetRandomFreeLane(blockedLanes);
        if (lane == int.MinValue) return;

        Vector3 pos = transform.position + new Vector3(lane * laneDistance, 1.5f, 0);
        SpawnPrefab(powerupPrefabs[Random.Range(0, powerupPrefabs.Length)], pos);
    }

    void SpawnPrefab(GameObject prefab, Vector3 position)
    {
        GameObject obj = Instantiate(prefab, position, prefab.transform.rotation, transform);
        obj.transform.localScale = prefab.transform.localScale;
        spawnedObjects.Add(obj);
    }

    int GetRandomFreeLane(List<int> blockedLanes)
    {
        var free = new List<int>();
        for (int i = -1; i <= 1; i++)
            if (!blockedLanes.Contains(i)) free.Add(i);

        return free.Count > 0 ? free[Random.Range(0, free.Count)] : int.MinValue;
    }

    int GetLaneIndex(Vector3 position)
    {
        float localX = position.x - transform.position.x;
        float threshold = laneDistance / 2f;
        if (localX < -threshold) return -1;
        if (localX > threshold) return 1;
        return 0;
    }
}