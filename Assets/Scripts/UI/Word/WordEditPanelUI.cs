using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

// компонент панели редактирования слова 
public class WordEditPanelUI : WordPanelUI
{
    // Ссылки на UI элементы 
    [SerializeField] private TMP_InputField _wordInput;
    [SerializeField] private TMP_InputField _explanationInput;
    [SerializeField] private TMP_InputField _testExplanation;
    [SerializeField] private GameObject _emptyWarningIcon;

    [Space(15)]
    [SerializeField] private Button _clipButton;
    [SerializeField] private Button _removeButton;

    //Событие на изменение слова
    public static event Action OnValuesChanged;

    // свойсто на наличае нужных полей для проверки на возможность сохранения
    public bool IsEmpty => IsValid();

    protected override void Start()
    {
        _wordInput.onEndEdit.AddListener(_ => _emptyWarningIcon.SetActive(IsValid()));

        _wordInput.onEndEdit.AddListener(input => {_wordInput.text = InputDefender.ToMaxCorrect(input); OnValuesUpdate();});
        _explanationInput.onEndEdit.AddListener(input => {_explanationInput.text = InputDefender.ToCorrectJsonString(input); OnValuesUpdate();});
        _testExplanation.onEndEdit.AddListener(input => {_testExplanation.text = InputDefender.ToCorrectJsonString(input); OnValuesUpdate();}); 

        AudioManager.onFinished += OnClipFinished;

        _clipButton.onClick.AddListener(ClipClip);
        _playButton.onClick.AddListener(PlayClip);
        _stopButton.onClick.AddListener(StopClip);
        _removeButton.onClick.AddListener(RemovePanel);

        OnValuesUpdate();
        _emptyWarningIcon.SetActive(IsValid()); 
    }

    // Заполнение данными при загрузке существующей библиотеки, используется в свойстве
    protected override void SetUp()
    {
        _wordInput.text = Word.Word;
        _explanationInput.text = Word.Explanation;
        _testExplanation.text = Word.TestExplanation;
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
    private void RemovePanel()
    {
        AudioManager.DeleteAudioClip(Word.Word);
        transform.SetParent(null);
        Destroy(gameObject);
        OnValuesChanged?.Invoke();
    }

    //метод для обработки нажатия на кнопку скрепки (выбор аудиофайла)
    private async void ClipClip()
    {
        if (IsEmpty)
        return;

        AudioManager.Stop();

        string path = StorageManager.GetUserPath(StorageFilterType.Audio);
        if (path == null)
        return;

        UIBlocker.Freeze();
        await AudioManager.AddAudioClipAsync(Word.Word, path);
        UIBlocker.Unfreeze();
    }

    protected override void OnDestroy()
    {
        AudioManager.onFinished -= OnClipFinished;
    }
}
