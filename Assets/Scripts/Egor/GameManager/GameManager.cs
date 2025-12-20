using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject gameOverCanvas;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void EndGame()
    {
        if (Wallet.instance != null)
        {
            Wallet.instance.ResetMoney();
        }

        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Игра окончена!");
    }
}