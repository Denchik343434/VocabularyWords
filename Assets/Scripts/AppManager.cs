using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //  отключение вертикальной синхронизации,
    //  установка целевой частоты кадров,
    //  переведение в оконный режим,
    //  установка минимальной ширины окна

public class AppManager : MonoBehaviour
{
    [SerializeField] private int _targetFrameRate = 60;
    [SerializeField] private int _minWidth = 1000;
    [SerializeField] private int _minHeight = 600;
    [SerializeField] private int _defaultWidth = 1280;
    [SerializeField] private int _defaultHeight = 720;

    void Awake()
    {
        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = _targetFrameRate; 
        Screen.SetResolution(_defaultWidth, _defaultHeight, false);
    }

    void Update()
{
    if (Screen.width < _minWidth || Screen.height < _minHeight)
    {
        int newWidth = Mathf.Max(Screen.width, _minWidth);
        int newHeight = Mathf.Max(Screen.height, _minHeight);

        Screen.SetResolution(newWidth, newHeight, false);
    }
}
}
