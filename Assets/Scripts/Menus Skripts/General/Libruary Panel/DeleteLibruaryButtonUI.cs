using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DeleteLibruaryButtonUI : MonoBehaviour
{
    public static event Action OnDeleted;
    private string _libraryName;

        public string LibraryName
    {
        get { return _libraryName;}
        set 
        { 
            _libraryName = value;
        }
    }


    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {StorageManager.DeleteLibrary(_libraryName); OnDeleted?.Invoke();});
    }
}
