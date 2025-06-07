using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BlockUnlockNotification : MonoBehaviour
{
    public static BlockUnlockNotification Instance;

    [Header("UI Elements")]
    public GameObject notificationPanel;
    public Animator messagePanel;
    public Image currentBlockImage;
    public Image nextBlockImage;
    public Button closeButton;
    public TMP_Text notificationText; // ��������� ��������� �������

    [Header("Settings")]
    public float showDuration = 2f;
    public string defaultText = "New Block Unlocked!";
    public string level10Text = "Max Level Reached!"; // ����� ��� 10 ������

    [Header("Level Settings")]
    public int minLevelToShow = 3;
    public int maxLevel = 10; // ������������ �������

    private Coroutine _autoHideCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        notificationPanel.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideNotification);
        }
    }

    public void ShowNotification(Sprite currentBlockSprite, Sprite nextBlockSprite, int blockLevel)
    {
        if (blockLevel < minLevelToShow) return;

        // ���� ��� ���� ������, �� ���������� �����������
        if (GameManager.Instance != null && GameManager.Instance.isWin) return;

        currentBlockImage.sprite = currentBlockSprite;

        // ��������� ��� 10 ������
        if (blockLevel >= maxLevel)
        {
            GameManager.Instance.YouWin();
            messagePanel.SetBool("isWin", true);
            notificationText.text = level10Text;
            nextBlockImage.gameObject.SetActive(false); // �������� nextBlockImage
        }
        else
        {
            notificationText.text = defaultText;
            nextBlockImage.sprite = nextBlockSprite;
            nextBlockImage.gameObject.SetActive(true); // ���������� nextBlockImage
        }

        notificationPanel.SetActive(true);
        _autoHideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);
        HideNotification();
    }

    public void HideNotification()
    {
        if (_autoHideCoroutine != null)
        {
            StopCoroutine(_autoHideCoroutine);
            _autoHideCoroutine = null;
        }

        notificationPanel.SetActive(false);
    }
}