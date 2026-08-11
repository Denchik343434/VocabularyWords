using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LibruarySaver : MonoBehaviour
{
    [SerializeField] private GameObject _loadingIcon;
    [SerializeField] private GameObject _savedIcon;
    [SerializeField] private GameObject _unsavedIcon;
    [SerializeField] private InputfieldTMPText _libruaryNameText;
    [SerializeField] private Button _saveButton;
    [SerializeField] private GameObject _content;
    private bool _isSavingPosible = false;

    void Start()
    {
        WordEditPanelUI.OnWordChanget += OnLibruaryChanged;
        _libruaryNameText.OnTextChanged += OnLibruaryChanged;
        _saveButton.onClick.AddListener(TrySaveLibruary);
        GetComponent<SaveWarning>().OnSaveWarningChanged += (isValid) => _isSavingPosible = isValid;
    }

    private void OnLibruaryChanged()
    {
        _unsavedIcon.SetActive(true);
        _savedIcon.SetActive(false);
    }

    private void TrySaveLibruary()
    {
        if (!_isSavingPosible)
        {
            Debug.Log("Отказано");
            return;
        }

        _loadingIcon.SetActive(true);
        _unsavedIcon.SetActive(false);
        _savedIcon.SetActive(false);

        try
        {
            SaveLibrary();
            
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
        LibraryData library = CreateLibruary();
        string lastName = StorageManager.GetLoadedLibrariesFromCache().FirstOrDefault()?.LibraryName;

        StorageManager.SaveJsonToCache(library);

        if (lastName != null && lastName != library.LibraryName)
        {
            StorageManager.DeleteJsonFromCache(lastName);
        }

        StorageManager.SaveLibrary(library.LibraryName);

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
