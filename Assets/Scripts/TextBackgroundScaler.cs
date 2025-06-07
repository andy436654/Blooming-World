using UnityEngine;
using UnityEngine.UI;
using TMPro; // Если используете TextMeshPro

[RequireComponent(typeof(Image))]
[ExecuteAlways] // Чтобы работало в редакторе без запуска игры
public class TextBackgroundScaler : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent; // Или Text для Legacy
    [SerializeField] private Vector2 padding = new Vector2(20, 10); // Отступы

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (textComponent == null)
            textComponent = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if (textComponent == null) return;

        // Получаем размер текста
        Vector2 textSize = textComponent.GetPreferredValues();

        // Устанавливаем размер панели с учетом отступов
        _rectTransform.sizeDelta = new Vector2(
            textSize.x + padding.x,
            textSize.y + padding.y
        );
    }
}