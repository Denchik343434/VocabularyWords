using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using UnityEngine.UI;
using TMPro;

//скрипт вывода списка всех библиотек в главном меню
public class LibraryNamesOutputter : MonoBehaviour
{
    //префаб панели библиотеки
    [SerializeField] private GameObject _libraryPanel;
    //префаб кнопки добавления из хранилища
    [SerializeField] private GameObject _addLibraryButtonPrefab;
    //текущая кнопка добавления
    private Button _addLibraryButton = null; 
    //контейнер для списка библиотек
    [SerializeField] private GameObject _content;
    //список загруженных имён библиотек
    private List<string> _libraryNames = new List<string>();

    //подписка на событие удаления библиотеки
    void Awake()
    {
        DeleteLibraryButtonUI.OnDeleted += UpdateLibraryList;
    }

    //обновление списка при открытии меню
    void OnEnable()
    {
        UpdateLibraryList();
    }

    //очистка списка при закрытии меню
    void OnDisable()
    {
        DestroyAllLibraryButtons();
    }

    //пересоздание списка библиотек и кнопки добавления
    private void UpdateLibraryList()
    {
        DestroyAllLibraryButtons();

        _libraryNames = StorageManager.GetLibraryNames();

        foreach (string libraryName in _libraryNames)
        {
            GameObject libraryPanel = Instantiate(_libraryPanel, _content.transform);
            libraryPanel.GetComponent<LibraryPanelUI>().LibraryName = libraryName;
        }

        if(_addLibraryButton != null)
            _addLibraryButton.onClick.RemoveAllListeners();

        _addLibraryButton = Instantiate(_addLibraryButtonPrefab, _content.transform).GetComponent<Button>();

        _addLibraryButton.onClick.AddListener(() => 
        {
            string targetPath = StorageManager.GetUserPath(StorageFilterType.Library);

            if (targetPath != null)
                StorageManager.AddLibrary(targetPath);
            else
                return;
            UpdateLibraryList();
        });
    }

    //удаление всех кнопок из контейнера
    private void DestroyAllLibraryButtons()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }
    }
}