using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    private const string CurrencyKey = "PlayerCurrency";
    public int CurrentCurrency { get; private set; }

    public event System.Action<int> OnCurrencyChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        LoadCurrency();
    }

    public void AddCurrency(int amount)
    {
        CurrentCurrency += amount;
        PlayerPrefs.SetInt(CurrencyKey, CurrentCurrency);
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke(CurrentCurrency);
    }


    public bool SpendCurrency(int amount)
    {
        if (CurrentCurrency >= amount)
        {
            CurrentCurrency -= amount;
            PlayerPrefs.SetInt(CurrencyKey, CurrentCurrency);
            PlayerPrefs.Save();
            OnCurrencyChanged?.Invoke(CurrentCurrency); // Важно: вызываем событие
            return true;
        }
        return false;
    }

    private void LoadCurrency()
    {
        CurrentCurrency = PlayerPrefs.GetInt(CurrencyKey, 0);
    }
}