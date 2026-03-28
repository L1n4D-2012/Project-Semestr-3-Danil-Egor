using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WalletInMain : MonoBehaviour
{
    public TMP_Text text;
    public string CoinsText;
    public int coinsNow = 0;
   
   

    

    private void Awake()
    {
        
        coinsNow = PlayerPrefs.GetInt("CoinsMenu");
        PlayerPrefs.Save();
    }

    public void Update_UI()
    {
        text.text = CoinsText;
        CoinsText = coinsNow.ToString();
    }
}
