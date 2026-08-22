using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//кнопка создания новой пустой библиотеки
public class NewLibraryButtonUI : MonoBehaviour
{
    //событие вызывается после создания папки кэша
    public event Action OnOpened;

    //подписка на нажатие: создание кэша и вызов события
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {StorageManager.EnsureCacheDirectoriesExist(); OnOpened?.Invoke();});
    }
}
