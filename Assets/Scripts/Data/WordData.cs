using System;

//класс данных одного слова
[Serializable]
public class WordData
{
    //само слово
    public string Word;
    //объяснение значения слова
    public string Explanation;
    //объяснение для проверки тестом
    public string TestExplanation;

    public WordData(string word = "", string explanation = "", string testExplanation = "")
    {
        Word = word;
        Explanation = explanation;
        TestExplanation = testExplanation;
    }
}