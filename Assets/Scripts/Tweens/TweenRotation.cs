using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenRotation : Tweening
{
    public Vector3 from;
    public Vector3 to;

    protected override void Init()
    {
        base.Init();
    }

    public override void Play()
    {
        transform.localEulerAngles = from;
        tween = transform.DOLocalRotate(to, duration);
        Init();
        tween.Play();
    }
}
