using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    public static bool IsLevelUnlocked(int levelNumber)
    {
        // Первый уровень всегда разблокирован
        if (levelNumber == 1) return true;

        // Проверяем прохождение предыдущего уровня
        return PlayerPrefs.GetInt($"Level_{levelNumber - 1}_Completed", 0) == 1;
    }

    public static int GetHighestUnlockedLevel()
    {
        int level = 1;
        while (IsLevelUnlocked(level + 1))
        {
            level++;
        }
        return level;
    }
}