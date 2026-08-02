using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using TMPro;
public class LibruaryOutputter : MonoBehaviour
{
    [SerializeField] private GameObject _wordPanel;
    [SerializeField] private GameObject _addWordPanel;
    [SerializeField] private GameObject _content;
    [SerializeField] private TMP_InputField _libruaryNameText;
    private string _librariesFolderPath;

    void Awake()
    {
        _librariesFolderPath = Path.Combine(Application.persistentDataPath, "Libraries");
        OpenLibruaryButtonUI.OnOpened += OutputLibrary;
    }

    private void OutputLibrary(string libraryName)
    {
        LibraryData library = LoadLibrary(libraryName);

        foreach (WordData word in library.Words)
        {
            GameObject wordButton = Instantiate(_wordPanel, _content.transform);
            wordButton.GetComponentInChildren<WordEditPanelUI>().Word = word;
        }
        Instantiate(_addWordPanel, _content.transform);
        _libruaryNameText.text = library.LibraryName;
    }

    void OnDisable()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }
        _libruaryNameText.text = "";
    }

    void OnDestroy()
    {
        OpenLibruaryButtonUI.OnOpened -= OutputLibrary;
    }

    private LibraryData LoadLibrary(string libraryName)
    {
        string vclFilePath = Path.Combine( _librariesFolderPath, libraryName + ".vcl");

        string tempFolder = Path.Combine(Application.temporaryCachePath, "Unpacked_" + libraryName);

        // 1. Готовим чистую папку для распаковки
        if (Directory.Exists(tempFolder))
        {
            Directory.Delete(tempFolder, true);
        }

        // 2. Распаковываем .vcl
        ZipFile.ExtractToDirectory(vclFilePath, tempFolder);

        // 3. Читаем library.json из архива
        string jsonPath = Path.Combine(tempFolder, "library.json");
        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"[Storage] В архиве {libraryName}.vcl не найден library.json!");
            Directory.Delete(tempFolder, true);
            return null;
        }
        string jsonText = File.ReadAllText(jsonPath);
        LibraryData loadedLibrary = JsonUtility.FromJson<LibraryData>(jsonText);

        // 4. Чистим за собой временную папку
        Directory.Delete(tempFolder, true);

        return loadedLibrary;
    }
}
