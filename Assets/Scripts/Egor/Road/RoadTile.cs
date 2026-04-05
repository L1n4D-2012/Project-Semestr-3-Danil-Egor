using System.Collections.Generic;
using UnityEngine;

public class RoadTile : MonoBehaviour
{
    // --- СТАТИЧЕСКАЯ ПАМЯТЬ (Общая для всех кусков дороги) ---
    // 0 = Центр, -1 = Лево, 1 = Право
    public static int lastJetpackLane = 0;
    // ---------------------------------------------------------

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
    public int coinsPerLine = 10;
    public float coinSpacing = 1.5f;
    public float groundCoinHeight = 1f;
    public float airCoinHeight = 7f;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void SpawnObstacles(bool isFlying, bool isSafeZone = false)
    {
        ClearOldObjects();
        SpawnDecor();

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

    void SpawnSmartCoinPath()
    {
        if (coinPrefab == null) return;

        int startLane = lastJetpackLane;
        int targetLane = startLane;

        // Шанс поворота (50% прямо, 50% поворот)
        if (Random.value > 0.5f)
        {
            if (startLane == 0) targetLane = (Random.value > 0.5f) ? 1 : -1;
            else targetLane = 0;
        }

        // --- НАСТРОЙКИ ДЛИНЫ И ПЛОТНОСТИ ---
        // Увеличиваем кол-во монет, чтобы линия была сплошной
        // Раньше было 10, ставим 15 или 20, чтобы заполнить ВЕСЬ тайл
        int coinsCount = 20;

        // Вычисляем шаг так, чтобы монеты равномерно покрыли всю длину тайла (30м)
        // 30 метров / 20 монет = 1.5 метра между монетами
        float step = 30f / coinsCount;

        for (int i = 0; i < coinsCount; i++)
        {
            // t идет от 0 до 1
            float t = (float)i / (float)(coinsCount - 1);

            // Плавный переход X
            float currentLaneX = Mathf.Lerp(startLane * laneDistance, targetLane * laneDistance, t);

            // --- ГЛАВНОЕ ИСПРАВЛЕНИЕ Z (УБИРАЕМ РАЗРЫВЫ) ---
            // Спавним монеты от самого начала (-15) до самого конца (+15) тайла.
            // Так как тайлы стоят встык, линии монет тоже состыкуются идеально.
            float zLocal = -15f + (i * step);

            Vector3 spawnPos = transform.position + new Vector3(currentLaneX, airCoinHeight, zLocal);

            // 20% шанс бонуса на самой последней монете
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

        if (freeLanes.Count > 0)
        {
            int chosenLane = freeLanes[Random.Range(0, freeLanes.Count)];
            float xPos = chosenLane * laneDistance;

            for (int i = 0; i < coinsPerLine; i++)
            {
                float zPos = -5f + (i * coinSpacing);
                Vector3 coinPos = transform.position + new Vector3(xPos, groundCoinHeight, zPos);

                GameObject coin = Instantiate(coinPrefab, coinPos, coinPrefab.transform.rotation, transform);
                coin.transform.localScale = coinPrefab.transform.localScale;
                spawnedObjects.Add(coin);
            }
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

        if (freeLanes.Count > 0)
        {
            int lane = freeLanes[Random.Range(0, freeLanes.Count)];
            float xPos = lane * laneDistance;

            Vector3 pos = transform.position + new Vector3(xPos, 1.5f, 0);
            GameObject powerup = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
            GameObject obj = Instantiate(powerup, pos, powerup.transform.rotation, transform);
            obj.transform.localScale = powerup.transform.localScale;
            spawnedObjects.Add(obj);
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
            if (p.childCount == 0 && Random.value > 0.4f)
            {
                GameObject decor = decorPrefabs[Random.Range(0, decorPrefabs.Length)];
                GameObject obj = Instantiate(decor, p.position, decor.transform.rotation, p);
                obj.transform.localScale = decor.transform.localScale;
            }
        }
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


