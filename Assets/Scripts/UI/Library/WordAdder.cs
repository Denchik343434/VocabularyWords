using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//скрипт добавления нового слова в библиотеку через UI
public class WordAdder : MonoBehaviour
{
    //контейнер для списка слов
    [SerializeField] private GameObject _content;
    //префаб панели редактирования слова
    [SerializeField] private GameObject _WordEditPanelPrefab;
    //префаб кнопки добавления слова
    [SerializeField] private GameObject _addWordPanelPrefab;
    //текущая кнопка добавления слова
    private GameObject _addWordPanel;

    //свойство для установки кнопки добавления с подпиской на нажатие
    public GameObject AddWordPanel
    {
        get { return _addWordPanel; }
        set 
        {
            _addWordPanel = value;
            _addWordPanel.GetComponent<Button>().onClick.AddListener(() => AddWord());
        }
    }

    //метод добавления нового слова: создаёт панель, заменяет кнопку на новую
    private void AddWord()
    {
        GameObject newWord = Instantiate(_WordEditPanelPrefab.gameObject, _content.transform);
        newWord.GetComponent<WordEditPanelUI>().Word = new WordData();
        _addWordPanel.GetComponent<Button>().onClick.RemoveAllListeners();
        Destroy(_addWordPanel);
        AddWordPanel = Instantiate(_addWordPanelPrefab, _content.transform);
    }
}
