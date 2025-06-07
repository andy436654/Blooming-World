using UnityEngine;
using UnityEngine.UI;
using TMPro; // ���� ����������� TextMeshPro

[RequireComponent(typeof(Image))]
[ExecuteAlways] // ����� �������� � ��������� ��� ������� ����
public class TextBackgroundScaler : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent; // ��� Text ��� Legacy
    [SerializeField] private Vector2 padding = new Vector2(20, 10); // �������

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

        // �������� ������ ������
        Vector2 textSize = textComponent.GetPreferredValues();

        // ������������� ������ ������ � ������ ��������
        _rectTransform.sizeDelta = new Vector2(
            textSize.x + padding.x,
            textSize.y + padding.y
        );
    }
}