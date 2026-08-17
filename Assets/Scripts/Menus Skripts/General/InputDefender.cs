using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class InputDefender
{
    private static char[] StrictInvalidChars = new char[]
    {
        '<', '>', ':', '"', '/', '\\', '|', '?', '*'
    };

    // Зарезервированные имена Windows, которые нельзя использовать как имя файла
    private static string[] WindowsReservedNames = new string[]
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };


    public static string ToCorrectFileName(string input)
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

    public static string ToCorrectJsonString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        StringBuilder sb = new StringBuilder(input.Length);

        foreach (char c in input)
        {
            // 1. Убираем переносы строк и непечатаемый мусор, заменяя на пробел
            if (char.IsControl(c))
            {
                sb.Append(' ');
                continue;
            }

            // 2. Экранируем только то, что ломает синтаксис JSON
            switch (c)
            {
                case '"': sb.Append("_"); break;
                case '\\': sb.Append("_"); break;
                default: sb.Append(c); break;
            }
        }

        return sb.ToString();
    }

    public static string ToMaxCorrect(string input)
    {
        return ToCorrectFileName(ToCorrectJsonString(input));
    }
}
