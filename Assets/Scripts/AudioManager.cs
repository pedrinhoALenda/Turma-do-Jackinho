using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton
    public static AudioManager instance;

    [Header("Background Music")]
    public AudioSource backgroundMusic;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float defaultVolume = 0.5f;

    void Awake()
    {
        // Implementa Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeMusic()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = defaultVolume;
            backgroundMusic.loop = true;
            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.Play();
            }
        }
        else
        {
            Debug.LogWarning("AudioManager: Nenhuma música atribuída ao AudioSource.");
        }
    }

    /// <summary>
    /// Ajusta o volume da música.
    /// </summary>
    /// <param name="volume">Valor entre 0 e 1</param>
    public void SetVolume(float volume)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = Mathf.Clamp01(volume);
        }
    }

    /// <summary>
    /// Para a música.
    /// </summary>
    public void StopMusic()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
    }

    /// <summary>
    /// Reproduz a música se não estiver tocando.
    /// </summary>
    public void PlayMusic()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }
}
