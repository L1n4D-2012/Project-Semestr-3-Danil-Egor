using System.Collections.Generic;
using UnityEngine;

public class SimpleRoadSpawner : MonoBehaviour
{
    public static SimpleRoadSpawner instance;
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

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = 120;
        globalSpeed = startSpeed;

        for (int i = 0; i < numberOfTiles; i++)
        {
            // ВАЖНО: Передаем индекс 'i' в функцию спавна
            SpawnTile(i * roadLength, i);
        }
    }

    void Update()
    {
        if (globalSpeed < maxSpeed)
        {
            globalSpeed += speedIncreaseRate * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        if (activeRoads.Count > 0)
        {
            GameObject firstTile = activeRoads[0];
            if (firstTile.transform.position.z < -roadLength - 15f) // Чуть увеличил дистанцию удаления
            {
                RecycleTile();
            }
        }
    }

    void RecycleTile()
    {
        GameObject oldTile = activeRoads[0];
        GameObject lastTile = activeRoads[activeRoads.Count - 1];

        float newSpawnZ = lastTile.transform.position.z + roadLength;

        activeRoads.RemoveAt(0);
        Destroy(oldTile);

        // При переработке плитки индекс уже большой, так что это не старт (isSafeZone = false)
        SpawnTile(newSpawnZ, 100);
    }

    // Добавил параметр tileIndex
    void SpawnTile(float zPos, int tileIndex)
    {
        GameObject newRoad = Instantiate(roadPrefab, new Vector3(0, 0, zPos), Quaternion.identity);
        activeRoads.Add(newRoad);

        bool isFlying = PlayerJetpack.instance != null && PlayerJetpack.instance.isFlying;

        // Если это первые 2 плитки (0 и 1), включаем Безопасную Зону (true)
        bool isSafeZone = (tileIndex < 2);

        newRoad.GetComponent<RoadTile>().SpawnObstacles(isFlying, isSafeZone);
    }

    public void ForceUpdateRoads(bool isFlying)
    {
        foreach (var road in activeRoads)
        {
            if (road.transform.position.z < -5f) continue;

            // При обновлении дороги мы считаем, что это уже не старт игры
            road.GetComponent<RoadTile>().SpawnObstacles(isFlying, false);
        }
    }
}