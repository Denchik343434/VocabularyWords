using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 //абстрактный базовый класс панели слова
public abstract class WordPanelUI : MonoBehaviour
{
    //кнопка воспроизведения аудио
    [SerializeField] protected Button _playButton;
    //кнопка остановки аудио
    [SerializeField] protected Button _stopButton;
    //данные текущего слова
    protected WordData _word;

    //данные слова с вызовом заполнения UI при установке
    public WordData Word
    {
        get
        {
            return _word;
        }
        set
        {
            _word = value;
            SetUp();
        }
    }

    //подписка на события аудио и кнопок
    protected virtual void Start()
    {
        AudioManager.onFinished += OnClipFinished;
        _playButton.onClick.AddListener(PlayClip);
        _stopButton.onClick.AddListener(StopClip);
    }

    protected abstract void OnDestroy();

    //заполнение UI данными слова
    protected abstract void SetUp();

    //воспроизведение аудиофайла слова
    protected void PlayClip()
    {
        AudioManager.Stop();
        _stopButton.gameObject.SetActive(true);
        _playButton.gameObject.SetActive(false);
        AudioManager.Play(Word.Word);
    }

    //остановка воспроизведения
    protected void StopClip()
    {
        AudioManager.Stop();
    }

    //переключение кнопок при окончании аудио
    protected void OnClipFinished()
    {
        _stopButton.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(true);
    }
}
