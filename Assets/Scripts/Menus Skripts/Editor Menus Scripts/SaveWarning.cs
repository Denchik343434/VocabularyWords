using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

//скрипт для иконки предупреждения о незаполненных словах
public class SaveWarning : MonoBehaviour
{
    private int _unFilldWordsCount;
    
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _saveWarningImage;
    [SerializeField] private InputfieldTMPText _LibruaryName;

    public event Action<bool> OnSaveWarningChanged;

    void Start()
    {
        WordPanelUI.OnValuesChanged += UpdateValidate;
        _LibruaryName.OnTextChanged += UpdateValidate;
    }

    void OnEnable()
    {
        UpdateValidate();
    }

    private void UpdateValidate()
    {
        bool IsValid = true;
        
        if (string.IsNullOrWhiteSpace(_LibruaryName.Text))
        {
            IsValid = false;
        }

        if (IsValid)
        {

            foreach (Transform panel in _content)
            {
                if (panel.TryGetComponent<WordPanelUI>(out var wordPanel))
                {
                    if (wordPanel.IsEmpty || _content.childCount == 0)
                    {
                        IsValid = false;
                    }
                }
            }
        }

        _saveWarningImage.SetActive(!IsValid);
        OnSaveWarningChanged?.Invoke(IsValid); 
    }

    void OnDestroy()
    {
        WordPanelUI.OnValuesChanged -= UpdateValidate;
    }
}
