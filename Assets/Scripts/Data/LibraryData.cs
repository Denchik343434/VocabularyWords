using System;
using System.Collections.Generic;

//класс данных библиотеки слов
[Serializable]
public class LibraryData
{
    //название библиотеки
    public string LibraryName;
    //список слов в библиотеке
    public List<WordData> Words = new List<WordData>();
}