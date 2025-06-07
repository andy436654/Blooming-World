using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockPanelController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button unlockWithCurrencyButton;
    [SerializeField] private Button unlockWithClicksButton;
    [SerializeField] private Button backButton;

    [Header("Text Settings")]
    [SerializeField] private TMP_Text unlockDescriptionText;
    [SerializeField] private string unlockTemplate = "«аплатите {0} или кликните {1} раз, чтобы открыть";

    private LevelButtonController currentLevelButton;
    private int remainingClicks;

    private void Awake()
    {
        unlockWithCurrencyButton.onClick.AddListener(UnlockWithCurrency);
        unlockWithClicksButton.onClick.AddListener(UnlockWithClick);
        Hide();
    }

    public void ShowUnlockOptions(LevelButtonController levelButton, int price, int clicksRequired)
    {
        currentLevelButton = levelButton;
        remainingClicks = clicksRequired;

        // ќбновл€ем текст с подстановкой значений
        UpdateUnlockText(price, clicksRequired);

        panel.SetActive(true);
    }

    private void UpdateUnlockText(int price, int clicks)
    {
        if (unlockDescriptionText != null)
        {
            unlockDescriptionText.text = string.Format(unlockTemplate, price, clicks);
        }
    }

    public void UpdateRemainingClicks(int clicks)
    {
        remainingClicks = clicks;
        UpdateUnlockText(currentLevelButton.UnlockPrice, clicks);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private void UnlockWithCurrency()
    {
        if (currentLevelButton != null)
        {
            currentLevelButton.TryUnlockWithCurrency();
        }
    }

    private void UnlockWithClick()
    {
        if (currentLevelButton != null)
        {
            currentLevelButton.TryUnlockWithClick();
        }
    }

    public void OnBackButtonClicked() 
    {
        gameObject.SetActive(false);
    }
}