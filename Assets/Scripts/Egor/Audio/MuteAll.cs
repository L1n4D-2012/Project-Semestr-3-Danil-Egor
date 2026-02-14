using UnityEngine;

public class MuteAllSounds : MonoBehaviour
{
    private bool isMuted = false;

    public GameObject X;

    public void ToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f;
        Debug.Log("Mute: " + isMuted);
        
        X.SetActive(isMuted);
    }
    
}