using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression; 


    // иниациализация хранилище,
    //  отключение вертикальной синхронизации,
    //  установка целевой частоты кадров,
    //  переведение в оконный режим,
    //  установка минимальной ширины высоты окна

public class AppManager : MonoBehaviour
{
    [SerializeField] private int _targetFrameRate = 60;
    [SerializeField] private int _defaultWidth = 1280;
    [SerializeField] private int _defaultHeight = 720;
    [SerializeField] private int _minWidth = 600;
    [SerializeField] private int _minHeight = 450;
    [SerializeField] private float _minAspect = 4f / 3f;
    [SerializeField] private float _maxAspect = 16f / 9f;

    private string _librariesFolderPath;

    private int _lastWidth;
    private int _lastHeight;

    void Awake()
    {
        _librariesFolderPath = Path.Combine(Application.persistentDataPath, "Libraries");
        InitializeStorage();

        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = _targetFrameRate; 
        
        Screen.SetResolution(_defaultWidth, _defaultHeight, FullScreenMode.Windowed);

        _lastWidth = _defaultWidth;
        _lastHeight = _defaultHeight;
    }

    void Update()
    {
        if (Screen.fullScreen) return;

        int currentWidth = Screen.width;
        int currentHeight = Screen.height;
        
        if (currentWidth == _lastWidth && currentHeight == _lastHeight) return;

        int targetWidth = currentWidth;
        int targetHeight = currentHeight;

        if (targetWidth < _minWidth) targetWidth = _minWidth;
        if (targetHeight < _minHeight) targetHeight = _minHeight;

        float currentAspect = (float)targetWidth / targetHeight;

        if (currentAspect > _maxAspect)
        {
            targetWidth = Mathf.RoundToInt(targetHeight * _maxAspect);
        }
        else if (currentAspect < _minAspect)
        {
            targetWidth = Mathf.RoundToInt(targetHeight * _minAspect);
        }

        if (targetWidth != currentWidth || targetHeight != currentHeight)
        {
            Screen.SetResolution(targetWidth, targetHeight, FullScreenMode.Windowed);
            _lastWidth = targetWidth;
            _lastHeight = targetHeight;
        }
        else
        {
            _lastWidth = currentWidth;
            _lastHeight = currentHeight;
        }
    }

        // Проверяет наличие папки и создает её при необходимости
    private void InitializeStorage()
    {
        if (!Directory.Exists(_librariesFolderPath))
        {
            Directory.CreateDirectory(_librariesFolderPath);
            Debug.Log($"[Storage] Папка создана по пути: {_librariesFolderPath}");
        }
    }
}