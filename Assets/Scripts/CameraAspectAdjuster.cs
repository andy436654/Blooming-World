using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectAdjuster : MonoBehaviour
{
    [SerializeField] private float targetAspect = 16f / 9f;
    private Camera _camera;
    private int _lastWidth;
    private int _lastHeight;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;
    }

    private void Start()
    {
        AdjustCamera();
    }

    private void Update()
    {
        // Проверяем, изменилось ли разрешение или ориентация
        if (Screen.width != _lastWidth || Screen.height != _lastHeight)
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            AdjustCamera();
        }
    }

    private void AdjustCamera()
    {
        Application.targetFrameRate = 180;
        float currentAspect = (float)Screen.width / Screen.height;
        float scaleHeight = currentAspect / targetAspect;

        if (scaleHeight < 1f) // Экран уже, чем нужно
        {
            _camera.orthographicSize = _camera.orthographicSize / scaleHeight;
        }
        // Если экран шире — Orthographic Size не меняется
    }
}