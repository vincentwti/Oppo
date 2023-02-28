using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System;

public class Tweening : MonoBehaviour
{
    public float duration = 0.2f;
    public float delay = 0;
    public Ease easeType = Ease.Linear;
    public int loopCount = 1;
    public LoopType loopType = LoopType.Restart;
    public bool playOnEnable = true;
    public bool ignoreTimeScale = false;
    public bool killTween = false;

    public UnityEvent onTweenStarted;
    public UnityEvent onTweenUpdated;
    public UnityEvent onTweenCompleted;

    protected Tween tween;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Play();
        }
    }

    protected virtual void Init()
    {
        tween.SetAutoKill(killTween);
        tween.SetDelay(delay);
        tween.SetEase(easeType);
        tween.SetLoops(loopCount, loopType);
        tween.SetUpdate(ignoreTimeScale);
        tween.OnStart(() => onTweenStarted?.Invoke());
        tween.OnUpdate(() => onTweenUpdated?.Invoke());
        tween.OnComplete(() => onTweenCompleted?.Invoke());
    }

    public virtual void Play()
    {
        
    }

    public virtual void PlayBackward(Action onComplete)
    {
        if (killTween)
        {
            Debug.LogWarning("YOU MUST DISABLE KILL TWEEN");
        }
        else
        {
            tween.OnRewind(() => onComplete?.Invoke()).PlayBackwards();
        }
    }
}
