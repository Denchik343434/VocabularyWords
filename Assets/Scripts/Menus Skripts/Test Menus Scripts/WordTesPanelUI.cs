using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WordTestPanelUI : WordPanelUI
{
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _correctWordText;
    [SerializeField] private TextMeshProUGUI _explanationTestText;
    [SerializeField] private TMP_InputField _userInput;
    [SerializeField] private Button _checkButton;
    [SerializeField] private Button _nextButton;
    public Button NextButton => _nextButton;

    private int _currentWordIndex = 0;
    public int CurrentWordIndex
    {
        get {return _currentWordIndex;}
        set
        {
            _currentWordIndex = value;
            _progressText.text = _currentWordIndex + "/" + _wordsCount;
        }
    }

    private int _wordsCount = 0;
    public int WordsCount
    {
        get {return _wordsCount;}
        set
        {
            _wordsCount = value;
            _progressText.text = _currentWordIndex + "/" + _wordsCount;
        }
    }

    private bool _isCorrectUserInput;
    public bool IsCorrectUserInput => _isCorrectUserInput;

    protected override void Start()
    {
        AudioManager.onFinished += OnClipFinished;
        _playButton.onClick.AddListener(PlayClip);
        _stopButton.onClick.AddListener(StopClip);
        _checkButton.onClick.AddListener(Check);
    }

    protected override void SetUp()
    {
        _correctWordText.text = Word.Word.Replace("_", "");;
        _explanationTestText.text = Word.TestExplanation.Replace("_", "");
        _userInput.text = "";
        _correctWordText.gameObject.SetActive(false);
        _checkButton.gameObject.SetActive(true);
        _nextButton.gameObject.SetActive(false);
        _isCorrectUserInput = false;
    }

    protected override void OnDestroy()
    {
        AudioManager.onFinished -= OnClipFinished;
    }

    private void Check()
    {
        _correctWordText.gameObject.SetActive(true);

        if(_correctWordText.text == _userInput.text)
        {
            _correctWordText.text = "верно";
            _isCorrectUserInput = true;
        }

        _nextButton.gameObject.SetActive(true);
    }
}
