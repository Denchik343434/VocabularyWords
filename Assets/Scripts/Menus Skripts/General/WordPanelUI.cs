using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using TMPro;

// компонент панели редактирования слова 
public class WordPanelUI : MonoBehaviour
{
    // Ссылки на UI элементы 
    [SerializeField] private TMP_InputField _wordInput;
    [SerializeField] private TMP_InputField _explanationInput;
    [SerializeField] private TMP_InputField _testExplanation;
    [SerializeField] private GameObject _emptyWarningIcon;

    [Space(15)]
    [SerializeField] private Button _clipButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _removeButton;

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
    public static event Action OnValuesChanged;

    // свойсто на наличае нужных полей для проверки на возможность сохранения
    public bool IsEmpty => IsValid();

    private void Start()
    {
        _wordInput.onEndEdit.AddListener(_ => _emptyWarningIcon.SetActive(IsValid()));
        //TODO: добавить проверку на прикрипленный аудиофайл

        _wordInput.onEndEdit.AddListener(input => {_wordInput.text = InputDefender.ToMaxCorrect(input); OnValuesUpdate();});
        _explanationInput.onEndEdit.AddListener(input => {_explanationInput.text = InputDefender.ToCorrectJsonString(input); OnValuesUpdate();});
        _testExplanation.onEndEdit.AddListener(input => {_testExplanation.text = InputDefender.ToCorrectJsonString(input); OnValuesUpdate();}); 

        AudioManager.onFinished += OnClipFinished;

        //TODO: добавить подпись на событие на изменение прикрипленного аудиофайла
         
        _clipButton.onClick.AddListener(OnClipClicked);
        _playButton.onClick.AddListener(OnPlayClicked);
        _pauseButton.onClick.AddListener(OnStopClicked);
        _removeButton.onClick.AddListener(OnRemoveClicked);

        OnValuesUpdate();
        _emptyWarningIcon.SetActive(IsValid()); 
    }

    // Заполнение данными при загрузке существующей библиотеки, используется в свойстве
    private void Setup(WordData wordData)
    {
        _wordInput.text = wordData.Word;
        _explanationInput.text = wordData.Explanation;
        _testExplanation.text = wordData.TestExplanation;
    }

    // Запись данных из полей в переменную _word
    private WordData UpdateWord()
    {
        return new WordData
        (
            _wordInput.text,
            _explanationInput.text,
            _testExplanation.text
        );
    }

    //Метод для подписки на события изменения полей
    private void OnValuesUpdate()
    {
        string oldName = Word.Word;
        _word = UpdateWord();
        OnValuesChanged?.Invoke();

        if(oldName == Word.Word)
        return;
        
        AudioManager.RenameAudioClip(oldName, Word.Word);
    }

    // Проверка, заполнены ли обязательные поля
    private bool IsValid()
    {
        return string.IsNullOrWhiteSpace(_wordInput.text);
    }

    //метод обработки нажатия кнопки удаления
    private void OnRemoveClicked()
    {
        AudioManager.DeleteAudioClip(Word.Word);
        transform.SetParent(null);
        Destroy(gameObject);
        OnValuesChanged?.Invoke();
    }

    //метод для обработки нажатия на кнопку скрепки (выбор аудиофайла)
    private async void OnClipClicked()
    {
        if (IsEmpty)
        return;

        AudioManager.Stop();

        string path = StorageManager.GetUserPath(StorageFilterType.Audio);
        if (path == null)
        return;

        UIBlocker.Freze();
        await AudioManager.AddAudioClipAsync(Word.Word, path);
        UIBlocker.Unfreze();
        Debug.Log("По идее прикрепилось");
    }

    //метод для обработки нажатия на кнопку воспроизведения аудио
    private void OnPlayClicked()
    {
        AudioManager.Stop();
        _pauseButton.gameObject.SetActive(true);
        _playButton.gameObject.SetActive(false);
        AudioManager.Play(Word.Word);
    }

     //метод для обработки нажатия на кнопку паузы аудио
    private void OnStopClicked()
    {
        AudioManager.Stop();
    }

    private void OnClipFinished()
    {
        _pauseButton.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        AudioManager.onFinished -= OnClipFinished;
    }
}
