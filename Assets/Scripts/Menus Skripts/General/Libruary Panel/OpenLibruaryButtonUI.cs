using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class OpenLibruaryButtonUI : MonoBehaviour
{
    public static event Action OnOpened;
    private string _libraryName;
    [SerializeField] private TMPro.TextMeshProUGUI _libraryNameText;
    public string LibraryName
    {
        get { return _libraryName;}
        set 
        { 
            _libraryName = value;
            _libraryNameText.text = value;
        }
    }

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
