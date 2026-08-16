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
    [SerializeField] private GameObject _savedIcon;
    [SerializeField] private GameObject _unsavedIcon;
    [SerializeField] private InputfieldTMPText _libruaryNameText;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _saveToFolderButton;
    [SerializeField] private GameObject _content;
    private bool _isSavingPosible = false;
    private CancellationTokenSource _saveCts;

    void Start()
    {
        WordPanelUI.OnValuesChanged += OnLibruaryChanged;
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

    private async void TrySaveLibruary(string saveFolderPath)
    {
        _saveCts?.Cancel();
        _saveCts = new CancellationTokenSource();
        var token = _saveCts.Token;

        UIBlocker.Block();
        _unsavedIcon.SetActive(false);
        _savedIcon.SetActive(false);

        try
        {
            Debug.Log("Сохранение начато");
            await SaveLibraryAsync(saveFolderPath, token);
            
            _savedIcon.SetActive(true);
            Debug.Log("Сохранение завершено");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Сохранение отменено");
            _unsavedIcon.SetActive(true);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при сохранении: {ex.Message}");
            _unsavedIcon.SetActive(true);
        }
        finally
        {
            UIBlocker.Unblock();
            _saveCts?.Dispose();
            _saveCts = null;
        }
    }


    void OnDestroy()
    {
        WordPanelUI.OnValuesChanged -= OnLibruaryChanged;
    }

    private async Task SaveLibraryAsync(string saveFolderPath, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        LibraryData library = CreateLibruary();
        string lastName = StorageManager.GetLoadedLibrariesFromCache().FirstOrDefault()?.LibraryName;

        StorageManager.SaveJsonToCache(library);

        if (lastName != null && lastName != library.LibraryName)
        {
            StorageManager.DeleteJsonFromCache(lastName);
        }

        await StorageManager.SaveLibraryAsync(saveFolderPath);

        token.ThrowIfCancellationRequested();

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
            if(child.TryGetComponent<WordPanelUI>(out var wordEditPanel))
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
