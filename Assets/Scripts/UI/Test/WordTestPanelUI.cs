using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

 //панель слова для теста: ввод ответа и проверка
public class WordTestPanelUI : WordPanelUI
{
    //текст прогресса (номер/всего)
    [SerializeField] private TextMeshProUGUI _progressText;
    //текст правильного ответа
    [SerializeField] private TextMeshProUGUI _correctWordText;
    //текст подсказки (объяснение для теста)
    [SerializeField] private TextMeshProUGUI _explanationTestText;
    //поле ввода ответа пользователя
    [SerializeField] private TMP_InputField _userInput;
    //кнопка проверки ответа
    [SerializeField] private Button _checkButton;
    //кнопка следующего слова
    [SerializeField] private Button _nextButton;
    //кнопка следующего слова для внешнего доступа
    public Button NextButton => _nextButton;

    //индекс текущего слова
    private int _currentWordIndex = 0;

    //установка индекса с обновлением прогресса
    public int CurrentWordIndex
    {
        get {return _currentWordIndex;}
        set
        {
            _currentWordIndex = value;
            _progressText.text = _currentWordIndex + "/" + _wordsCount;
        }
    }

    //общее количество слов
    private int _wordsCount = 0;

    //установка общего количества с обновлением прогресса
    public int WordsCount
    {
        get {return _wordsCount;}
        set
        {
            _wordsCount = value;
            _progressText.text = _currentWordIndex + "/" + _wordsCount;
        }
    }

    //результат проверки ответа
    private bool _isCorrectUserInput;
    public bool IsCorrectUserInput => _isCorrectUserInput;

    //подписка на кнопки проверки и аудио
    protected override void Start()
    {
        AudioManager.onFinished += OnClipFinished;
        _playButton.onClick.AddListener(PlayClip);
        _stopButton.onClick.AddListener(StopClip);
        _checkButton.onClick.AddListener(Check);
    }

    //сброс панели и отображение нового слова
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

    //проверка ответа пользователя
    private void Check()
    {
        _correctWordText.gameObject.SetActive(true);

        if(string.Equals(_correctWordText?.text, _userInput?.text, StringComparison.OrdinalIgnoreCase))
        {
            _correctWordText.text = "верно";
            _isCorrectUserInput = true;
        }

        _nextButton.gameObject.SetActive(true);
    }
}
