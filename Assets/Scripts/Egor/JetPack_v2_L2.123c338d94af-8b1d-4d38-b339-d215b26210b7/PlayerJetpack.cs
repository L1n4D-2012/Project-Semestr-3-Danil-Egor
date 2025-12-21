using UnityEngine;

public class PlayerJetpack : MonoBehaviour
{
    public static PlayerJetpack instance;
    public bool isFlying = false;
    public float duration = 5f;
    private float timer = 0f;

    private Rigidbody rb;

    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isFlying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StopJetpack();
            }
        }
    }

    public void ActivateJetpack()
    {
        if (isFlying) return;

        isFlying = true;
        timer = duration;

        // --- ВАЖНО: Сбрасываем память дорожки ---
        // Начинаем строить линию монет с центра (индекс 0), или можно считать полосу игрока
        RoadTile.lastJetpackLane = 0;
        // ----------------------------------------

        if (SimpleRoadSpawner.instance != null)
            SimpleRoadSpawner.instance.ForceUpdateRoads(true);
    }

    void StopJetpack()
    {
        isFlying = false;

        if (SimpleRoadSpawner.instance != null)
            SimpleRoadSpawner.instance.ForceUpdateRoads(false);
    }
}