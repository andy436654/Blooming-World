using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private Image exampleImage1;
    [SerializeField] private Image exampleImage2;
    [SerializeField] private Button skipButton;

    [SerializeField] private TutorialStep[] tutorialSteps;

    private int currentStepIndex = 0;
    private bool isTutorialActive = true;
    private Dictionary<string, bool> completionFlags = new Dictionary<string, bool>();

    public Toggle tutorialCheckbox;

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

        // Загружаем сохранённое состояние
        tutorialCheckbox.isOn = PlayerPrefs.GetInt("TutorialCompleted", 0) == 0;
        tutorialCheckbox.onValueChanged.AddListener(OnTutorialToggleChanged);
    }

    private void Start()
    {
        // Проверяем, был ли туториал завершён ИЛИ отключен через Toggle
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1 || !tutorialCheckbox.isOn)
        {
            tutorialPanel.SetActive(false);
            isTutorialActive = false;
            return;
        }

        //continueButton.onClick.AddListener(ContinueTutorial);
        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        if (currentStepIndex >= tutorialSteps.Length)
        {
            EndTutorial();
            return;
        }

        var currentStep = tutorialSteps[currentStepIndex];
        tutorialPanel.SetActive(true);
        instructionText.text = currentStep.instructionText;

        if (currentStep.exampleImage1 != null)
        {
            exampleImage1.sprite = currentStep.exampleImage1;
            exampleImage1.gameObject.SetActive(true);
        }
        else
        {
            exampleImage1.gameObject.SetActive(false);
        }

        if (currentStep.exampleImage2 != null)
        {
            exampleImage2.sprite = currentStep.exampleImage2;
            exampleImage2.gameObject.SetActive(true);
        }
        else
        {
            exampleImage2.gameObject.SetActive(false);
        }

        //continueButton.gameObject.SetActive(!currentStep.waitForUserAction);

        if (currentStep.autoContinueAfter > 0)
        {
            Invoke("ContinueTutorial", currentStep.autoContinueAfter);
        }
    }

    public void ContinueTutorial()
    {
        currentStepIndex++;
        ShowCurrentStep();
    }

    public void SkipTutorial() 
    {
        EndTutorial();
    }

    public void CompleteStep(string condition)
    {
        if (!isTutorialActive) return;

        if (currentStepIndex < tutorialSteps.Length &&
            tutorialSteps[currentStepIndex].completionCondition == condition)
        {
            ContinueTutorial();
        }
    }

    public void OnTutorialToggleChanged(bool isOn)
    {
        if (isOn)
        {
            // Если туториал включают, сбрасываем флаг завершения
            PlayerPrefs.SetInt("TutorialCompleted", 0);
        }
        else
        {
            // Если туториал выключают, устанавливаем флаг завершения
            PlayerPrefs.SetInt("TutorialCompleted", 1);
        }
    }

    private void EndTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.SetInt("TutorialCompleted", tutorialCheckbox.isOn ? 0 : 1);
        tutorialPanel.SetActive(false);
        isTutorialActive = false;
    }

    public bool IsTutorialActive()
    {
        return isTutorialActive;
    }
}