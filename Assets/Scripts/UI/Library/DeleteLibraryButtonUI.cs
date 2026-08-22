using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

//кнопка удаления библиотеки
public class DeleteLibraryButtonUI : MonoBehaviour
{
    //событие вызывается после удаления библиотеки
    public static event Action OnDeleted;
    //имя удаляемой библиотеки
    private string _libraryName;

    //имя библиотеки для удаления
    public string LibraryName
    {
        get { return _libraryName;}
        set 
        { 
            _libraryName = value;
        }
    }

    //подписка на нажатие кнопки удаления
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {StorageManager.DeleteLibrary(_libraryName); OnDeleted?.Invoke();});
    }
}
