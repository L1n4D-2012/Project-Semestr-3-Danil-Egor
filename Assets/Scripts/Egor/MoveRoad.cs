using UnityEngine;

public class MoveRoad : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.back * SimpleRoadSpawner.globalSpeed * Time.deltaTime);
    }
}