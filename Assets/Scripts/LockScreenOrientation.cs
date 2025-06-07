using UnityEngine;

[DisallowMultipleComponent] // Запрещаем дублирование компонента
public class LockScreenOrientation : MonoBehaviour
{
    [SerializeField]
    private ScreenOrientation _orientation = ScreenOrientation.Portrait;

    private void Awake()
    {
        Application.targetFrameRate = 180;
        ApplyOrientationSettings();
    }

    private void ApplyOrientationSettings()
    {
        Screen.autorotateToPortrait = IsPortrait(_orientation);
        Screen.autorotateToLandscapeLeft = _orientation == ScreenOrientation.LandscapeLeft;
        Screen.autorotateToLandscapeRight = _orientation == ScreenOrientation.LandscapeRight;

        Screen.orientation = _orientation;
    }

    private bool IsPortrait(ScreenOrientation orientation)
    {
        return orientation == ScreenOrientation.Portrait ||
               orientation == ScreenOrientation.PortraitUpsideDown;
    }
}