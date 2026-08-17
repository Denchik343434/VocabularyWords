using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordAdder : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _WordEditPanelPrefab;
    [SerializeField] private GameObject _addWordPanelPrefab;
    private GameObject _addWordPanel;

    public GameObject AddWordPanel
    {
        get { return _addWordPanel; }
        set 
        {
            _addWordPanel = value;
            _addWordPanel.GetComponent<Button>().onClick.AddListener(() => AddWord());
        }
    }


    void Start()
    {
    
    }

    private void AddWord()
    {
        GameObject newWord = Instantiate(_WordEditPanelPrefab.gameObject, _content.transform);
        newWord.GetComponent<WordEditPanelUI>().Word = new WordData();
        _addWordPanel.GetComponent<Button>().onClick.RemoveAllListeners();
        Destroy(_addWordPanel);
        AddWordPanel = Instantiate(_addWordPanelPrefab, _content.transform);
    }
}
