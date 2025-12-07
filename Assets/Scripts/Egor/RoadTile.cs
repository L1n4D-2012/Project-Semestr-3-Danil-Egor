using UnityEngine;

public class RoadTile : MonoBehaviour
{
    [Header("Куда спавнить")]
    public Transform[] spawnPoints;

    [Header("Что спавнить")]
    public GameObject[] obstaclePrefabs;

    void Start()
    {
        SpawnRandomObstacle();
    }

    void SpawnRandomObstacle()
    {
        if (spawnPoints.Length == 0 || obstaclePrefabs.Length == 0) return;

        int randomPointIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenPoint = spawnPoints[randomPointIndex];

        int randomObstacleIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject chosenObstacle = obstaclePrefabs[randomObstacleIndex];
        GameObject spawnedObj = Instantiate(chosenObstacle, chosenPoint.position, chosenObstacle.transform.rotation, null);
        spawnedObj.transform.SetParent(transform, true);
    }
}