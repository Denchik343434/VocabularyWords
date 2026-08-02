using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: добавить поддержку кнопки сброса данных меню

//скрипт управления меню, при переходе пользователя между ними
public class MenuManager : MonoBehaviour
{
    //Словарь для обращения к меню по имени, Стек для хранения истории открытых меню
    private Dictionary<string, Menu> _menusDictionary = new Dictionary<string, Menu>();
    private Stack<string> _menuStack = new Stack<string>();

    //добавление всех меню в словарь при старте игры
    private void Awake()
    {
        Menu[] foundMenus = FindObjectsByType<Menu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Menu menu in foundMenus)
        {
            _menusDictionary.Add(menu.Name, menu);
        }
    }

    //закрытие всех меню при старте и открытие главного, подпись на событие кнопок перехода в другое меню
    void Start()
    {
        CloseMenu("all");
        OpenMenu("MainMenu");
        MenuButtonTrigger.OnOpenPanelRequested += GoToMenu;
    }

    void OnDestroy()
    {
        MenuButtonTrigger.OnOpenPanelRequested -= GoToMenu;
    }

    //закрытие меню 
    private void CloseMenu(string name)
    {
        if (name == "all")
        {
            foreach (Menu menu in _menusDictionary.Values)
            {
                menu.gameObject.SetActive(false);
            }
            return;
        }

        if (_menusDictionary.TryGetValue(name, out Menu targetMenu))
        {
            targetMenu.gameObject.SetActive(false);
        }
    }
    
    //метод открытия меню
    private void OpenMenu(string name)
    {
        if (_menusDictionary.TryGetValue(name, out Menu targetMenu))
        {
            targetMenu.gameObject.SetActive(true);
        }
        _menuStack.Push(name);
    }

    //метод для возврата в предыдущее меню, если оно есть
    private void GoBack()
    {
        if (_menuStack.Count > 1)
        {
            string current = _menuStack.Pop();
            CloseMenu(current);

            string previous = _menuStack.Peek();

            if (_menusDictionary.TryGetValue(previous, out Menu previousMenu))
            {
                previousMenu.gameObject.SetActive(true);
            }
        }
    }

    //метод для перехода в главное меню, закрывая все остальные
    private void ToMainMenu()
    {
        CloseMenu("all");
        _menuStack.Clear();
        OpenMenu("MainMenu");
    }

    //метод для перехода в определенное меню, закрывая текущее, для подписи на событие
    private void GoToMenu(string name)
    {
        if (name == "MainMenu")
        {
            ToMainMenu();
            return;
        }

        if (name == "last")
        {
            GoBack();
            return;
        }

        CloseMenu(_menuStack.Peek());
        OpenMenu(name);
    }
}
