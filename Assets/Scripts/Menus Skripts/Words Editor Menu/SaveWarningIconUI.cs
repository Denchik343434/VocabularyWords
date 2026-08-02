using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//скрипт для иконки предупреждения о незаполненных словах
public class WordOutputer_12 : MonoBehaviour
{
    private int _unFilldWordsCount;
    
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _saveWarningImage;

    void Start()
    {
        WordEditPanelUI.OnWordChanget += UpdateCount;
    }

    private void UpdateCount()
    {
        foreach (Transform panel in _content)
        {
            if (panel.TryGetComponent<WordEditPanelUI>(out var wordPanel))
            {
                if (wordPanel.IsEmpty)
                {
                    _saveWarningImage.SetActive(true);
                    return; 
                }
            }
        }
        _saveWarningImage.SetActive(false);
    }

    void OnDestroy()
    {
        WordEditPanelUI.OnWordChanget -= UpdateCount;
    }
}
