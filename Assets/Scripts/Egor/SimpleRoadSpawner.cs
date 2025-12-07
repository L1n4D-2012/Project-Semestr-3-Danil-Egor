using System.Collections.Generic;
using UnityEngine;

public class SimpleRoadSpawner : MonoBehaviour
{
    public static float globalSpeed;

    [Header("Base Settings")]
    public GameObject roadPrefab;
    public int numberOfTiles = 10;
    public float roadLength = 30f;

    [Header("Difficulty Settings")]
    public float startSpeed = 15f;
    public float maxSpeed = 40f;
    public float speedIncreaseRate = 0.5f;

    private List<GameObject> activeRoads = new List<GameObject>();

    void Start()
    {
        globalSpeed = startSpeed;

        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile(i * roadLength, 0);
        }
    }

    void Update()
    {
        if (globalSpeed < maxSpeed)
        {
            globalSpeed += speedIncreaseRate * Time.deltaTime;
        }

        if (activeRoads.Count > 0)
        {
            GameObject firstTile = activeRoads[0];

            if (firstTile.transform.position.z < -roadLength)
            {
                GameObject lastTile = activeRoads[activeRoads.Count - 1];
                float newSpawnZ = lastTile.transform.position.z + roadLength;

                int difficulty = CalculateDifficulty();

                SpawnTile(newSpawnZ, difficulty);

                Destroy(firstTile);
                activeRoads.RemoveAt(0);
            }
        }
    }

    int CalculateDifficulty()
    {
        if (globalSpeed < 20f) return 1;
        if (globalSpeed < 30f) return 2;
        return 3;
    }

    void SpawnTile(float zPos, int obstacleCount)
    {
        GameObject newRoad = Instantiate(roadPrefab, new Vector3(0, 0, zPos), Quaternion.identity);
        activeRoads.Add(newRoad);

        // Эта функция теперь сама вызовет и SpawnDecor внутри себя
        newRoad.GetComponent<RoadTile>().SpawnObstacles(obstacleCount);
    }
}