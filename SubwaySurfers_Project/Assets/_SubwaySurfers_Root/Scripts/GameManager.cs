using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private Transform player;

    [Header("UI")]
    [SerializeField] private UIManager uiManager;

    [Header("Puntuación")]
    private int currentScore = 0;
    private int totalCoins = 0;
    private int sessionCoins = 0;
    private float distanceTraveled = 0f;

    [Header("Configuración de Score")]
    [SerializeField] private float scorePerMeter = 10f;

    private Vector3 lastPlayerPosition;
    private bool isPaused = false;
    private bool isGameOver = false;

    // PlayerPrefs Keys
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string TOTAL_COINS_KEY = "TotalCoins";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadData();
    }

    private void Start()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }

        // IMPORTANTE: Asegurarse que empiece desbloqueado
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;

        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
            uiManager.UpdateCoins(sessionCoins);

            // Asegurarse que los menús estén ocultos al inicio
            uiManager.HidePauseMenu();
            uiManager.HideGameOverMenu();
        }

        Debug.Log("GameManager iniciado - Time.timeScale = " + Time.timeScale);
    }

    private void Update()
    {
        // DEBUG: Forzar despausa con F1 si hay problemas
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Debug.Log("F1 presionado - Forzando Time.timeScale = 1");
            Time.timeScale = 1f;
            isPaused = false;
            if (uiManager != null)
            {
                uiManager.HidePauseMenu();
            }
        }

        if (isGameOver || isPaused) return;

        // Calcular distancia y puntuación
        if (player != null)
        {
            float distance = player.position.z - lastPlayerPosition.z;
            if (distance > 0)
            {
                distanceTraveled += distance;

                int scoreGained = Mathf.FloorToInt(distance * scorePerMeter);
                AddScore(scoreGained);
            }
            lastPlayerPosition = player.position;
        }
    }

    // ========== MÉTODO PARA EL NUEVO INPUT SYSTEM ==========
    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (isGameOver) return;

        TogglePause();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
        }
    }

    public void AddCoins(int amount)
    {
        sessionCoins += amount;
        totalCoins += amount;

        if (uiManager != null)
        {
            uiManager.UpdateCoins(sessionCoins);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            Debug.Log("Juego pausado - Time.timeScale = 0");
            if (uiManager != null)
            {
                uiManager.ShowPauseMenu();
            }
        }
        else
        {
            Time.timeScale = 1f;
            Debug.Log("Juego reanudado - Time.timeScale = 1");
            if (uiManager != null)
            {
                uiManager.HidePauseMenu();
            }
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
        Debug.Log("Game Over - Time.timeScale = 0");

        int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        bool isNewHighScore = currentScore > highScore;

        if (isNewHighScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, currentScore);
        }

        PlayerPrefs.SetInt(TOTAL_COINS_KEY, totalCoins);
        PlayerPrefs.Save();

        if (uiManager != null)
        {
            uiManager.ShowGameOverMenu(currentScore, highScore, sessionCoins, totalCoins, isNewHighScore);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Reiniciando juego - Time.timeScale = 1");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MAIN MENU");
    }

    private void LoadData()
    {
        totalCoins = PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
    }

    // Getters
    public int GetCurrentScore() => currentScore;
    public int GetSessionCoins() => sessionCoins;
    public int GetTotalCoins() => totalCoins;
    public int GetHighScore() => PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    public float GetDistanceTraveled() => distanceTraveled;
    public bool IsPaused() => isPaused;
}