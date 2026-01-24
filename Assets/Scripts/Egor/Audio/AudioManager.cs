using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Делаем скрипт доступным из любого места (Singleton)
    public static AudioManager instance;

    [Header("Источники звука (Audio Source)")]
    [Tooltip("Перетащи сюда AudioSource для музыки")]
    public AudioSource bgmSource;
    [Tooltip("Перетащи сюда AudioSource для эффектов")]
    public AudioSource sfxSource;

    [Header("Фоновая музыка")]
    public AudioClip bgmTrack1;
    public AudioClip bgmTrack2;

    void Awake()
    {
        // Проверка на дубликаты. Важно для мобилок при перезагрузке сцен!
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Не удалять при смене сцены
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Включаем первый трек при старте
        PlayBGM(1);
    }

    // --- УПРАВЛЕНИЕ МУЗЫКОЙ ---
    public void PlayBGM(int trackNumber)
    {
        AudioClip clipToPlay = (trackNumber == 1) ? bgmTrack1 : bgmTrack2;

        // Включаем, только если этот трек еще не играет
        if (bgmSource.clip != clipToPlay)
        {
            bgmSource.clip = clipToPlay;
            bgmSource.loop = true; // Зацикливаем
            bgmSource.Play();
        }
    }

    // --- УПРАВЛЕНИЕ ЭФФЕКТАМИ (SFX) ---
    // Вызывай эту функцию из других скриптов, например: AudioManager.instance.PlaySFX(jumpSound);
    public void PlaySFX(AudioClip clip)
    {
        // PlayOneShot идеально для телефонов: звуки могут накладываться друг на друга
        sfxSource.PlayOneShot(clip);
    }
}