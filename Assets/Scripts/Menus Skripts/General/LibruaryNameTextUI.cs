using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class LibruaryNameTextUI : MonoBehaviour
{
    public event Action OnTextChanged;
     private TextMeshProUGUI _libruaryNameText;
    private TMP_InputField _libruaryNameInputField;

    private string _text;

    public string Text
    {
        get {return _text;}
        set
        {
            value = ToCorrectText(value);

            _text = value;

            if (_libruaryNameInputField != null)
            _libruaryNameInputField.text = value;

            if(_libruaryNameText != null)
            _libruaryNameText.text = value;

            OnTextChanged?.Invoke();
        }
    }

    private char[] StrictInvalidChars = new char[]
    {
        '<', '>', ':', '"', '/', '\\', '|', '?', '*'
    };

    // Зарезервированные имена Windows, которые нельзя использовать как имя файла
    private string[] WindowsReservedNames = new string[]
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    void Awake()
    {
        _libruaryNameText = GetComponent<TextMeshProUGUI>();
        _libruaryNameInputField = GetComponent<TMP_InputField>();
    }

    void Start()
    {
        if (_libruaryNameInputField != null)
        _libruaryNameInputField.onEndEdit.AddListener((value) => {Text = value;});
    }

    private string ToCorrectText(string input)
    {
        if (input == null)
            return null;

        char replacement = '_'; // Символ, на который будут заменяться недопустимые символы

        // Заменяем недопустимые символы
        foreach (char c in StrictInvalidChars)
        {
            input = input.Replace(c, replacement);
        }

        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsControl(input[i]))
            {
                input = input.Replace(input[i], replacement);
            }
        }

        // Windows не разрешает имена файлов, заканчивающиеся на точку или пробел
        input = input.Trim(' ', '.');

        string nameOnly = Path.GetFileNameWithoutExtension(input).ToUpperInvariant();
        foreach (string reserved in WindowsReservedNames)
        {
            if (nameOnly == reserved)
            {
                input = "_" + input; // Защищаем префиксом
                break;
            }
        }

        // Проверяем, осталось ли хоть что-то после очистки
        return input;
    }
}
