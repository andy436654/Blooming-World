using UnityEngine;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Step")]
public class TutorialStep : ScriptableObject
{
    [TextArea(3, 10)]
    public string instructionText;
    public Sprite exampleImage1;
    public Sprite exampleImage2;
    public string completionCondition; // Например: "SpawnBlock", "MergeBlocks"
    public bool waitForUserAction = true;
    public float autoContinueAfter = 0f; // Если 0 - ждем действия пользователя
}