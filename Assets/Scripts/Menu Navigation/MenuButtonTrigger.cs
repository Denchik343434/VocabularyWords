using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonTrigger : MonoBehaviour
{
    [SerializeField] private string _targetMenuName;
    private Button _button;
    private MenuManager _menuManager;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(UpdateMenu);
        _menuManager = FindObjectOfType<MenuManager>();
    }

    private void UpdateMenu()
    {
        _menuManager.OnOpenPanelRequested?.Invoke(_targetMenuName);
    }
}
