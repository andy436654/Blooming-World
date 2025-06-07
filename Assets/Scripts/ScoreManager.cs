using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Добавляем для работы со сценами

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = 0;
    public int bestScore = 0;
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;

    private string BestScoreKey => $"BestScore_{SceneManager.GetActiveScene().name}"; // Генерируем ключ на основе имени сцены

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        LoadBestScore(); // Загружаем рекорд для текущего уровня
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Счёт: " + score;

        if (score > bestScore)
        {
            bestScore = score;
            bestScoreText.text = "Рекорд: " + bestScore;
            SaveBestScore();
        }
    }

    private void LoadBestScore()
    {
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestScoreText.text = "Рекорд: " + bestScore;
    }

    private void SaveBestScore()
    {
        PlayerPrefs.SetInt(BestScoreKey, bestScore);
        PlayerPrefs.Save();
    }
}