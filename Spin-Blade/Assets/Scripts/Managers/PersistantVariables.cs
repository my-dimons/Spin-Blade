using System.Collections;
using UnityEngine;

public class PersistentVariables : MonoBehaviour
{
    public static PersistentVariables Instance { get; private set; }

    // Persistent audio settings
    public float sfxVolume = 1f;
    public float musicVolume = 1f;

    public float difficulty = 1;
    public float moneyMultiplier = 1;

    public bool infiniteMode;

    [Header("Music Settings")]
    public AudioSource musicSource;
    public AudioClip[] musicTracks;
    public AudioClip currentMusic;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
        
        if (musicTracks.Length > 0)
            StartCoroutine(PlayMusicContinuously());
    }

    private void Update()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    private IEnumerator PlayMusicContinuously()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(3);
            // Pick a random track
            AudioClip clip = musicTracks[UnityEngine.Random.Range(0, musicTracks.Length)];
            musicSource.clip = clip;
            currentMusic = clip;
            musicSource.Play();

            Debug.Log($"Playing: {clip.name}");

            yield return new WaitForSecondsRealtime(clip.length);
            Debug.Log("Finished Music Clip");
            yield return new WaitForSecondsRealtime(Random.Range(10, 20));
        }
    }
}