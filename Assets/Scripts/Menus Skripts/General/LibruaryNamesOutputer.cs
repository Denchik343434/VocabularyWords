using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using UnityEngine.UI;
using TMPro;

public class LibruaryNamesOutputter : MonoBehaviour
{
    [SerializeField] private GameObject _libruaryPanel;
    [SerializeField] private GameObject _addLibruaryButtonPrefab;
    private Button _addLibruaryButton = null; 
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

        if(_addLibruaryButton != null)
            _addLibruaryButton.onClick.RemoveAllListeners();

        _addLibruaryButton = Instantiate(_addLibruaryButtonPrefab, _content.transform).GetComponent<Button>();

        _addLibruaryButton.onClick.AddListener(() => 
        {
            Debug.Log("оно тыкаеться");
            string targetPath = StorageManager.GetUserPath(StorageFilterType.Library);

            if (targetPath != null)
                StorageManager.AddLibrary(targetPath);
            else
                return;
            UpdateLibraryList();
        });
    }

    private void DestroyAllLibraryButtons()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }
    }
}