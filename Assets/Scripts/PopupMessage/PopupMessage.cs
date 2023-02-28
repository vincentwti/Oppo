using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PopupMessage : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text contentText;
    public Button singleButton;
    public Button acceptButton;
    public Button rejectButton;

    public Transform panel;

    private void Start()
    {
        panel.transform.localScale = Vector3.zero;
        Tweener tween = panel.DOScale(Vector3.one, 0.2f);
        tween.Play();
    }

    public void ShoweMessage(string title = "", string content = "", Action onButtonClicked = null)
    {
        titleText.text = title;
        contentText.text = content;
        singleButton.onClick.RemoveAllListeners();
        singleButton.onClick.AddListener(() => onButtonClicked?.Invoke());
        singleButton.onClick.AddListener(Hide);
        singleButton.gameObject.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        rejectButton.gameObject.SetActive(false);
    }

    public void ShowConfirmationMessage(string title = "", string content = "", Action onAcceptButtonClicked = null, Action onRejectButtonClicked = null)
    {
        titleText.text = title;
        contentText.text = content;
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() => onAcceptButtonClicked?.Invoke());
        acceptButton.onClick.AddListener(Hide);
        rejectButton.onClick.RemoveAllListeners();
        rejectButton.onClick.AddListener(() => onRejectButtonClicked?.Invoke());
        rejectButton.onClick.AddListener(Hide);
        singleButton.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(true);
        rejectButton.gameObject.SetActive(true);
    }

    private void Hide()
    {
        Destroy(gameObject);
    }
}
