using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenFade : Tweening
{
    public float from;
    public float to;
    public enum FadeType
    {
        canvasGroup,
        graphic,
        spriteRenderer
    }
    public FadeType fadeType;

    private Graphic graphic;
    private CanvasGroup canvasGroup;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        graphic = GetComponent<Graphic>();
        canvasGroup = GetComponent<CanvasGroup>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Init()
    {
        base.Init();
    }

    public override void Play()
    {
        Color fromColor;
        Color toColor;
        switch (fadeType)
        {
            case FadeType.canvasGroup:
                canvasGroup.alpha = from;
                tween = canvasGroup.DOFade(to, duration);
                break;
            case FadeType.graphic:
                fromColor = graphic.color;
                fromColor.a = from;
                graphic.color = fromColor;
                toColor = graphic.color;
                toColor.a = to;
                tween = graphic.DOColor(toColor, duration);
                break;
            case FadeType.spriteRenderer:
                fromColor = spriteRenderer.color;
                fromColor.a = from;
                spriteRenderer.color = fromColor;
                toColor = spriteRenderer.color;
                toColor.a = to;
                tween = spriteRenderer.DOColor(toColor, duration);
                break;
        }
        Init();
        tween.Play();
    }
}
