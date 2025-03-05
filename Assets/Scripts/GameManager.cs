using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    bool gameHasEnded = false;
    public float restartDelay = 5f;
    public GameObject failedLevelUI;
    public GameObject victoryLevelUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (failedLevelUI != null)
        {
            failedLevelUI.SetActive(false);
            Debug.Log("FailedLevelUI disabled on scene start.");
        }
        if (victoryLevelUI != null)
        {
            victoryLevelUI.SetActive(false);
            Debug.Log("VictoryLevelUI disabled on scene start.");
        }
    }

    public void EndGame()
    {
        if (gameHasEnded) return; // Prevent multiple triggers
        gameHasEnded = true;

        if (failedLevelUI != null)
        {
            failedLevelUI.SetActive(true); // Show the "Game Over" UI
            Debug.Log("EndGame called. Showing FailedLevelUI.");
        }
        else
        {
            Debug.LogWarning("failedLevelUI is not assigned in the Inspector!");
        }

        // Automatically restart after the delay
        Invoke(nameof(RestartGame), restartDelay);
    }

    public void WinGame()
    {
        if (gameHasEnded) return; // Prevent multiple triggers
        gameHasEnded = true;

        if (victoryLevelUI != null)
        {
            victoryLevelUI.SetActive(true); // Show the "Victory" UI
            Debug.Log("WinGame called. Showing VictoryLevelUI.");
        }
        else
        {
            Debug.LogWarning("victoryLevelUI is not assigned in the Inspector!");
        }

        // Automatically restart after the delay
        Invoke(nameof(RestartGame), restartDelay);
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame called. Reloading scene.");
        gameHasEnded = false;
        Time.timeScale = 1f; // Resume the game
        Debug.Log("Time.timeScale reset to: " + Time.timeScale);
        if (failedLevelUI != null)
        {
            failedLevelUI.SetActive(false);
        }
        if (victoryLevelUI != null)
        {
            victoryLevelUI.SetActive(false);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }
}