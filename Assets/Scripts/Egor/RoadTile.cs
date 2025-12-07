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
        int randomPointIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenPoint = spawnPoints[randomPointIndex];

        int randomObstacleIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject chosenObstacle = obstaclePrefabs[randomObstacleIndex];

        Instantiate(chosenObstacle, chosenPoint.position, chosenPoint.rotation, transform);
    }
}