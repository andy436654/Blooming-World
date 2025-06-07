using UnityEngine;

[System.Serializable]
public class UnlockCondition
{
    public enum ConditionType
    {
        PreviousLevelComplete,
        MaxBlockReached
    }

    public ConditionType type;
    public int requiredValue;
    public string customEventName;

    public bool IsMet()
    {
        switch (type)
        {
            case ConditionType.PreviousLevelComplete:
                return PlayerPrefs.GetInt("LastCompletedLevel", 0) >= requiredValue - 1;
            case ConditionType.MaxBlockReached:
                return PlayerPrefs.GetInt("MaxBlockReached", 0) == 1;

            default:
                return false;
        }
    }
}