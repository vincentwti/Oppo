using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenScale : Tweening
{
    public Vector3 from;
    public Vector3 to;
    protected override void Init()
    {
        base.Init();
    }

    public override void Play()
    {
        transform.localScale = from;
        tween = transform.DOScale(to, duration);
        Init();
        tween.Play();
    }
}
