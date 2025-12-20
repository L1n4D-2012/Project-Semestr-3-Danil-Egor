using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public static PlayerShield instance;
    public bool isShieldActive = false;
    public GameObject shieldVisual;

    void Awake()
    {
        instance = this;
    }

    public void ActivateShield()
    {
        isShieldActive = true;
        if (shieldVisual != null) shieldVisual.SetActive(true);
    }

    public bool TryUseShield()
    {
        if (isShieldActive)
        {
            isShieldActive = false;
            if (shieldVisual != null) shieldVisual.SetActive(false);
            return true;
        }
        return false;
    }
}