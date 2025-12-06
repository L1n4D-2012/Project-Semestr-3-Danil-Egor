using System;
using UnityEngine;
using TMPro;
public class CoinManager : MonoBehaviour
{
    [SerializeField] public TMP_Text coinText;
    
    public static CoinManager Instance;

    public int coins = 0;

    private void Start()
    {
       
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
       
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        Debug.Log("Coins: " + coins);
         
    }
}