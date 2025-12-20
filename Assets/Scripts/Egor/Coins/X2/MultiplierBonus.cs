using UnityEngine;

public class MultiplierBonus : MonoBehaviour
{
    public float duration = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Wallet.instance != null)
            {
                Wallet.instance.ActivateDoubleCoins(duration);
                Destroy(gameObject);
            }
        }
    }
}