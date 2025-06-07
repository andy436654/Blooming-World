using System; // Добавьте для использования Mathf
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float maxHeight = 5f; // Максимальная допустимая высота
    public bool isGameOver = false;
    public bool isWin = false;
    public GameObject gameOverPanel;
    public Animator gameOverPanelAnimation;

    [Header("Win Condition")]
    public bool hasWinCondition = false; // Нужен ли режим с победой
    public GameObject youWinPanel; // Панель победы
    public Animator youWinPanelAnimation; // Аниматор панели победы
    public AudioClip winSound; // Звук победы


    [Header("New Feedback Elements")]
    public List<Animator> indicatorAnimator = new List<Animator>(); // Ссылка на аниматор индикатора
    public AudioClip gameOverSound; // Ссылка на AudioClip для звука поражения

    [Header("Currency Reward")]
    public TMP_Text currencyRewardText; // Ссылка на TextMeshPro текст для отображения награды
    public float countDuration = 1.5f;
    public AnimationCurve countCurve;
    public float startDelay = 0.5f; // Задержка перед стартом счета

    private int pendingCurrencyToAdd;
    private bool isCounting;

    private void Awake() => Instance = this;

    void Start()
    {
        isWin = false;

        gameOverPanel.SetActive(false);
        youWinPanel.SetActive(false);

        // Отключаем анимацию всех индикаторов на старте
        foreach (var animator in indicatorAnimator)
        {
            if (animator != null)
                animator.enabled = false;
        }
    }

    void Update()
    {
        if (isGameOver) return;

        // Проверяем высоту блоков
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (var block in blocks)
        {
            var mergeBlock = block.GetComponent<MergeBlocks>();
            if (mergeBlock != null && mergeBlock.IsMerging) continue;

            if (block.transform.position.y >= maxHeight)
            {
                GameOver();
                return; // Выходим сразу после GameOver
            }
        }
    }

    // Отдельный метод для обновления индикаторов
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

        // Очищаем список аниматоров
        indicatorAnimator.Clear();

        // Сбрасываем анимации панелей
        if (gameOverPanelAnimation != null)
            gameOverPanelAnimation.SetBool("isGameOver", false);
        if (youWinPanelAnimation != null)
            youWinPanelAnimation.SetBool("isWin", false);
    }

    public void GameOver()
    {
        if (isGameOver) return; // Защита от многократного вызова

        isGameOver = true;
        Debug.Log("Game Over! Блоки слишком высоко!");

        // Начисляем валюту перед показом панели поражения
        AwardCurrencyBasedOnScore(100f);

        // Активируем панель Game Over
        gameOverPanel.SetActive(true);
        gameOverPanelAnimation.SetBool("isGameOver", true);

        // Запускаем анимацию всех индикаторов
        foreach (var animator in indicatorAnimator)
        {
            if (animator != null)
            {
                animator.enabled = true;
                animator.SetTrigger("StartLoseBlinking");
            }
        }

        // Воспроизводим звук поражения
        if (gameOverSound != null)
            AudioManager.PlaySound(gameOverSound, transform.position);
    }

    public void YouWin()
    {
        if (isWin || !hasWinCondition) return; // Меняем isGameOver на isWin

        isWin = true;
        Debug.Log("You Win! Уровень пройден!");

        AwardCurrencyBasedOnScore(50f);

        // Остальной код без изменений
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
            currencyRewardText.text = "Зелёных получено: "; // Устанавливаем начальное значение
            currencyRewardText.gameObject.SetActive(true);

            // Начинаем отслеживать завершение анимации появления
            StartCoroutine(WaitForTextAppearance());

            CurrencyManager.Instance.AddCurrency(currencyToAdd);
        }
    }

    private IEnumerator WaitForTextAppearance()
    {
        // Ждем пока текст полностью появится
        // (можно заменить на проверку аниматора, если используется аниматор)
        yield return new WaitForSecondsRealtime(startDelay);

        // Запускаем анимацию счета
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
            currencyRewardText.text = $"Зелёных получено: {currentAmount}";

            yield return null;
        }

        currencyRewardText.text = $"Зелёных получено: {targetAmount}";
        isCounting = false;
    }
}