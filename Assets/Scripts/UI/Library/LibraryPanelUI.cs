using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//панель отображения одной библиотеки в списке
public class LibraryPanelUI : MonoBehaviour
{
    //кнопка удаления библиотеки
    [SerializeField] private DeleteLibraryButtonUI _deleteButton;
    //кнопка открытия библиотеки
    [SerializeField] private OpenLibraryButtonUI _openButton; 
    //текст с названием библиотеки
    [SerializeField] private TextMeshProUGUI _libraryNameText;

    //название библиотеки
    private string _libraryName;

    //установка имени библиотеки и обновление UI
    public string LibraryName
    {
        get { return _libraryName;}
        set 
        { 
            _libraryName = value;
            _deleteButton.LibraryName = value;
            _libraryNameText.text = value;

            if(_openButton != null)
            _openButton.LibraryName = value;
        }
    }
}
