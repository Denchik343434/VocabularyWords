using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// компонент панели редактирования слова
public class WordPanelUI : MonoBehaviour
{
    // Ссылки на UI элементы 
    [SerializeField] private TMP_InputField _wordInput;
    [SerializeField] private TMP_InputField _explanationInput;
    [SerializeField] private GameObject _emptyWarningIcon;

    [Space(15)]
    [SerializeField] private Button _clipButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _removeButton;

    // Путь к прикрепленному аудиофайлу
    private string _attachedAudioPath = "";

    // поле и свойство для получения данный слова при сохранении библиотеки
    private WordData _word;
    public WordData Word => _word;

    //Событие говорящее можно ли сохранятся или нет
    public event Action _onSavePosible;

    private void Start()
    {
        _wordInput.onEndEdit.AddListener(_ => {_emptyWarningIcon.SetActive(IsValid());});
        //TODO: добавить проверку на прикрипленный аудиофайл

        _wordInput.onEndEdit.AddListener(_ => OnValuesUpdate());
        _explanationInput.onEndEdit.AddListener(_ => _word = UpdateWord());
        //TODO: добавить подпись на событие на изменение прикрипленного аудиофайла
         
        _clipButton.onClick.AddListener(OnClipClicked);
        _playButton.onClick.AddListener(OnPlayClicked);
        _pauseButton.onClick.AddListener(OnPauseClicked);
        _removeButton.onClick.AddListener(OnRemoveClicked);

        OnValuesUpdate();
    }

    // Заполнение данными при загрузке существующей библиотеки, требует подписки на событие
    private void Setup(WordData wordData)
    {
        _wordInput.text = wordData.Word;
        _explanationInput.text = wordData.Explanation;

        //TODO: добавить заполнение пути к аудио файлу
        // attachedAudioPath = wordData.audio;
    }

    // Запись данных из полей в переменную _word
    private WordData UpdateWord()
    {
        return new WordData
        {
            Word = _wordInput.text.Trim(),
            Explanation = _explanationInput.text.Trim()
            // audio = attachedAudioPath
        };
    }

    private void OnValuesUpdate()
    {
        _word = UpdateWord();
        _onSavePosible?.Invoke();
    }

    // Проверка, заполнены ли обязательные поля
    private bool IsValid()
    {
        return string.IsNullOrWhiteSpace(_wordInput.text);

        //TODO: добавить проверку на наличае аудио
    }

    //метод обработки нажатия кнопки удаления
    private void OnRemoveClicked()
    {
        Debug.Log("Типо удалено");
    }

    //метод для обработки нажатия на кнопку скрепки (выбор аудиофайла)
    private void OnClipClicked()
    {
        // TODO: Вызов проводника для выбора аудиофайла
        Debug.Log("Нажата скрепка: выбираем аудио...");
    }

    //метод для обработки нажатия на кнопку воспроизведения аудио
    private void OnPlayClicked()
    {
        // TODO: Воспроизведение прикрепленного аудио
        Debug.Log("Воспроизведение аудио...");
    }

     //метод для обработки нажатия на кнопку паузы аудио
    private void OnPauseClicked()
    {
        // TODO: Пауза воспроизведения аудио
        Debug.Log("Пауза воспроизведения аудио...");
    }
}
