using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//скрипт блокировки интерфейса во время загрузок
public class UIBlocker : MonoBehaviour
{
    //группа canvas для управления прозрачностью и блокировкой ввода
    private static CanvasGroup _canvasGroup;
    //изображение загрузки
    private static GameObject _loadingImage;

    //получение компонентов при старте
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _loadingImage = transform.Find("Loading Image").gameObject;
    }

    //блокировка нажатий без затемнения
    public static void Freeze()
    {
        _canvasGroup.blocksRaycasts = false;
    }

    //разблокировка нажатий
    public static void Unfreeze()
    {
        _canvasGroup.blocksRaycasts = true;
    }

    //блокировка интерфейса с затемнением и индикатором загрузки
    public static void Block()
    {
        _canvasGroup.alpha = 0.5f;
        _canvasGroup.blocksRaycasts = false;
        _loadingImage.SetActive(true);
    }

    //разблокировка интерфейса и скрытие индикатора загрузки
    public static void Unblock()
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1;
        _loadingImage.SetActive(false);
    }
}
