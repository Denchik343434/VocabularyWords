using System;
using System.Collections.Generic;

[Serializable]
public class Library
{
    public string libraryName;
    public List<Word> words = new List<Word>();
}