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
    private string _librariesFolderPath;

    void Awake()
    {
        _librariesFolderPath = Path.Combine(Application.persistentDataPath, "Libraries");
    }

    void OnEnable()
    {
        _libraryNames = GetLibraryNames();

        foreach (string libraryName in _libraryNames)
        {
            GameObject libraryButton = Instantiate(_libruaryPanel, _content.transform);
            libraryButton.GetComponentInChildren<OpenLibruaryButtonUI>().LibraryName = libraryName;
        }
    }

    void OnDisable()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }
    }

        // Получить имена всех библиотек
    private List<string> GetLibraryNames()
    {
        List<string> libraryNames = new List<string>();
        string[] files = Directory.GetFiles(_librariesFolderPath, "*.vcl");

        foreach (string file in files)
        {
            libraryNames.Add(Path.GetFileNameWithoutExtension(file));
        }

        return libraryNames;
    }
}