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

            // ИСПРАВЛЕНИЕ: Проверяем, существует ли еще объект.
            // Если он был удален другим скриптом, убираем его из списка и выходим.
            if (firstTile == null)
            {
                activeRoads.RemoveAt(0);
                return;
            }

            // Если объект существует, работаем с ним как обычно (Строка 52 была тут)
            if (firstTile.transform.position.z < -roadLength - 15f)
            {
                RecycleTile();
            }
        }
    }

    void RecycleTile()
    {
        // Получаем первую и последнюю плитки ДО того, как что-то удалить
        GameObject oldTile = activeRoads[0];
        GameObject lastTile = activeRoads[activeRoads.Count - 1];

        // Обязательно проверяем lastTile на null, чтобы избежать ошибки при расчетах
        if (lastTile == null) return;

        float newSpawnZ = lastTile.transform.position.z + roadLength;

        // Удаляем из списка и уничтожаем
        activeRoads.RemoveAt(0);
        Destroy(oldTile);

        // Спавним новую
        SpawnTile(newSpawnZ, 100);
    }

    void SpawnTile(float zPos, int tileIndex)
    {
        GameObject newRoad = Instantiate(roadPrefab, new Vector3(0, 0, zPos), Quaternion.identity);
        activeRoads.Add(newRoad);

        bool isFlying = PlayerJetpack.instance != null && PlayerJetpack.instance.isFlying;
        bool isSafeZone = (tileIndex < 2);

        newRoad.GetComponent<RoadTile>().SpawnObstacles(isFlying, isSafeZone);
    }

    public void ForceUpdateRoads(bool isFlying)
    {
        foreach (var road in activeRoads)
        {
            // Еще одна защита: пропускаем удаленные объекты
            if (road == null) continue;

            if (road.transform.position.z < -5f) continue;
            road.GetComponent<RoadTile>().SpawnObstacles(isFlying, false);
        }
    }
}