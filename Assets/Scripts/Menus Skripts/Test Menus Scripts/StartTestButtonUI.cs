using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartTestButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    public event Action OnStarted;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(async () =>
        {

            List<string> chosedLibraries = new List<string>();

           foreach (Transform panel in _content.transform)
            {
                LibruaryPanelUI libruaryPanelUI = panel.gameObject.GetComponent<LibruaryPanelUI>();
                
                if(libruaryPanelUI == null)
                    continue;

                if (libruaryPanelUI.gameObject.GetComponentInChildren<Toggle>().isOn)
                    chosedLibraries.Add(libruaryPanelUI.LibraryName);
            }

            if(chosedLibraries.Count() <= 0)
            return;

            UIBlocker.Block();
            await StorageManager.UnpackLibrariesToCache(chosedLibraries.ToArray());
            await AudioManager.RefreshAudioDictionaryAsync();
            OnStarted?.Invoke();
            UIBlocker.Unblock();
        });
    }
}
