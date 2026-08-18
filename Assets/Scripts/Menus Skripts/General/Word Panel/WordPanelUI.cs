using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WordPanelUI : MonoBehaviour
{
    [SerializeField] protected Button _playButton;
    [SerializeField] protected Button _stopButton;
    protected WordData _word;
    public WordData Word
    {
        get
        {
            return _word;
        }
        set
        {
            Debug.Log("Проблема здесь");
            _word = value;
            SetUp();
        }
    }

    protected virtual void Start()
    {
        AudioManager.onFinished += OnClipFinished;
        _playButton.onClick.AddListener(PlayClip);
        _stopButton.onClick.AddListener(StopClip);
    }

    protected abstract void OnDestroy();

    protected abstract void SetUp();

    protected void PlayClip()
    {
        AudioManager.Stop();
        _stopButton.gameObject.SetActive(true);
        _playButton.gameObject.SetActive(false);
        AudioManager.Play(Word.Word);
    }

    protected void StopClip()
    {
        AudioManager.Stop();
    }

    protected void OnClipFinished()
    {
        _stopButton.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(true);
    }
}
