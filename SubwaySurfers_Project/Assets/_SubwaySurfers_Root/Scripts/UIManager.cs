using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD - Juego")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Menú de Pausa")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private TextMeshProUGUI pauseScoreText;
    [SerializeField] private TextMeshProUGUI pauseCoinsText;

    [Header("Menú Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI sessionCoinsText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private GameObject newHighScoreLabel;

    private void Start()
    {
        // Asegurarse que solo el HUD esté visible al inicio
        ShowHUD();
        HidePauseMenu();
        HideGameOverMenu();
    }

    // ========== HUD ==========
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }

    public void UpdateCoins(int coins)
    {
        if (coinsText != null)
        {
            coinsText.text = $"{coins}";
        }
    }

    private void ShowHUD()
    {
        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }
    }

    private void HideHUD()
    {
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }
    }

    // ========== PAUSE MENU ==========
    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        if (pauseScoreText != null && GameManager.Instance != null)
        {
            pauseScoreText.text = $"{GameManager.Instance.GetCurrentScore()}";
        }

        if (pauseCoinsText != null && GameManager.Instance != null)
        {
            pauseCoinsText.text = $"{GameManager.Instance.GetSessionCoins()}";
        }
    }

    public void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    // ========== GAME OVER MENU ==========
    public void ShowGameOverMenu(int finalScore, int highScore, int sessionCoins, int totalCoins, bool isNewHighScore)
    {
        HideHUD();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Puntuacion Final: {finalScore}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = $"Maxima Puntuacion {highScore}";
        }

        if (sessionCoinsText != null)
        {
            sessionCoinsText.text = $"Monedas Conseguidas: {sessionCoins}";
        }

        if (totalCoinsText != null)
        {
            totalCoinsText.text = $"Monedas Totales: {totalCoins}";
        }

        if (newHighScoreLabel != null)
        {
            newHighScoreLabel.SetActive(isNewHighScore);
        }
    }

    public void HideGameOverMenu()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        ShowHUD();
    }

    // ========== BOTONES ==========
    public void OnResumeButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    public void OnRestartButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    public void OnMainMenuButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
    }
}