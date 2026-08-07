using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LibruaryNameTextUI : MonoBehaviour
{
    private TextMeshProUGUI _libruaryNameText;
    private TMP_InputField _libruaryNameInputField;

    private string _text;

    public string Text
    {
        get {return _text;}
        set
        {
            _text = value;

            if (_libruaryNameInputField != null)
            _libruaryNameInputField.text = value;

            if(_libruaryNameText != null)
            _libruaryNameText.text = value;
        }
    }

    void Awake()
    {
        _libruaryNameText = GetComponent<TextMeshProUGUI>();
        _libruaryNameInputField = GetComponent<TMP_InputField>();
    }
}
