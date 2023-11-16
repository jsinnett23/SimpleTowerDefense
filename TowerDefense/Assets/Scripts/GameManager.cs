using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform exitPoint; // The common exit point

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional, to persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Other GameManager methods...
}