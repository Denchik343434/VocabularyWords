using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    private List<LibraryData> _libraries = new List<LibraryData>();
    private List<WordData> _words = new List<WordData>();
    [SerializeField] private WordTestPanelUI _wordTestPanelUI;
    [SerializeField] private GameObject _resoltsPanel;
    private int _correctWordsCount = 0;
    private int _currentWordIndex = 0;
    [SerializeField] private TextMeshProUGUI _wordsCountText;
    [SerializeField] private TextMeshProUGUI _correctWordsCountText;
    [SerializeField] private TextMeshProUGUI _coorectProcentText;
    [SerializeField] private StartTestButtonUI _startTestButton;

    void Start()
    {
        _startTestButton.OnStarted += StartTest;
        _wordTestPanelUI.NextButton.onClick.AddListener(UpdateWord);
    }

    private void StartTest()
    {
            _wordTestPanelUI.gameObject.SetActive(true);
            _resoltsPanel.SetActive(false);

            _libraries = StorageManager.GetLoadedLibrariesFromCache().ToList();

            foreach(LibraryData library in _libraries)
            {
                foreach(WordData word in library.Words)
                {
                    _words.Add(word);
                }
            }

            _wordTestPanelUI.WordsCount = _words.Count();
            _wordTestPanelUI.CurrentWordIndex = _currentWordIndex + 1;
            _wordTestPanelUI.Word = _words[_currentWordIndex];
    }

    private void UpdateWord()
    {
        _currentWordIndex++;

        if (_wordTestPanelUI.IsCorrectUserInput)
            _correctWordsCount++;

        if(_currentWordIndex > _words.Count() - 1)
        {
            EndTest();
            return;
        }

        _wordTestPanelUI.CurrentWordIndex = _currentWordIndex + 1;
        _wordTestPanelUI.Word = _words[_currentWordIndex];
    }

    private void EndTest()
    {
        _wordTestPanelUI.gameObject.SetActive(false);
        _resoltsPanel.SetActive(true);
        _wordsCountText.text = "Количество слов:" + _words.Count().ToString();
        _correctWordsCountText.text = "Количество верных:" + _correctWordsCount;
        float procent = (float)System.Math.Round((float)_correctWordsCount / _words.Count() * 100f, MidpointRounding.AwayFromZero);
        _coorectProcentText.text = "Процент выполнения:" + procent.ToString() + "%";
    }

    void OnDisable()
    {
        StorageManager.ClearLibraryData();
        _words.Clear();
        _correctWordsCount = 0;
        _currentWordIndex = 0;
        _wordsCountText.text = "Ошибка";
        _correctWordsCountText.text = "Ни одна библиотека не выбрана";
        _coorectProcentText.text = "Вернитесь назад";
    }

}
