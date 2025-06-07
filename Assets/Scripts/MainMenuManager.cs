using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _levelSelectionCanvas; // Serialized ���� ��� ������������
    [SerializeField]
    private GameObject _settingsSelectionCanvas; // Serialized ���� ��� ������������
    [SerializeField]
    private GameObject _menuSelectionCanvas; // Serialized ���� ��� ������������

    public TMP_Text currencyCounter;

    private void Start()
    {
        // ��������� ����� ��� ������
        UpdateCurrencyUI(CurrencyManager.Instance.CurrentCurrency);

        // ������������� �� ��������� ������
        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyUI;
    }

    private void UpdateCurrencyUI(int newCurrencyValue)
    {
        currencyCounter.text = "������: " + newCurrencyValue.ToString();

        // �����������: �������� ���������
        // LeanTween.scale(currencyText.gameObject, Vector3.one * 1.2f, 0.3f)
        //     .setEasePunch();
    }
     public void OnPlayButtonClicked()
    {
        SetMenuActive(false);
        SetSettingsActive(false);
        SetLevelSelectionActive(true);
    }
    public void OnMenuButtonClicked()
    {
        SetMenuActive(true);
        SetSettingsActive(false);
        SetLevelSelectionActive(false);
    }
    public void OnSettingsButtonClicked()
    {
        SetSettingsActive(true);
        SetMenuActive(false);
        SetLevelSelectionActive(false);
    }
    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    private void SetMenuActive(bool isActive)
    {
        _menuSelectionCanvas.SetActive(isActive);
    }

    private void SetSettingsActive(bool isActive)
    {
        _settingsSelectionCanvas.SetActive(isActive);
    }

    private void SetLevelSelectionActive(bool isActive)
    {
        if (_levelSelectionCanvas != null)
        {
            _levelSelectionCanvas.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Level Selection Canvas is not assigned!", this);
        }
    }

    private void Update()
    {
        currencyCounter.text = $"������: {CurrencyManager.Instance.CurrentCurrency}";
    }

    private void OnDestroy()
    {
        // �����: ������������ ��� ����������� �������
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyUI;
        }
    }
}