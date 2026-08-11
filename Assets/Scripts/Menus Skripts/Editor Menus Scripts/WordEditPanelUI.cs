using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
using System.Linq;
// компонент панели редактирования слова
public class WordEditPanelUI : MonoBehaviour
{
    // Ссылки на UI элементы 
    [SerializeField] private InputfieldTMPText _wordInput;
    [SerializeField] private InputfieldTMPText _explanationInput;
    [SerializeField] private GameObject _emptyWarningIcon;

    [Space(15)]
    [SerializeField] private Button _clipButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _removeButton;

    // Путь к прикрепленному аудиофайлу
    //private string _attachedAudioPath = "";

    // поле и свойство для получения данный слова при сохранении библиотеки
    private WordData _word;
    public WordData Word
    {
        get
        {
            return _word;
        }

        set
        {
            _word = value;
            Setup(value);
        }
    }

    //Событие на изменение слова
    public static event Action OnWordChanget;

    // свойсто на наличае нужных полей для проверки на возможность сохранения
    public bool IsEmpty => IsValid();

    private void Start()
    {
        _wordInput.OnTextChanged += () => _emptyWarningIcon.SetActive(IsValid());
        //TODO: добавить проверку на прикрипленный аудиофайл

        _wordInput.OnTextChanged += () => OnValuesUpdate();
        _explanationInput.OnTextChanged += () => _word = UpdateWord();
        //TODO: добавить подпись на событие на изменение прикрипленного аудиофайла
         
        _clipButton.onClick.AddListener(OnClipClicked);
        _playButton.onClick.AddListener(OnPlayClicked);
        _pauseButton.onClick.AddListener(OnPauseClicked);
        _removeButton.onClick.AddListener(OnRemoveClicked);

        OnValuesUpdate();
        _emptyWarningIcon.SetActive(IsValid()); //чтото с вс кодом не так
    }

    // Заполнение данными при загрузке существующей библиотеки, используется в свойстве
    private void Setup(WordData wordData)
    {
        _wordInput.Text = wordData.Word;
        _explanationInput.Text = wordData.Explanation;

        //TODO: добавить заполнение пути к аудио файлу
        // attachedAudioPath = wordData.audio;
    }

    // Запись данных из полей в переменную _word
    private WordData UpdateWord()
    {
        return new WordData
        {
            Word =  _wordInput.Text,
            Explanation = _explanationInput.Text
        // audio = attachedAudioPath
        };
    }

    //Метод для подписки на события изменения полей
    private void OnValuesUpdate()
    {
        _word = UpdateWord();
        OnWordChanget?.Invoke();
    }

    // Проверка, заполнены ли обязательные поля
    private bool IsValid()
    {
        return string.IsNullOrWhiteSpace(_wordInput.Text);

        //TODO: добавить проверку на наличае аудио
    }

    //метод обработки нажатия кнопки удаления
    private void OnRemoveClicked()
    {
        OnWordChanget?.Invoke();
        Destroy(gameObject);
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
