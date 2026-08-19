using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordLerningPanel : WordPanelUI
{
    [SerializeField] private TextMeshProUGUI _wordText;
    [SerializeField] private TextMeshProUGUI _explanationText;
    [SerializeField] private GameObject _partitionPlank;

    protected override void OnDestroy()
    {
        AudioManager.onFinished -= OnClipFinished;
    }

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
