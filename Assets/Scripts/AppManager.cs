using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField] private int _targetFrameRate = 60;
    [SerializeField] private int _defaultWidth = 1280;
    [SerializeField] private int _defaultHeight = 720;
    [SerializeField] private int _minWidth = 600;
    [SerializeField] private int _minHeight = 450;
    //[SerializeField] private float _minAspect = 4f / 3f;
    //[SerializeField] private float _maxAspect = 16f / 9f;

    //private int _lastWidth;
    //private int _lastHeight;

    void Awake()
    {
        StorageManager.InitializeStorage();
        StorageManager.ClearCache();
        AudioManager.EnsureSource();

        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = _targetFrameRate; 
        
        Screen.SetResolution(_defaultWidth, _defaultHeight, FullScreenMode.Windowed);

        //_lastWidth = _defaultWidth;
        //_lastHeight = _defaultHeight;
    }

    void Update()
    {
        //ControlWindouSize();
    }

    /*
    private void ControlWindouSize()
    {
        if (Screen.fullScreen) return;

        int currentWidth = Screen.width;
        int currentHeight = Screen.height;

        if (currentWidth == _lastWidth && currentHeight == _lastHeight) return;

        // Если окно стало МЕНЬШЕ минимального порога — корректируем
        bool needsResize = false;
        int targetWidth = currentWidth;
        int targetHeight = currentHeight;

        if (targetWidth < _minWidth) 
        {
            targetWidth = _minWidth;
            needsResize = true;
        }
        if (targetHeight < _minHeight) 
        {
            targetHeight = _minHeight;
            needsResize = true;
        }

        float currentAspect = (float)targetWidth / targetHeight;

        if (currentAspect > _maxAspect)
        {
            targetWidth = Mathf.RoundToInt(targetHeight * _maxAspect);
            needsResize = true;
        }
        else if (currentAspect < _minAspect)
        {
            targetWidth = Mathf.RoundToInt(targetHeight * _minAspect);
            needsResize = true;
        }
        if (needsResize)
        {
            Screen.SetResolution(targetWidth, targetHeight, FullScreenMode.Windowed);
        }

        _lastWidth = targetWidth;
        _lastHeight = targetHeight;
    }
    */

    /*
    private void ControlWindouSize()
    {
        if (Screen.width < _minWidth || Screen.height < _minHeight)
        {
            Screen.SetResolution(_minWidth, _minHeight, FullScreenMode.Windowed);
        }
    }
    */
}