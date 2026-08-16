using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlocker : MonoBehaviour
{
    private static CanvasGroup _canvasGroup;
    private static GameObject _loadingImage;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _loadingImage = transform.Find("Loading Image").gameObject;
    }

    public static void Freze()
    {
        _canvasGroup.blocksRaycasts = false;
    }

    public static void Unfreze()
    {
        _canvasGroup.blocksRaycasts = true;
    }

    public static void Block()
    {
        _canvasGroup.alpha = 0.5f;
        _canvasGroup.blocksRaycasts = false;
        _loadingImage.SetActive(true);
    }

    public static void Unblock()
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1;
        _loadingImage.SetActive(false);
    }
}
