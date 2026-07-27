using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageInitialiser : MonoBehaviour
{
    void Start()
    {
        StorageManager.InitializeStorage();
    }
}
