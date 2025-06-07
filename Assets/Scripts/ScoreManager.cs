using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // ��������� ��� ������ �� �������

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = 0;
    public int bestScore = 0;
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;

    private string BestScoreKey => $"BestScore_{SceneManager.GetActiveScene().name}"; // ���������� ���� �� ������ ����� �����

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        LoadBestScore(); // ��������� ������ ��� �������� ������
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "����: " + score;

        if (score > bestScore)
        {
            bestScore = score;
            bestScoreText.text = "������: " + bestScore;
            SaveBestScore();
        }
    }

    private void LoadBestScore()
    {
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestScoreText.text = "������: " + bestScore;
    }

    private void SaveBestScore()
    {
        PlayerPrefs.SetInt(BestScoreKey, bestScore);
        PlayerPrefs.Save();
    }
}