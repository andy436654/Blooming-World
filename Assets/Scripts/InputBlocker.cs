using UnityEngine;

public static class InputBlocker
{
    private static bool _isInputBlocked = false;
    private static float _blockUntilTime = 0f;

    public static bool IsInputBlocked()
    {
        return _isInputBlocked && Time.time < _blockUntilTime;
    }

    public static void BlockInputForSeconds(float seconds)
    {
        _isInputBlocked = true;
        _blockUntilTime = Time.time + seconds;
    }
}