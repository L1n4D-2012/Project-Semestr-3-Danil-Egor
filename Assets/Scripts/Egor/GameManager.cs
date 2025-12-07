using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // для інших скриптів ДАНЯ
    public GameObject gameOverCanvas;   // На канвас

    void Awake()
    {
        // Делаем этот скрипт главным
        if (instance == null) instance = this;
    }

    public void EndGame()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Игра окончена!");
    }
}