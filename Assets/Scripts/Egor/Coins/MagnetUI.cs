using UnityEngine;
using UnityEngine.UI;

public class MagnetUI : MonoBehaviour
{
    public Image timerImage;

    void Update()
    {
        if (CoinMagnet.instance != null && CoinMagnet.instance.isMagnetActive)
        {
            timerImage.enabled = true;
            timerImage.fillAmount = CoinMagnet.instance.GetTimeRatio();
        }
        else
        {
            timerImage.enabled = false;
        }
    }
}