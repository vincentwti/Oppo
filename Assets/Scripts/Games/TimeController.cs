using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TimeController : MonoBehaviour
{
    public Image timeBarImage;
    public TMP_Text timeText;

    private float time;
    private float elapsedTime;
    private bool isTimeOut = false;
    private bool isPaused = false;
    private Action onTimeOut;

    public void SetTime(int time, Action onTimeOut)
    {
        this.time = time;
        this.onTimeOut = onTimeOut;
        elapsedTime = time;
        isTimeOut = false;
        isPaused = false;
        timeText.text = time.ToString();
        timeBarImage.fillAmount = elapsedTime / time;
    }

    public void SetTime(float time, bool isTimeOut, bool isPaused)
    {
        this.time = time;
        elapsedTime = time;
        this.isTimeOut = isTimeOut;
        this.isPaused = isPaused;
        timeText.text = time.ToString();
        timeBarImage.fillAmount = elapsedTime / time;
    }

    public void SetElapsedTime(float time)
    {
        elapsedTime = time;
        timeText.text = time.ToString();
        timeBarImage.fillAmount = elapsedTime / this.time;
    }

    public void Pause(bool pause)
    {
        isPaused = pause;
        Debug.Log("isPaused : " + isPaused);
    }

    private void Update()
    {
        Debug.Log("upd isPaused : " + isPaused);
        if (!isPaused)
        {
            if (elapsedTime > 0)
            {
                elapsedTime -= Time.deltaTime;
                timeBarImage.fillAmount = elapsedTime / time;
            }
            else
            {
                if (!isTimeOut)
                {
                    isTimeOut = true;
                    onTimeOut?.Invoke();
                }
            }
            timeText.text = Mathf.CeilToInt(elapsedTime).ToString();
        }
    }

    public float GetCurrentTime()
    {
        return time;
    }

    public bool GetIsPaused()
    {
        return isPaused;
    }

}
