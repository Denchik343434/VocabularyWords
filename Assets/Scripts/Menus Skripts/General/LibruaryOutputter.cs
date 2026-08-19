using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using TMPro;
using System;
using System.Linq;

public class LibruaryOutputter : MonoBehaviour
{
    [SerializeField] private GameObject _wordPanelPrefab;
    [SerializeField] private GameObject _addWordPanel;
    [SerializeField] private GameObject _content;
    [SerializeField] private TMP_InputField _libruaryNameInputField;
    [SerializeField] private TextMeshProUGUI _libraryNameText;
    [SerializeField] private NewLibruaryButtonUI _newLibruaryButton;

    void Awake()
    {
        OpenLibruaryButtonUI.OnOpened += OutputLibrary;

        if(_newLibruaryButton != null)
        _newLibruaryButton.OnOpened += OutputLibrary;
    }

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

        if (_libruaryNameInputField != null)
        _libruaryNameInputField.text = library.LibraryName;

        if(_libraryNameText != null)
        _libraryNameText.text = library.LibraryName;
    }

    private void ClearMenu()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }

        if (_libruaryNameInputField != null)
        _libruaryNameInputField.text = "";

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
        OpenLibruaryButtonUI.OnOpened -= OutputLibrary;
    }
}
