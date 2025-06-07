using UnityEngine;
using TMPro;
using System.Collections;

public class ChangingFacts : MonoBehaviour
{
    public TMP_Text textDisplay; // Ссылка на текстовый компонент
    public string[] textMessages; // Массив сообщений для отображения
    public float changeInterval = 30f; // Интервал смены текста в секундах

    private int currentIndex = 0;

    void Start()
    {
        if (textDisplay == null)
        {
            // Попробуем найти текстовый компонент на этом же объекте
            textDisplay = GetComponent<TMP_Text>();
        }

        if (textMessages == null || textMessages.Length == 0)
        {
            Debug.LogError("Не заданы сообщения для отображения!");
            return;
        }

        // Устанавливаем начальный текст
        if (textDisplay != null)
        {
            textDisplay.text = textMessages[currentIndex];
        }

        // Запускаем корутину для смены текста
        StartCoroutine(ChangeTextRoutine());
    }

    IEnumerator ChangeTextRoutine()
    {
        while (true)
        {
            // Ждем указанный интервал
            yield return new WaitForSeconds(changeInterval);

            // Переходим к следующему сообщению
            currentIndex = Random.Range(0,textMessages.Length-1);

            // Обновляем текст
            if (textDisplay != null)
            {
                textDisplay.text = textMessages[currentIndex];
            }
        }
    }
}