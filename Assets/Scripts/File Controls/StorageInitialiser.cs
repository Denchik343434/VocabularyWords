using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageInitialiser : MonoBehaviour
{
    void Awake()
    {
        StorageManager.InitializeStorage();
    }
}
