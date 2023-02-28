using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenPosition : Tweening
{
    public Vector3 from;
    public Vector3 to;
    public enum PositionType
    {
        localPosition,
        worldPosition,
        anchoredPosition
    }
    public PositionType positionType;
    protected override void Init()
    {
        base.Init();
    }

    public override void Play()
    {
        switch (positionType)
        {
            case PositionType.localPosition:
                transform.localPosition = from;
                break;
            case PositionType.worldPosition:
                transform.position = from;
                break;
            case PositionType.anchoredPosition:
                ((RectTransform)transform).anchoredPosition = from;
                break;
        }


        switch (positionType)
        {
            case PositionType.localPosition:
                tween = transform.DOLocalMove(to, duration);
                break;
            case PositionType.worldPosition:
                tween = transform.DOMove(to, duration);
                break;
            case PositionType.anchoredPosition:
                tween = ((RectTransform)transform).DOAnchorPos(to, duration);
                break;
        }
        Init();
        tween.Play();
    }
}
