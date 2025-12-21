using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject gameOverCanvas;

    public int targetFrameRate = 120;
    public bool disableShadowsOnMobile = true;

    void Awake()
    {
        if (instance == null) instance = this;

        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (disableShadowsOnMobile)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
            }
        }
    }

    public void EndGame()
    {
        if (Wallet.instance != null)
        {
            Wallet.instance.ResetMoney();
        }

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}