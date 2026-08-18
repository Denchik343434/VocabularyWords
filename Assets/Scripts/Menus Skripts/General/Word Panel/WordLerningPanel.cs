using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordLerningPanel : WordPanelUI
{
    [SerializeField] private TextMeshProUGUI _wordText;
    [SerializeField] private TextMeshProUGUI _explanationText;
    
    protected override void OnDestroy()
    {
        AudioManager.onFinished -= OnClipFinished;
    }

    protected override void SetUp()
    {
        _wordText.text = Word.Word;
        _explanationText.text = Word.Explanation;
    }
}
