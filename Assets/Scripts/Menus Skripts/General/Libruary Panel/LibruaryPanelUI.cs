using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibruaryPanelUI : MonoBehaviour
{
    [SerializeField] private DeleteLibruaryButtonUI _deleteButton;
    [SerializeField] private OpenLibruaryButtonUI _openButton; 

    private string _libraryName;

    public string LibraryName
    {
        get { return _libraryName;}
        set 
        { 
            _libraryName = value;
            _deleteButton.LibraryName = value;
            _openButton.LibraryName = value;
        }
    }
}
