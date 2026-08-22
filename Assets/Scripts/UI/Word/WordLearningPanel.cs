using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//панель отображения слова для изучения
public class WordLearningPanel : WordPanelUI
{
    //текст слова
    [SerializeField] private TextMeshProUGUI _wordText;
    //текст объяснения
    [SerializeField] private TextMeshProUGUI _explanationText;
    //разделитель между словом и объяснением
    [SerializeField] private GameObject _partitionPlank;

    //отписка от события аудио
    protected override void OnDestroy()
    {
        AudioManager.onFinished -= OnClipFinished;
    }

    //заполнение панели данными слова
    protected override void SetUp()
    {
        _wordText.text = Word.Word.Replace("_", "\u0301");;
        _explanationText.text = Word.Explanation;

        
        if (string.IsNullOrWhiteSpace(_explanationText.text))
        {
            _explanationText.gameObject.SetActive(false);
            _partitionPlank.SetActive(false);
        }
        
    }
}
