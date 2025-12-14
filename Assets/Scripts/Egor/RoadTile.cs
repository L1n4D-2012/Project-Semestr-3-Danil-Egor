using System.Collections.Generic;
using UnityEngine;

public class RoadTile : MonoBehaviour
{
    [Header("Точки спавна ПРЕГРАД")]
    public Transform[] spawnPoints;
    [Header("Точки спавна ДЕКОРА (Слева и Справа)")]
    public Transform[] leftDecorPoints;
    public Transform[] rightDecorPoints;

    [Header("Преграды")]
    public GameObject[] jumpableObstacles;
    public GameObject[] slidableObstacles;
    public GameObject[] impassableObstacles;

    [Header("Декор (Дома, деревья, столбы)")]
    public GameObject[] decorPrefabs;

    [Header("Монетки")]
    public GameObject coinPrefab;
    [Range(0f, 1f)] public float coinSpawnChance = 0.5f;
    public float coinHeightOffset = 1.0f;

    public void SpawnObstacles(int count)
    {
        SpawnDecor();

        List<GameObject> allPossibleObstacles = new List<GameObject>();
        if (jumpableObstacles != null) allPossibleObstacles.AddRange(jumpableObstacles);
        if (slidableObstacles != null) allPossibleObstacles.AddRange(slidableObstacles);
        if (impassableObstacles != null) allPossibleObstacles.AddRange(impassableObstacles);

        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        if (spawnPoints.Length > 0 && allPossibleObstacles.Count > 0)
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

        if (coinPrefab != null)
        {
            foreach (Transform point in availablePoints)
            {
                if (Random.value < coinSpawnChance)
                {
                    Vector3 coinPos = point.position;
                    coinPos.y += coinHeightOffset;

                    GameObject spawnedCoin = Instantiate(coinPrefab, coinPos, coinPrefab.transform.rotation, null);
                    spawnedCoin.transform.SetParent(transform, true);
                }
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