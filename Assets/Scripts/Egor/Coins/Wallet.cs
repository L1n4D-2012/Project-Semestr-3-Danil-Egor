using UnityEngine;
using UnityEngine.UI;

public class Wallet : MonoBehaviour
{
    public static Wallet instance;
    public int coins;
    public Text coinText;
    public string labelText = "Coins: ";

    public int coinMultiplier = 1;
    private float multiplierTimer = 0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        coins = PlayerPrefs.GetInt("Coins", 0);
        UpdateUI();
    }

    void Update()
    {
        if (multiplierTimer > 0)
        {
            multiplierTimer -= Time.deltaTime;
            if (multiplierTimer <= 0)
            {
                coinMultiplier = 1;
            }
        }
    }

    public void AddCoin(int amount)
    {
        coins += amount * coinMultiplier;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void ActivateDoubleCoins(float duration)
    {
        coinMultiplier = 2;
        multiplierTimer = duration;
    }

    public void ResetMoney()
    {
        coins = 0;
        PlayerPrefs.SetInt("Coins", 0);
        PlayerPrefs.Save();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = labelText + coins.ToString();
        }
    }
}