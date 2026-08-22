using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

//кнопка открытия библиотеки для редактирования
public class OpenLibraryButtonUI : MonoBehaviour
{
    //событие вызывается после загрузки библиотеки
    public static event Action OnOpened;
    //имя открываемой библиотеки
    private string _libraryName;

    //имя библиотеки для открытия
    public string LibraryName
    {
        get { return _libraryName;}
        set 
        { 
            _libraryName = value;
        }
    }

    //подписка на нажатие: распаковка библиотеки в кэш и загрузка аудио
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(async () =>
        {
            UIBlocker.Block();
            await StorageManager.UnpackLibrariesToCache(LibraryName);
            await AudioManager.RefreshAudioDictionaryAsync();
            OnOpened?.Invoke();
            UIBlocker.Unblock();
        });
    }
}
