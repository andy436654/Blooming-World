using System; // �������� ��� ������������� Mathf
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float maxHeight = 5f; // ������������ ���������� ������
    public bool isGameOver = false;
    public bool isWin = false;
    public GameObject gameOverPanel;
    public Animator gameOverPanelAnimation;

    [Header("Win Condition")]
    public bool hasWinCondition = false; // ����� �� ����� � �������
    public GameObject youWinPanel; // ������ ������
    public Animator youWinPanelAnimation; // �������� ������ ������
    public AudioClip winSound; // ���� ������


    [Header("New Feedback Elements")]
    public List<Animator> indicatorAnimator = new List<Animator>(); // ������ �� �������� ����������
    public AudioClip gameOverSound; // ������ �� AudioClip ��� ����� ���������

    [Header("Currency Reward")]
    public TMP_Text currencyRewardText; // ������ �� TextMeshPro ����� ��� ����������� �������
    public float countDuration = 1.5f;
    public AnimationCurve countCurve;
    public float startDelay = 0.5f; // �������� ����� ������� �����

    private int pendingCurrencyToAdd;
    private bool isCounting;

    private void Awake() => Instance = this;

    void Start()
    {
        isWin = false;

        gameOverPanel.SetActive(false);
        youWinPanel.SetActive(false);

        // ��������� �������� ���� ����������� �� ������
        foreach (var animator in indicatorAnimator)
        {
            if (animator != null)
                animator.enabled = false;
        }
    }

    void Update()
    {
        if (isGameOver) return;

        // ��������� ������ ������
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (var block in blocks)
        {
            var mergeBlock = block.GetComponent<MergeBlocks>();
            if (mergeBlock != null && mergeBlock.IsMerging) continue;

            if (block.transform.position.y >= maxHeight)
            {
                GameOver();
                return; // ������� ����� ����� GameOver
            }
        }
    }

    // ��������� ����� ��� ���������� �����������
    public void UpdateIndicators()
    {
        indicatorAnimator.Clear();
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("Dot");
        foreach (GameObject obj in foundObjects)
        {
            Animator animator = obj.GetComponent<Animator>();
            if (animator != null)
                indicatorAnimator.Add(animator);
        }
    }

    public void ResetGameState()
    {
        isGameOver = false;
        isWin = false;
        gameOverPanel.SetActive(false);
        youWinPanel.SetActive(false);

        // ������� ������ ����������
        indicatorAnimator.Clear();

        // ���������� �������� �������
        if (gameOverPanelAnimation != null)
            gameOverPanelAnimation.SetBool("isGameOver", false);
        if (youWinPanelAnimation != null)
            youWinPanelAnimation.SetBool("isWin", false);
    }

    public void GameOver()
    {
        if (isGameOver) return; // ������ �� ������������� ������

        isGameOver = true;
        Debug.Log("Game Over! ����� ������� ������!");

        // ��������� ������ ����� ������� ������ ���������
        AwardCurrencyBasedOnScore(100f);

        // ���������� ������ Game Over
        gameOverPanel.SetActive(true);
        gameOverPanelAnimation.SetBool("isGameOver", true);

        // ��������� �������� ���� �����������
        foreach (var animator in indicatorAnimator)
        {
            if (animator != null)
            {
                animator.enabled = true;
                animator.SetTrigger("StartLoseBlinking");
            }
        }

        // ������������� ���� ���������
        if (gameOverSound != null)
            AudioManager.PlaySound(gameOverSound, transform.position);
    }

    public void YouWin()
    {
        if (isWin || !hasWinCondition) return; // ������ isGameOver �� isWin

        isWin = true;
        Debug.Log("You Win! ������� �������!");

        AwardCurrencyBasedOnScore(50f);

        // ��������� ��� ��� ���������
        youWinPanel.SetActive(true);
        youWinPanelAnimation.SetBool("isWin", true);

        foreach (var animator in indicatorAnimator)
        {
            if (animator != null)
            {
                animator.enabled = true;
                animator.SetTrigger("StartWinBlinking");
            }
        }

        if (winSound != null)
            AudioManager.PlaySound(winSound, transform.position);
    }

    private void AwardCurrencyBasedOnScore(float winMultiplyer)
    {
        int currentScore = ScoreManager.Instance.score;
        int currencyToAdd = Mathf.RoundToInt(currentScore / winMultiplyer);

        if (currencyToAdd > 0)
        {
            pendingCurrencyToAdd = currencyToAdd;
            currencyRewardText.text = "������ ��������: "; // ������������� ��������� ��������
            currencyRewardText.gameObject.SetActive(true);

            // �������� ����������� ���������� �������� ���������
            StartCoroutine(WaitForTextAppearance());

            CurrencyManager.Instance.AddCurrency(currencyToAdd);
        }
    }

    private IEnumerator WaitForTextAppearance()
    {
        // ���� ���� ����� ��������� ��������
        // (����� �������� �� �������� ���������, ���� ������������ ��������)
        yield return new WaitForSecondsRealtime(startDelay);

        // ��������� �������� �����
        StartCurrencyCountAnimation(pendingCurrencyToAdd);
    }

    public void StartCurrencyCountAnimation(int targetAmount)
    {
        if (isCounting) return;

        StartCoroutine(AnimateCurrencyCount(targetAmount));
    }

    private IEnumerator AnimateCurrencyCount(int targetAmount)
    {
        isCounting = true;
        float elapsed = 0f;
        int startingAmount = 0;

        while (elapsed < countDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / countDuration);
            progress = countCurve.Evaluate(progress);

            int currentAmount = (int)Mathf.Lerp(startingAmount, targetAmount, progress);
            currencyRewardText.text = $"������ ��������: {currentAmount}";

            yield return null;
        }

        currencyRewardText.text = $"������ ��������: {targetAmount}";
        isCounting = false;
    }
}