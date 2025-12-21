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
        isFlying = true;
        timer = duration;

        if (CoinMagnet.instance != null)
        {
            CoinMagnet.instance.ActivateMagnet(duration);
        }
    }

    void StopJetpack()
    {
        isFlying = false;
    }
}