using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

 //менеджер тестирования: управление прохождением и подсчётом результатов
public class TestManager : MonoBehaviour
{
    //загруженные библиотеки
    private List<LibraryData> _libraries = new List<LibraryData>();
    //все слова из выбранных библиотек
    private List<WordData> _words = new List<WordData>();
    //панель текущего слова для теста
    [SerializeField] private WordTestPanelUI _wordTestPanelUI;
    //панель результатов
    [SerializeField] private GameObject _resultsPanel;
    //количество правильных ответов
    private int _correctWordsCount = 0;
    //индекс текущего слова
    private int _currentWordIndex = 0;
    //текст общего количества слов
    [SerializeField] private TextMeshProUGUI _wordsCountText;
    //текст количества правильных ответов
    [SerializeField] private TextMeshProUGUI _correctWordsCountText;
    //текст процента выполнения
    [SerializeField] private TextMeshProUGUI _correctPercentText;
    //кнопка запуска теста
    [SerializeField] private StartTestButtonUI _startTestButton;

    //подписка на запуск теста и кнопку следующего слова
    void Start()
    {
        _startTestButton.OnStarted += StartTest;
        _wordTestPanelUI.NextButton.onClick.AddListener(UpdateWord);
    }

    //начало теста: загрузка слов и отображение первого
    private void StartTest()
    {
            _wordTestPanelUI.gameObject.SetActive(true);
            _resultsPanel.SetActive(false);

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

    //переход к следующему слову или завершение теста
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

    //завершение теста и вывод результатов
    private void EndTest()
    {
        _wordTestPanelUI.gameObject.SetActive(false);
        _resultsPanel.SetActive(true);
        _wordsCountText.text = "Количество слов:" + _words.Count().ToString();
        _correctWordsCountText.text = "Количество верных:" + _correctWordsCount;
        float percent = (float)System.Math.Round((float)_correctWordsCount / _words.Count() * 100f, MidpointRounding.AwayFromZero);
        _correctPercentText.text = "Процент выполнения:" + percent.ToString() + "%";
    }

    void OnDisable()
    {
        StorageManager.ClearLibraryData();
        _words.Clear();
        _correctWordsCount = 0;
        _currentWordIndex = 0;
        _wordsCountText.text = "Ошибка";
        _correctWordsCountText.text = "Ни одна библиотека не выбрана";
        _correctPercentText.text = "Вернитесь назад";
    }

}
