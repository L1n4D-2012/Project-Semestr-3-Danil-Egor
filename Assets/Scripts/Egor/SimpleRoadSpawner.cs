using UnityEngine;

public class SimpleRoadSpawner : MonoBehaviour
{
    public GameObject roadPrefab;
    public int numberOfTiles = 10;
    public float roadLength = 30f;

    private float currentZPos = 0;

    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnNextTile();
        }
    }

    public void SpawnNextTile()
    {
        Instantiate(roadPrefab, new Vector3(0, 0, currentZPos), Quaternion.identity);
        currentZPos += roadLength;
    }
}