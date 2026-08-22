using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using TMPro;
using System;
using System.Linq;

//скрипт вывода содержимого библиотеки (списка слов) в меню редактирования
public class LibraryOutputter : MonoBehaviour
{
    //префаб панели слова
    [SerializeField] private GameObject _wordPanelPrefab;
    //префаб кнопки добавления нового слова
    [SerializeField] private GameObject _addWordPanel;
    //контейнер для списка слов
    [SerializeField] private GameObject _content;
    //поле ввода имени библиотеки
    [SerializeField] private TMP_InputField _libraryNameInputField;
    //текст с именем библиотеки
    [SerializeField] private TextMeshProUGUI _libraryNameText;
    //кнопка создания новой библиотеки
    [SerializeField] private NewLibraryButtonUI _newLibraryButton;

    //подписка на события открытия библиотеки
    void Awake()
    {
        OpenLibraryButtonUI.OnOpened += OutputLibrary;

        if(_newLibraryButton != null)
        _newLibraryButton.OnOpened += OutputLibrary;
    }

    //загрузка и отображение слов из кэша
    private void OutputLibrary()
    {
        ClearMenu();

        LibraryData library = StorageManager.GetLoadedLibrariesFromCache().FirstOrDefault() ?? new LibraryData();

        foreach (WordData word in library.Words)
        {
            GameObject wordPanel = Instantiate(_wordPanelPrefab, _content.transform);
            wordPanel.GetComponentInChildren<WordPanelUI>().Word = word;
        }

        if (_addWordPanel != null)
        {
            GameObject addWordPanel = Instantiate(_addWordPanel, _content.transform);
            GetComponent<WordAdder>().AddWordPanel = addWordPanel;
        }

        if (_libraryNameInputField != null)
        _libraryNameInputField.text = library.LibraryName;

        if(_libraryNameText != null)
        _libraryNameText.text = library.LibraryName;
    }

    //очистка всех слов и полей из меню
    private void ClearMenu()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }

        if (_libraryNameInputField != null)
        _libraryNameInputField.text = "";

        if (_libraryNameText != null)
        _libraryNameText.text = "";

    }

    void OnDisable()
    {
        ClearMenu();
        StorageManager.ClearLibraryData();
    }

    void OnDestroy()
    {
        OpenLibraryButtonUI.OnOpened -= OutputLibrary;
    }
}
