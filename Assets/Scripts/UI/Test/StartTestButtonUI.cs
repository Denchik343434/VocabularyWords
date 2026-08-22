using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//кнопка запуска теста: сбор выбранных библиотек и загрузка в кэш
public class StartTestButtonUI : MonoBehaviour
{
    //контейнер со списком библиотек для выбора
    [SerializeField] private GameObject _content;
    //событие вызывается после загрузки библиотек
    public event Action OnStarted;

    //подписка на нажатие: сбор выбранных библиотек и запуск
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(async () =>
        {

            List<string> selectedLibraries = new List<string>();

           foreach (Transform panel in _content.transform)
            {
                LibraryPanelUI libraryPanelUI = panel.gameObject.GetComponent<LibraryPanelUI>();
                
                if(libraryPanelUI == null)
                    continue;

                if (libraryPanelUI.gameObject.GetComponentInChildren<Toggle>().isOn)
                    selectedLibraries.Add(libraryPanelUI.LibraryName);
            }

            if(selectedLibraries.Count() <= 0)
            return;

            UIBlocker.Block();
            await StorageManager.UnpackLibrariesToCache(selectedLibraries.ToArray());
            await AudioManager.RefreshAudioDictionaryAsync();
            OnStarted?.Invoke();
            UIBlocker.Unblock();
        });
    }
}
