using UnityEngine;

//скрипт инициализации приложения: настройка окна и хранилища
public class AppManager : MonoBehaviour
{
    //настройки окна при запуске
    [SerializeField] private int _targetFrameRate = 60;
    [SerializeField] private int _defaultWidth = 1280;
    [SerializeField] private int _defaultHeight = 720;

    //инициализация хранилища, аудио и настройка окна
    void Awake()
    {
        StorageManager.InitializeStorage();
        StorageManager.ClearCache();
        AudioManager.EnsureSource();

        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = _targetFrameRate; 
        
        Screen.SetResolution(_defaultWidth, _defaultHeight, FullScreenMode.Windowed);
    }
}
