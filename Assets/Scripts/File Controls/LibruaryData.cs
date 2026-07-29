using System;
using System.Collections.Generic;

[Serializable]
public class LibraryData
{
    public string LibraryName;
    public List<WordData> Words = new List<WordData>();
}