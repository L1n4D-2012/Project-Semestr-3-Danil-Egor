using UnityEngine;

public class Coins : MonoBehaviour
{
    public int value = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Wallet.instance != null)
            {
                Wallet.instance.AddCoin(value);
            }
            Destroy(gameObject);
        }
    }
}