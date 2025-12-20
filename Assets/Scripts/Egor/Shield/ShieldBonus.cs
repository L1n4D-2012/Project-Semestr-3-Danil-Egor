using UnityEngine;

public class ShieldBonus : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerShield.instance != null)
            {
                PlayerShield.instance.ActivateShield();
                Destroy(gameObject);
            }
        }
    }
}