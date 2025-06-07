#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class PlayerPrefsCleaner : EditorWindow
{
    [MenuItem("Window/Clear PlayerPrefs")]
    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs очищены!");
    }
}
#endif