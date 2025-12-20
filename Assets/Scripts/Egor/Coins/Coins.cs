using UnityEngine;

public class Coins : MonoBehaviour
{
    public int value = 1;
    private bool isFlyingToPlayer = false;

    void Update()
    {
        if (CoinMagnet.instance != null && CoinMagnet.instance.isMagnetActive)
        {
            float distance = Vector3.Distance(transform.position, CoinMagnet.instance.transform.position);

            if (distance < CoinMagnet.instance.pullRadius)
            {
                isFlyingToPlayer = true;
                transform.SetParent(null);
            }
        }

        if (isFlyingToPlayer && CoinMagnet.instance != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, CoinMagnet.instance.transform.position, CoinMagnet.instance.pullSpeed * Time.deltaTime);
        }
    }

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