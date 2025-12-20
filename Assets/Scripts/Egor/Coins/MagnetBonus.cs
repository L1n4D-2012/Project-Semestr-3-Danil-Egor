using UnityEngine;

public class MagnetBonus : MonoBehaviour
{
    public float duration = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CoinMagnet.instance != null)
            {
                CoinMagnet.instance.ActivateMagnet(duration);
                Destroy(gameObject);
            }
        }
    }
}