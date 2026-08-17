using System;

// Класс одного слова
[Serializable]
public class WordData
{
    public string Word;
    public string Explanation;
    public string TestExplanation;

    public WordData(string word = "", string explanation = "", string testExplanation = "")
    {
        Word = word;
        Explanation = explanation;
        TestExplanation = testExplanation;
    }
}