using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class LevelButtonController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button levelButton;
    [SerializeField] private Image buttonImage;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private Button lockButton; // ��������� ��������� ������ �� ������ �����

    [Header("Sprites")]
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite lockedSprite;

    [Header("Settings")]
    [SerializeField] private string levelSceneName; // "ForestLevel"
    [SerializeField] private int levelOrder; // 1, 2, 3...
    [SerializeField] private int unlockPrice = 100;
    public int UnlockPrice => unlockPrice;
    [SerializeField] private int requiredClicksToUnlock = 5;

    [Header("Unlock Settings")]
    [SerializeField] private bool unlockByPreviousLevel = true;
    public UnlockCondition[] unlockConditions;

    [Header("Events")]
    public UnityEvent onLevelUnlocked;
    public UnityEvent onLevelSelected;

    private bool isLocked = true;
    private int currentClicks = 0;

    private CurrencyManager currencyManager => CurrencyManager.Instance;
    private UnlockPanelController unlockPanel;

    private const string UnlockKeyPrefix = "LevelUnlocked_";
    private const string ClicksKeyPrefix = "LevelClicks_";

    private void Awake()
    {
        LoadUnlockState(); // ��������� ��������� ��� ������

        unlockPanel = FindObjectOfType<UnlockPanelController>(true);

        levelButton.onClick.AddListener(() => {
            if (!isLocked) LoadLevel();
        });

        if (lockButton != null)
        {
            lockButton.onClick.AddListener(OnLockClicked);
        }

        // ���� �������� ������������� �� ����������� ������
        if (unlockByPreviousLevel && levelOrder > 1)
        {
            CheckPreviousLevelCompletion();
        }

        UpdateButtonState(); // �����!
    }

    private void Start()
    {
        CheckUnlockConditions();
    }

    public void CheckUnlockConditions()
    {
        bool shouldUnlock = true;

        foreach (var condition in unlockConditions)
        {
            if (!condition.IsMet())
            {
                shouldUnlock = false;
                break;
            }
        }

        if (shouldUnlock) UnlockLevel();
    }

    private void CheckPreviousLevelCompletion()
    {
        // ��������� ���������� ����������� ������
        bool prevLevelCompleted = PlayerPrefs.GetInt($"Level_{levelOrder - 1}_Completed", 0) == 1;

        if (prevLevelCompleted)
        {
            UnlockLevel();
        }
    }

    public void CompleteLevel() // �������� ��� �������� ����������� ������
    {
        PlayerPrefs.SetInt($"Level_{levelOrder}_Completed", 1);
        PlayerPrefs.Save();

        // ������������� ������������ ��������� �������
        UnlockNextLevel();
    }

    private void UnlockNextLevel()
    {
        // ������� ��������� ������ ������
        var nextLevelButton = FindObjectsOfType<LevelButtonController>()
            .FirstOrDefault(b => b.levelOrder == this.levelOrder + 1);

        nextLevelButton?.UnlockLevel();
    }

    private void LoadUnlockState()
    {
        string key = UnlockKeyPrefix + levelOrder;
        isLocked = PlayerPrefs.GetInt(key, 1) == 1; // 1 - �������������, 0 - ��������������

        string clicksKey = ClicksKeyPrefix + levelOrder;
        currentClicks = PlayerPrefs.GetInt(clicksKey, 0);

        UpdateButtonState();
    }

    public void Initialize(int level, bool locked, int price, int clicksRequired)
    {
        levelOrder = level;
        isLocked = locked;
        unlockPrice = price;
        requiredClicksToUnlock = clicksRequired;

        UpdateButtonState();
    }

    private void OnButtonClick()
    {
        if (isLocked)
        {
            ShowUnlockPanel();
        }
        else
        {
            onLevelSelected.Invoke();
            LoadLevel();
        }
    }

    private void SaveUnlockState()
    {
        string key = UnlockKeyPrefix + levelOrder;
        PlayerPrefs.SetInt(key, isLocked ? 1 : 0);

        string clicksKey = ClicksKeyPrefix + levelOrder;
        PlayerPrefs.SetInt(clicksKey, currentClicks);

        PlayerPrefs.Save(); // �����: ��������� �� ����
    }

    private void OnLockClicked()
    {
        if (isLocked)
        {
            ShowUnlockPanel();
        }
    }

    private void ShowUnlockPanel()
    {
        if (unlockPanel != null)
        {
            unlockPanel.ShowUnlockOptions(this, unlockPrice, requiredClicksToUnlock - currentClicks);
        }
    }

    public void TryUnlockWithCurrency()
    {
        if (currencyManager.SpendCurrency(unlockPrice))
        {
            UnlockLevel();
        }
        else
        {
            Debug.Log("Not enough currency");
            // ����� ������� ������� ��� �������� UI-�����������
        }
    }

    public void TryUnlockWithClick()
    {
        currentClicks++;
        SaveUnlockState(); // ��������� ����� ������� �����

        if (currentClicks >= requiredClicksToUnlock)
        {
            UnlockLevel();
        }
        else
        {
            UpdateButtonState();
            // ��������� ����� �� ������, ���� ��� �������
            if (unlockPanel != null && unlockPanel.gameObject.activeSelf)
            {
                unlockPanel.UpdateRemainingClicks(requiredClicksToUnlock - currentClicks);
            }
        }
    }

    private void UnlockLevel()
    {
        isLocked = false;
        currentClicks = 0;
        SaveUnlockState();
        UpdateButtonState();
        onLevelUnlocked.Invoke();

        if (unlockPanel != null)
        {
            unlockPanel.Hide();
        }
    }

    private void UpdateButtonState()
    {
        buttonImage.sprite = isLocked ? lockedSprite : unlockedSprite;
        lockedOverlay.SetActive(isLocked);
    }

    private void LoadLevel()
    {
        // ��������� ������� ��������� �������
        PlayerPrefs.SetInt("LastSelectedLevel", levelOrder);

        // ��������� ����� ������
        SceneManager.LoadScene(levelSceneName);
    }

    public bool IsLocked => isLocked;
    public int LevelNumber => levelOrder;
}