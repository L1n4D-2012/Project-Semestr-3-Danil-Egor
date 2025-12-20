using System.Collections.Generic;
using UnityEngine;

public class RoadTile : MonoBehaviour
{
    [Header("Точки спавна")]
    public Transform[] spawnPoints;
    public Transform[] leftDecorPoints;
    public Transform[] rightDecorPoints;

    [Header("Преграды")]
    public GameObject[] jumpableObstacles;
    public GameObject[] slidableObstacles;
    public GameObject[] impassableObstacles;

    [Header("Декор")]
    public GameObject[] decorPrefabs;

    [Header("Бонусы и Монеты")]
    public GameObject coinPrefab;
    public GameObject[] powerupPrefabs;

    [Range(0f, 1f)] public float coinSpawnChance = 0.5f;
    [Range(0f, 1f)] public float powerupSpawnChance = 0.05f;
    public float coinHeightOffset = 1.0f;

    public void SpawnObstacles(int count)
    {
        SpawnDecor();

        List<GameObject> allPossibleObstacles = new List<GameObject>();
        if (jumpableObstacles != null) allPossibleObstacles.AddRange(jumpableObstacles);
        if (slidableObstacles != null) allPossibleObstacles.AddRange(slidableObstacles);
        if (impassableObstacles != null) allPossibleObstacles.AddRange(impassableObstacles);

        // ФИЛЬТРАЦИЯ ДУБЛИКАТОВ: Убираем точки с одинаковыми позициями
        List<Transform> availablePoints = new List<Transform>();
        List<Vector3> usedPositions = new List<Vector3>();

        foreach (var point in spawnPoints)
        {
            if (point != null)
            {
                bool alreadyExists = false;
                foreach (var pos in usedPositions)
                {
                    if (Vector3.Distance(point.position, pos) < 0.1f)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (!alreadyExists)
                {
                    usedPositions.Add(point.position);
                    availablePoints.Add(point);
                }
            }
        }

        if (availablePoints.Count > 0 && allPossibleObstacles.Count > 0)
        {
            int maxObstacles = availablePoints.Count > 1 ? availablePoints.Count - 1 : availablePoints.Count;
            int obstaclesToSpawn = Mathf.Clamp(count, 0, maxObstacles);

            for (int i = 0; i < obstaclesToSpawn; i++)
            {
                if (availablePoints.Count == 0) break;

                int randomPointIndex = Random.Range(0, availablePoints.Count);
                Transform chosenPoint = availablePoints[randomPointIndex];
                availablePoints.RemoveAt(randomPointIndex);

                int randomObstacleIndex = Random.Range(0, allPossibleObstacles.Count);
                GameObject chosenObstacle = allPossibleObstacles[randomObstacleIndex];

                GameObject spawnedObj = Instantiate(chosenObstacle, chosenPoint.position, chosenObstacle.transform.rotation, null);
                spawnedObj.transform.SetParent(transform, true);
            }
        }

        foreach (Transform point in availablePoints)
        {
            float roll = Random.value;

            if (powerupPrefabs.Length > 0 && roll < powerupSpawnChance)
            {
                GameObject powerup = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
                Vector3 pos = point.position;
                pos.y += 1f;
                GameObject spawned = Instantiate(powerup, pos, powerup.transform.rotation, null);
                spawned.transform.SetParent(transform, true);
            }
            else if (coinPrefab != null && roll < (coinSpawnChance + powerupSpawnChance))
            {
                Vector3 coinPos = point.position;
                coinPos.y += coinHeightOffset;
                GameObject spawnedCoin = Instantiate(coinPrefab, coinPos, coinPrefab.transform.rotation, null);
                spawnedCoin.transform.SetParent(transform, true);
            }
        }
    }

    void SpawnDecor()
    {
        if (decorPrefabs.Length == 0) return;

        if (leftDecorPoints.Length > 0)
        {
            foreach (Transform point in leftDecorPoints)
            {
                if (Random.value > 0.3f)
                {
                    GameObject decor = decorPrefabs[Random.Range(0, decorPrefabs.Length)];
                    GameObject spawned = Instantiate(decor, point.position, decor.transform.rotation, null);
                    spawned.transform.SetParent(transform, true);
                }
            }
        }

        if (rightDecorPoints.Length > 0)
        {
            foreach (Transform point in rightDecorPoints)
            {
                if (Random.value > 0.3f)
                {
                    GameObject decor = decorPrefabs[Random.Range(0, decorPrefabs.Length)];
                    GameObject spawned = Instantiate(decor, point.position, decor.transform.rotation, null);
                    spawned.transform.SetParent(transform, true);
                }
            }
        }
    }
}