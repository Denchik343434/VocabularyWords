using System;

 //TODO: добавить сохранение аудио файлов в zip архиве

// Класс одного слова (в будущем добавить сюда аудио) 
[Serializable]
public class WordData
{
    // Снаружи читать МОЖНО, менять НЕЛЬЗЯ (private set)
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