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
    public float reductionAmount = 0.5f; // �� ������� ������ �������
    public float minAllowedWidth = 2f; // ����������� ������ ����

    private float currentMinX;
    private float currentMaxX;

    [Header("Walls Settings")]
    public GameObject leftBorder;
    public GameObject rightBorder;
    public GameObject dottedLine;
    public float dottedLineYPosition;

    [Header("Floor Settings")]
    public Transform floorTransform; // ������ �� ������ ����
    public float floorWidthOffset = 0.5f; // ������ ���� �� ����
    private Vector3 initialFloorScale;

    [Header("Dotted Line Settings")]
    public GameObject dottedLineSegmentPrefab; // ������ ����� �������
    public Transform dottedLineParent; // ������������ ������ ��� ���� ���������
    public float segmentSpacing = 0.5f; // ���������� ����� �������� ������
    public float lineYPosition = 2.0f; // Y-������� �����
    public float segmentWidth = 0.2f; // ������ ������ �������
    public float segmentHeight = 0.1f; // ������ ������ �������

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
        // ��������� ������� � ����� ������
        currentMinX += reductionAmount;
        currentMaxX -= reductionAmount;

        // ���������, ����� ���� �� ����� ������� �����
        if (currentMaxX - currentMinX < minAllowedWidth)
        {
            currentMinX = -minAllowedWidth / 2f;
            currentMaxX = minAllowedWidth / 2f;
        }

        UpdateVisualBounds();
    }

    private void UpdateDottedLine()
    {
        // ������� ������ ��������
        foreach (var segment in lineSegments)
        {
            if (segment != null) Destroy(segment);
        }
        lineSegments.Clear();

        // ������������ ����� ��������� �����
        float lineLength = currentMaxX - currentMinX;
        int segmentsCount = Mathf.FloorToInt(lineLength / segmentSpacing);

        // ���� ��� ����� �� ��� ������ �������� - �������
        if (segmentsCount <= 0) return;

        // ������������ ��������� �������, ����� ����� ���� ������������
        float startX = currentMinX + (lineLength - (segmentsCount - 1) * segmentSpacing) / 2f;

        // ������� ����� ��������
        for (int i = 0; i < segmentsCount; i++)
        {
            float xPos = startX + i * segmentSpacing;
            Vector3 position = new Vector3(xPos, lineYPosition, 0);
            Vector3 rotation = new Vector3(0, 0, 90); // ������� �� 90 ��������

            GameObject segment = Instantiate(
                dottedLineSegmentPrefab,
                position,
                Quaternion.Euler(rotation),
                dottedLineParent
            );

            // ������������ ������� ���� �����
            segment.transform.localScale = new Vector3(segmentWidth, segmentHeight, 1f);

            lineSegments.Add(segment);
        }
    }

    private void UpdateVisualBounds()
    {
        // ��������� ������� ������
        if (leftBorder != null)
        {
            leftBorder.transform.position = new Vector3(currentMinX, leftBorder.transform.position.y, 0);
            // ����� ����� �������������� �� ������, ���� �����
        }

        if (rightBorder != null)
        {
            rightBorder.transform.position = new Vector3(currentMaxX, rightBorder.transform.position.y, 0);
        }

        if (floorTransform != null)
        {
            // ����� ������ ������ (��� offset)
            float floorWidth = (currentMaxX - currentMinX);

            // ���� ����� ����������� ���������� �� �������, �������� ������������� ��������
            float visualPadding = 0.218f; // ��������� ���������� ������ (0.1-0.5)
            floorTransform.localScale = new Vector3(
                floorWidth * visualPadding,
                floorTransform.localScale.y,
                floorTransform.localScale.z
            );

            // �������������
            floorTransform.position = new Vector3(
                (currentMinX + currentMaxX) / 2f,
                floorTransform.position.y,
                floorTransform.position.z
            );
        }

        // ��������� ���������� �����
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