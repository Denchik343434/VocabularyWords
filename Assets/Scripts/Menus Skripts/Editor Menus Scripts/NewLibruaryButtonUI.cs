using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NewLibruaryButtonUI : MonoBehaviour
{
    public event Action OnOpened;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {StorageManager.EnsureCacheDirectoriesExist(); OnOpened?.Invoke();});
    }
}
