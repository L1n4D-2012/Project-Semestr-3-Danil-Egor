using UnityEngine;

public class JetpackBonus : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Эта строка покажет в консоли ИМЯ того, кто коснулся бонуса
        Debug.Log("Джетпака коснулся объект: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Тег верный! Пробую включить полет...");

            if (PlayerJetpack.instance != null)
            {
                PlayerJetpack.instance.ActivateJetpack();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("ОШИБКА: На игроке нет скрипта PlayerJetpack (или он выключен)!");
            }
        }
        else
        {
            Debug.Log("ОШИБКА: У объекта нет тега Player! Его тег: " + other.tag);
        }
    }
}