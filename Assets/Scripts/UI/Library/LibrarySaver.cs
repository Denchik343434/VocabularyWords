using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

//скрипт сохранения библиотеки в файл
public class LibrarySaver : MonoBehaviour
{
    //иконка успешного сохранения
    [SerializeField] private GameObject _savedIcon;
    //иконка несохранённых изменений
    [SerializeField] private GameObject _unsavedIcon;
    //иконка предупреждения о невозможности сохранения
    [SerializeField] private GameObject _saveWarningIcon;
    //поле ввода имени библиотеки
    [SerializeField] private TMP_InputField _libraryNameText;
    //кнопка сохранения
    [SerializeField] private Button _saveButton;
    //кнопка сохранения в выбранную папку
    [SerializeField] private Button _saveToFolderButton;
    //контейнер со списком слов
    [SerializeField] private GameObject _content;
    //флаг возможности сохранения
    private bool _isSavingPossible = false;
    //источник отмены для асинхронного сохранения
    private CancellationTokenSource _saveCts;

    //подписка на события изменения слов и имени, настройка кнопок
    void Start()
    {
        WordEditPanelUI.OnValuesChanged += OnLibraryChanged;
        _libraryNameText.onEndEdit.AddListener(input => 
        {
            _libraryNameText.text = InputDefender.ToMaxCorrect(input);
            OnLibraryChanged();
        });

        _saveButton.onClick.AddListener(() => 
        {
            if (_isSavingPossible)
            {
                TrySaveLibrary();
            }
        });

        _saveToFolderButton.onClick.AddListener(() =>
        {
            if (_isSavingPossible)
            {
                string SaveFolderPath = StorageManager.GetUserPath(StorageFilterType.Folder);
                if (SaveFolderPath != null)
                {
                    TrySaveLibrary(SaveFolderPath);
                }
            }
        });
    }

    //обработка изменения данных библиотеки
    private void OnLibraryChanged()
    {
        _unsavedIcon.SetActive(true);
        _savedIcon.SetActive(false);
        _isSavingPossible = UpdateValidate();
    }

    //запуск сохранения в основное хранилище
    private void TrySaveLibrary()
    {
        TrySaveLibrary(default);
    }

    //асинхронное сохранение библиотеки с блокировкой интерфейса
    private async void TrySaveLibrary(string saveFolderPath)
    {
        _saveCts?.Cancel();
        _saveCts = new CancellationTokenSource();
        var token = _saveCts.Token;

        UIBlocker.Block();
        _unsavedIcon.SetActive(false);
        _savedIcon.SetActive(false);

        try
        {
            await SaveLibraryAsync(saveFolderPath, token);
            
            _savedIcon.SetActive(true);
        }
        catch (OperationCanceledException)
        {
            _unsavedIcon.SetActive(true);
        }
        catch (Exception ex)
        {
            _unsavedIcon.SetActive(true);
            Debug.Log(ex);
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
        WordEditPanelUI.OnValuesChanged -= OnLibraryChanged;
    }

    //формирование данных библиотеки и запись в файл
    private async Task SaveLibraryAsync(string saveFolderPath, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        LibraryData library = CreateLibrary();
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
    //сбор данных из панелей слов в объект библиотеки
    private LibraryData CreateLibrary()
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
            LibraryName = _libraryNameText.text,
            Words = words
        };
        return library;
    }

    //проверка заполненности имени и слов для возможности сохранения
    private bool UpdateValidate()
    {
        bool IsValid = true;
        
        if (string.IsNullOrWhiteSpace(_libraryNameText.text))
        {
            IsValid = false;
        }

        if (IsValid)
        {

            foreach (Transform panel in _content.transform)
            {
                if (panel.TryGetComponent<WordEditPanelUI>(out var wordPanel))
                {
                    if (wordPanel.IsEmpty || _content.transform.childCount == 0)
                    {
                        IsValid = false;
                    }
                }
            }
        }

        _saveWarningIcon.SetActive(!IsValid);
        return IsValid;
    }

    void OnEnable()
    {
        UpdateValidate();
    }
}
