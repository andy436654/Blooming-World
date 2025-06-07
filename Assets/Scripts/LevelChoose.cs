using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChoose : MonoBehaviour
{
    public void Change(int levelIndex)
    {
        LoadLevel(levelIndex);
    }

    private void LoadLevel(int levelIndex)
    {
        if (IsValidLevelIndex(levelIndex))
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogError($"Invalid level index: {levelIndex}", this);
        }
    }

    private bool IsValidLevelIndex(int levelIndex)
    {
        return levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings;
    }
}