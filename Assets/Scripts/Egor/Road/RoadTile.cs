using System.Collections.Generic;
using UnityEngine;

public class RoadTile : MonoBehaviour
{
    // --- —“ј“»„≈— јя ѕјћя“№ (ќбща€ дл€ всех кусков дороги) ---
    // 0 = ÷ентр, -1 = Ћево, 1 = ѕраво
    public static int lastJetpackLane = 0;
    // ---------------------------------------------------------

    [Header("Points")]
    public Transform[] spawnPoints;
    public Transform[] leftHousePoints;
    public Transform[] rightHousePoints;

    [Header("Obstacles")]
    public GameObject[] jumpableObstacles;
    public GameObject[] slidableObstacles;
    public GameObject[] impassableObstacles;

    [Header("Houses")]
    public GameObject[] housePrefabs;
    [Range(0f, 1f)]
    public float houseSpawnChance = 0.6f; // веро€тность спавна дома в каждой точке

    [Header("Items")]
    public GameObject coinPrefab;
    public GameObject[] powerupPrefabs;

    [Header("Settings")]
    // ¬ј∆Ќќ: laneDistance = рассто€ние между полосами, Ќ≈ мен€етс€ при изменении длины тайла!
    // ≈сли полосы были на рассто€нии 2 Ч оставь 2, даже если тайл стал длиннее.
    public float laneDistance = 2f;
    public int coinsPerLine = 10;
    public float coinSpacing = 1.5f;
    public float groundCoinHeight = 1f;
    public float airCoinHeight = 7f;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private float tileMinZ;
    private float tileMaxZ;
    private float tileCenterZ;

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            tileMinZ = col.bounds.min.z;
            tileMaxZ = col.bounds.max.z;
            tileCenterZ = col.bounds.center.z;
        }
        else
        {
            tileMinZ = transform.position.z;
            tileMaxZ = transform.position.z + 60f;
            tileCenterZ = transform.position.z + 30f;
            Debug.LogWarning($"[RoadTile] Ќа {gameObject.name} нет Collider Ч Z позиции могут быть неточными!");
        }
    }

    public void SpawnObstacles(bool isFlying, bool isSafeZone = false)
    {
        ClearOldObjects();
        SpawnHouses();

        if (isSafeZone) return;

        if (isFlying)
        {
            SpawnSmartCoinPath();
        }
        else
        {
            List<int> blockedLanes = new List<int>();
            SpawnBarriers(blockedLanes);
            SpawnStraightCoinsOnGround(blockedLanes);
            SpawnPowerups(blockedLanes);
        }
    }

    void ClearOldObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    // X полос берЄм напр€мую из spawnPoints Ч они всегда корректны
    // ≈сли spawnPoints пустой Ч считаем от transform
    float GetLaneWorldX(int lane)
    {
        if (spawnPoints != null)
        {
            foreach (var p in spawnPoints)
            {
                if (GetLaneIndex(p.position) == lane)
                    return p.position.x;
            }
        }
        return transform.position.x + lane * laneDistance;
    }

    void SpawnSmartCoinPath()
    {
        if (coinPrefab == null) return;

        int startLane = lastJetpackLane;
        int targetLane = startLane;

        if (Random.value > 0.5f)
        {
            if (startLane == 0) targetLane = (Random.value > 0.5f) ? 1 : -1;
            else targetLane = 0;
        }

        int coinsCount = 20;
        float tileLength = tileMaxZ - tileMinZ;
        float step = tileLength / coinsCount;
        float spawnY = transform.position.y + airCoinHeight;

        float startX = GetLaneWorldX(startLane);
        float endX = GetLaneWorldX(targetLane);

        for (int i = 0; i < coinsCount; i++)
        {
            float t = (float)i / (float)(coinsCount - 1);
            float worldX = Mathf.Lerp(startX, endX, t);
            float worldZ = tileMinZ + (i * step);

            Vector3 spawnPos = new Vector3(worldX, spawnY, worldZ);

            if (i == coinsCount - 1 && Random.value < 0.2f && powerupPrefabs.Length > 0)
            {
                GameObject powerup = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
                GameObject obj = Instantiate(powerup, spawnPos, powerup.transform.rotation, transform);
                obj.transform.localScale = powerup.transform.localScale;
                spawnedObjects.Add(obj);
            }
            else
            {
                GameObject coin = Instantiate(coinPrefab, spawnPos, coinPrefab.transform.rotation, transform);
                coin.transform.localScale = coinPrefab.transform.localScale;
                spawnedObjects.Add(coin);
            }
        }

        lastJetpackLane = targetLane;
    }

    void SpawnBarriers(List<int> blockedLanes)
    {
        List<GameObject> allObstacles = new List<GameObject>();
        if (jumpableObstacles != null) allObstacles.AddRange(jumpableObstacles);
        if (slidableObstacles != null) allObstacles.AddRange(slidableObstacles);
        if (impassableObstacles != null) allObstacles.AddRange(impassableObstacles);

        if (spawnPoints.Length == 0 || allObstacles.Count == 0) return;

        List<Transform> availablePoints = new List<Transform>(spawnPoints);
        int obstaclesToSpawn = Random.Range(1, 3);

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
                GameObject obsPrefab = allObstacles[Random.Range(0, allObstacles.Count)];
                GameObject obj = Instantiate(obsPrefab, point.position, obsPrefab.transform.rotation, transform);
                obj.transform.localScale = obsPrefab.transform.localScale;
                spawnedObjects.Add(obj);
            }
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

        if (freeLanes.Count == 0) return;

        int chosenLane = freeLanes[Random.Range(0, freeLanes.Count)];
        float worldX = GetLaneWorldX(chosenLane);
        float spawnY = transform.position.y + groundCoinHeight;
        float startZ = tileMinZ + 2f;

        for (int i = 0; i < coinsPerLine; i++)
        {
            float worldZ = startZ + (i * coinSpacing);
            if (worldZ > tileMaxZ - 2f) break;

            Vector3 coinPos = new Vector3(worldX, spawnY, worldZ);
            GameObject coin = Instantiate(coinPrefab, coinPos, coinPrefab.transform.rotation, transform);
            coin.transform.localScale = coinPrefab.transform.localScale;
            spawnedObjects.Add(coin);
        }
    }

    void SpawnPowerups(List<int> blockedLanes)
    {
        if (powerupPrefabs.Length == 0) return;
        if (Random.value > 0.15f) return;

        List<int> freeLanes = new List<int>();
        for (int i = -1; i <= 1; i++)
        {
            if (!blockedLanes.Contains(i)) freeLanes.Add(i);
        }

        if (freeLanes.Count == 0) return;

        int lane = freeLanes[Random.Range(0, freeLanes.Count)];
        float worldX = GetLaneWorldX(lane);

        Vector3 pos = new Vector3(worldX, transform.position.y + 1.5f, tileCenterZ);
        GameObject powerup = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
        GameObject obj = Instantiate(powerup, pos, powerup.transform.rotation, transform);
        obj.transform.localScale = powerup.transform.localScale;
        spawnedObjects.Add(obj);
    }

    void SpawnHouses()
    {
        if (housePrefabs == null || housePrefabs.Length == 0) return;
        SpawnHousesAtPoints(leftHousePoints);
        SpawnHousesAtPoints(rightHousePoints);
    }

    void SpawnHousesAtPoints(Transform[] points)
    {
        if (points == null) return;

        foreach (var p in points)
        {
            if (p.childCount > 0) continue;

            // —лучайный пропуск Ч регулируй houseSpawnChance в инспекторе (0.6 = 60%)
            if (Random.value > houseSpawnChance) continue;

            GameObject housePrefab = housePrefabs[Random.Range(0, housePrefabs.Length)];
            GameObject obj = Instantiate(housePrefab, p.position, housePrefab.transform.rotation, p);
            obj.transform.localScale = housePrefab.transform.localScale;
            spawnedObjects.Add(obj);
        }
    }

    int GetLaneIndex(Vector3 worldPosition)
    {
        float localX = worldPosition.x - transform.position.x;
        float threshold = laneDistance / 2f;
        if (localX < -threshold) return -1;
        if (localX > threshold) return 1;
        return 0;
    }
}