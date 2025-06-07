using UnityEngine;
using TMPro;
using System.Collections;

public class ChangingFacts : MonoBehaviour
{
    public TMP_Text textDisplay; // ������ �� ��������� ���������
    public string[] textMessages; // ������ ��������� ��� �����������
    public float changeInterval = 30f; // �������� ����� ������ � ��������

    private int currentIndex = 0;

    void Start()
    {
        if (textDisplay == null)
        {
            // ��������� ����� ��������� ��������� �� ���� �� �������
            textDisplay = GetComponent<TMP_Text>();
        }

        if (textMessages == null || textMessages.Length == 0)
        {
            Debug.LogError("�� ������ ��������� ��� �����������!");
            return;
        }

        // ������������� ��������� �����
        if (textDisplay != null)
        {
            textDisplay.text = textMessages[currentIndex];
        }

        // ��������� �������� ��� ����� ������
        StartCoroutine(ChangeTextRoutine());
    }

    IEnumerator ChangeTextRoutine()
    {
        while (true)
        {
            // ���� ��������� ��������
            yield return new WaitForSeconds(changeInterval);

            // ��������� � ���������� ���������
            currentIndex = Random.Range(0,textMessages.Length-1);

            // ��������� �����
            if (textDisplay != null)
            {
                textDisplay.text = textMessages[currentIndex];
            }
        }
    }
}