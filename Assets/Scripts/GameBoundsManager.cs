using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameBoundsManager : MonoBehaviour
{
    public static GameBoundsManager Instance;

    [Header("Initial Bounds")]
    public float initialMinX = -5f;
    public float initialMaxX = 5f;

    [Header("Bounds Reduction")]
    public float reductionAmount = 0.5f; // На сколько сужаем границы
    public float minAllowedWidth = 2f; // Минимальная ширина поля

    private float currentMinX;
    private float currentMaxX;

    [Header("Walls Settings")]
    public GameObject leftBorder;
    public GameObject rightBorder;
    public GameObject dottedLine;
    public float dottedLineYPosition;

    [Header("Floor Settings")]
    public Transform floorTransform; // Ссылка на объект пола
    public float floorWidthOffset = 0.5f; // Отступ пола от стен
    private Vector3 initialFloorScale;

    [Header("Dotted Line Settings")]
    public GameObject dottedLineSegmentPrefab; // Префаб одной капсулы
    public Transform dottedLineParent; // Родительский объект для всех сегментов
    public float segmentSpacing = 0.5f; // Расстояние между центрами капсул
    public float lineYPosition = 2.0f; // Y-позиция линии
    public float segmentWidth = 0.2f; // Ширина каждой капсулы
    public float segmentHeight = 0.1f; // Высота каждой капсулы

    public static int maxBlocksAmount;
    public TMP_Text maxBlocksText;

    private List<GameObject> lineSegments = new List<GameObject>();

    private void Awake()
    {
        maxBlocksAmount = 0;
        Instance = this;
        ResetBounds();
    }

    private void Update()
    {
        maxBlocksText.text = " - " + maxBlocksAmount.ToString();
    }

    public void ResetBounds()
    {
        currentMinX = initialMinX;
        currentMaxX = initialMaxX;
        UpdateVisualBounds();
    }

    public void ReduceBounds()
    {
        // Уменьшаем границы с обеих сторон
        currentMinX += reductionAmount;
        currentMaxX -= reductionAmount;

        // Проверяем, чтобы поле не стало слишком узким
        if (currentMaxX - currentMinX < minAllowedWidth)
        {
            currentMinX = -minAllowedWidth / 2f;
            currentMaxX = minAllowedWidth / 2f;
        }

        UpdateVisualBounds();
    }

    private void UpdateDottedLine()
    {
        // Очищаем старые сегменты
        foreach (var segment in lineSegments)
        {
            if (segment != null) Destroy(segment);
        }
        lineSegments.Clear();

        // Рассчитываем новые параметры линии
        float lineLength = currentMaxX - currentMinX;
        int segmentsCount = Mathf.FloorToInt(lineLength / segmentSpacing);

        // Если нет места ни для одного сегмента - выходим
        if (segmentsCount <= 0) return;

        // Рассчитываем начальную позицию, чтобы линия была центрирована
        float startX = currentMinX + (lineLength - (segmentsCount - 1) * segmentSpacing) / 2f;

        // Создаем новые сегменты
        for (int i = 0; i < segmentsCount; i++)
        {
            float xPos = startX + i * segmentSpacing;
            Vector3 position = new Vector3(xPos, lineYPosition, 0);
            Vector3 rotation = new Vector3(0, 0, 90); // Поворот на 90 градусов

            GameObject segment = Instantiate(
                dottedLineSegmentPrefab,
                position,
                Quaternion.Euler(rotation),
                dottedLineParent
            );

            // Масштабируем сегмент если нужно
            segment.transform.localScale = new Vector3(segmentWidth, segmentHeight, 1f);

            lineSegments.Add(segment);
        }
    }

    private void UpdateVisualBounds()
    {
        // Обновляем позиции границ
        if (leftBorder != null)
        {
            leftBorder.transform.position = new Vector3(currentMinX, leftBorder.transform.position.y, 0);
            // Можно также масштабировать по высоте, если нужно
        }

        if (rightBorder != null)
        {
            rightBorder.transform.position = new Vector3(currentMaxX, rightBorder.transform.position.y, 0);
        }

        if (floorTransform != null)
        {
            // Новый расчет ширины (без offset)
            float floorWidth = (currentMaxX - currentMinX);

            // Если нужно минимальное расширение за границы, добавьте фиксированное значение
            float visualPadding = 0.218f; // Небольшой визуальный отступ (0.1-0.5)
            floorTransform.localScale = new Vector3(
                floorWidth * visualPadding,
                floorTransform.localScale.y,
                floorTransform.localScale.z
            );

            // Центрирование
            floorTransform.position = new Vector3(
                (currentMinX + currentMaxX) / 2f,
                floorTransform.position.y,
                floorTransform.position.z
            );
        }

        // Обновляем пунктирную линию
        if (dottedLine != null)
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = new Vector3(currentMinX, dottedLineYPosition, 0);
            positions[1] = new Vector3(currentMaxX, dottedLineYPosition, 0);
            //dottedLine.SetPositions(positions);
        }

        UpdateDottedLine();
    }


    public float GetMinX() => currentMinX;
    public float GetMaxX() => currentMaxX;
}