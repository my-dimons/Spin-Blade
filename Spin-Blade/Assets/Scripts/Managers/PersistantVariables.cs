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
    }
}