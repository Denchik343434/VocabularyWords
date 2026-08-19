using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LibruaryPanelUI : MonoBehaviour
{
    [SerializeField] private DeleteLibruaryButtonUI _deleteButton;
    [SerializeField] private OpenLibruaryButtonUI _openButton; 
    [SerializeField] private TextMeshProUGUI _libraryNameText;

    private string _libraryName;

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
