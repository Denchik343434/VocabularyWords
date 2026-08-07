using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression; 

public class LibruryOutputter : MonoBehaviour
{
    [SerializeField] private GameObject _libruaryPanel;
    [SerializeField] private GameObject _addLibruaryPanel;
    [SerializeField] private GameObject _content;
    private List<string> _libraryNames = new List<string>();

    void Awake()
    {
        DeleteLibruaryButtonUI.OnDeleted += UpdateLibraryList;
    }

    void OnEnable()
    {
        UpdateLibraryList();
    }

    void OnDisable()
    {
        DestroyAllLibraryButtons();
    }

    private void UpdateLibraryList()
    {
        DestroyAllLibraryButtons();

        _libraryNames = StorageManager.GetLibraryNames();

        foreach (string libraryName in _libraryNames)
        {
            GameObject libraryPanel = Instantiate(_libruaryPanel, _content.transform);
            libraryPanel.GetComponent<LibruaryPanelUI>().LibraryName = libraryName;
        }

            Instantiate(_addLibruaryPanel, _content.transform);
    }

    private void DestroyAllLibraryButtons()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }
    }
}