using UnityEngine;

public class DeadlyObstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerShield.instance != null && PlayerShield.instance.TryUseShield())
            {
                Destroy(gameObject);
            }
            else
            {
                GameManager.instance.EndGame();
            }
        }
    }
}