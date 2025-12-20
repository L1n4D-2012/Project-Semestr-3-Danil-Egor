using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    public static CoinMagnet instance;
    public bool isMagnetActive = false;
    public float pullRadius = 20f;
    public float pullSpeed = 100f;
    private float magnetTimer = 0f;
    private float totalDuration = 1f;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (isMagnetActive)
        {
            magnetTimer -= Time.deltaTime;
            if (magnetTimer <= 0) isMagnetActive = false;
        }
    }

    public void ActivateMagnet(float duration)
    {
        isMagnetActive = true;
        magnetTimer = duration;
        totalDuration = duration;
    }

    public float GetTimeRatio()
    {
        return Mathf.Clamp01(magnetTimer / totalDuration);
    }
}