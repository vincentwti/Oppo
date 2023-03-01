using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HighlightedControlType : MonoBehaviour
{
    public Image image;
    public TMP_Text playText;

    public Button playButton;
    public Button backButton;

    public Vector3 desiredPosition;
    public Vector3 desiredScale;
    public UnityEvent onAnimationStarted;
    public UnityEvent onAnimationCompleted;

    private SelectableControlType controller;
    private ControlTypeInfo info;

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    public void Show(SelectableControlType controller, Transform target, ControlTypeInfo info)
    {
        this.controller = controller;
        this.info = info;
        image.sprite = info.controlSprite;
        image.SetNativeSize();
        playText.text = "Let's " + (info.controlName == "Draw" ? info.controlName : "Play");
        transform.position = target.position;
        transform.localScale = target.localScale;
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        float duration = 0.5f;
        Tweener tweenPos = ((RectTransform)transform).DOAnchorPos(desiredPosition, duration);
        tweenPos.SetEase(Ease.InOutQuad);
        tweenPos.Play();

        Tweener tweenScale = transform.DOScale(desiredScale, duration);
        tweenScale.SetEase(Ease.InOutQuad);
        tweenScale.OnStart(() => onAnimationStarted?.Invoke());
        tweenScale.OnComplete(() => onAnimationCompleted?.Invoke());
        tweenScale.Play();
    }

    private void Play()
    {
        gameObject.SetActive(false);
        controller.gameObject.SetActive(false);
        int index = 0;
        switch (info.controlName)
        {
            case "Draw":
                index = (int)GameManager.ControlType.DRAW;
                break;
            case "Kite":
                index = (int)GameManager.ControlType.KITE;
                break;
            case "Shake":
                index = (int)GameManager.ControlType.SHAKEDRAW;
                break;
            case "Selfie":
                index = (int)GameManager.ControlType.PICTURE;
                GameManager.Instance.ShowClearLineButton();
                break;
        }
        EventManager.onControlTypeSelected?.Invoke(index);
        EventManager.onConnectToNetwork?.Invoke();
    }

    private void Back()
    {
        controller.tweenFade.PlayBackward(null);
        gameObject.SetActive(false);
    }

    private void OnBackButtonClicked()
    {
        Back();
    }

    private void OnPlayButtonClicked()
    {
        Play();
    }
}
