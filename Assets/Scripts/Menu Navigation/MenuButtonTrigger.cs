using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//скрипт для кнопок открывающих другие меню
public class MenuButtonTrigger : MonoBehaviour
{
    //переменные имени открывающегося меню, и необходимые для работы компоненты 
    [SerializeField] private string _targetMenuName;
    private Button _button;
    private MenuManager _menuManager;

    //получение компонентов и подписка на событие нажатия кнопки
    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(UpdateMenu);
        _menuManager = FindObjectOfType<MenuManager>();
    }

    //метод вызывающий событие на которое реагирует менеджер меню для открытия меню
    private void UpdateMenu()
    {
        _menuManager.OnOpenPanelRequested?.Invoke(_targetMenuName);
    }
}
