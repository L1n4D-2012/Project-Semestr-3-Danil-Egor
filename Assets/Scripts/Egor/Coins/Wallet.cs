using UnityEngine;
using UnityEngine.UI;

public class Wallet : MonoBehaviour
{
    WalletInMain walletInMain;
    public static Wallet instance;
    public int coins;
    public Text coinText;
    public string labelText = "Coins: ";

    public int coinMultiplier = 1;
    private float multiplierTimer = 0f;

    void Awake()
    {
        // ������� �������: � ����� ����� - ����� ������� �������.
        // ������ ������� ����������� ��� ��� ����� �����, ��� ��� ���������� �� �����.
        instance = this;

        // ��������� ����������� ������
        coins = PlayerPrefs.GetInt("Coins", 0);
        PlayerPrefs.SetInt("CoinsMenu", coins);
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
        // ��������� �������� �� null, ����� ���� �� ���������, ���� ���� ����� ����������
        if (coinText != null)
        {
            coinText.text = labelText + coins.ToString();
        }
    }
}