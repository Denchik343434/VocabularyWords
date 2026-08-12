using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

public class LibruarySaver : MonoBehaviour
{
    [SerializeField] private GameObject _loadingIcon;
    [SerializeField] private GameObject _savedIcon;
    [SerializeField] private GameObject _unsavedIcon;
    [SerializeField] private InputfieldTMPText _libruaryNameText;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _saveToFolderButton;
    [SerializeField] private GameObject _content;
    private bool _isSavingPosible = false;

    void Start()
    {
        WordEditPanelUI.OnWordChanget += OnLibruaryChanged;
        _libruaryNameText.OnTextChanged += OnLibruaryChanged;

        _saveButton.onClick.AddListener(() => 
        {
            if (_isSavingPosible)
            {
                TrySaveLibruary();
            }
            else
            Debug.Log("отказано");
        });

        _saveToFolderButton.onClick.AddListener(() =>
        {
            if (_isSavingPosible)
            {
                string SaveFolderPath = StorageManager.GetUserPath(StorageFilterType.Folder);
                if (SaveFolderPath != null)
                {
                    TrySaveLibruary(SaveFolderPath);
                }
            }
            else
            Debug.Log("отказано");
        });

        GetComponent<SaveWarning>().OnSaveWarningChanged += (isValid) => _isSavingPosible = isValid;
    }

    private void OnLibruaryChanged()
    {
        _unsavedIcon.SetActive(true);
        _savedIcon.SetActive(false);
    }

    private void TrySaveLibruary()
    {
        TrySaveLibruary(default);
    }

    private void TrySaveLibruary(string saveFolderPath = null)
    {
        _loadingIcon.SetActive(true);
        _unsavedIcon.SetActive(false);
        _savedIcon.SetActive(false);

        try
        {
            SaveLibrary(saveFolderPath);
            
            _savedIcon.SetActive(true);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при сохранении: {ex.Message}");
            _unsavedIcon.SetActive(true);
        }
        finally
        {
            _loadingIcon.SetActive(false);
        }
    }


    void OnDestroy()
    {
        WordEditPanelUI.OnWordChanget -= OnLibruaryChanged;
    }

    private void SaveLibrary()
    {
        SaveLibrary(default);
    }

    private void SaveLibrary(string saveFolderPath = null)
    {
        LibraryData library = CreateLibruary();
        string lastName = StorageManager.GetLoadedLibrariesFromCache().FirstOrDefault()?.LibraryName;

        StorageManager.SaveJsonToCache(library);

        if (lastName != null && lastName != library.LibraryName)
        {
            StorageManager.DeleteJsonFromCache(lastName);
        }

        StorageManager.SaveLibrary(saveFolderPath);

        if (lastName != null && lastName != library.LibraryName)
        {
            StorageManager.DeleteLibrary(lastName);
        }

    }
    private LibraryData CreateLibruary()
    {
        List<WordData> words = new List<WordData>();

        foreach (Transform child in _content.transform)
        {
            if(child.TryGetComponent<WordEditPanelUI>(out var wordEditPanel))
            {
                words.Add(wordEditPanel.Word);
            }
        }

        LibraryData library = new LibraryData
        {
            LibraryName = _libruaryNameText.Text,
            Words = words
        };
        return library;
    }
}
