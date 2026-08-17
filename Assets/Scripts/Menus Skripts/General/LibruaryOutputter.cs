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
    [SerializeField] private GameObject _wordPanel;
    [SerializeField] private GameObject _addWordPanel;
    [SerializeField] private GameObject _content;
    [SerializeField] private TMP_InputField _libruaryNameText;
    [SerializeField] private NewLibruaryButtonUI _newLibruaryButton;

    void Awake()
    {
        OpenLibruaryButtonUI.OnOpened += OutputLibrary;
        _newLibruaryButton.OnOpened += OutputLibrary;
    }

    private void OutputLibrary()
    {
        LibraryData library = StorageManager.GetLoadedLibrariesFromCache().FirstOrDefault() ?? new LibraryData();

        foreach (WordData word in library.Words)
        {
            GameObject wordButton = Instantiate(_wordPanel, _content.transform);
            wordButton.GetComponentInChildren<WordPanelUI>().Word = word;
        }

        if (_addWordPanel != null)
        {
            GameObject addWordPanel = Instantiate(_addWordPanel, _content.transform);
            GetComponent<WordAdder>().AddWordPanel = addWordPanel;
        }

        _libruaryNameText.text = library.LibraryName;
    }

    void OnDisable()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }
        _libruaryNameText.text = "";
        StorageManager.ClearCache();
        AudioManager.UnloadCurrentLibrary();
    }

    void OnDestroy()
    {
        OpenLibruaryButtonUI.OnOpened -= OutputLibrary;
    }
}
