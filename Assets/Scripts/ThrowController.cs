using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ThrowController : MonoBehaviour
{
    private Vector2 acceleration;

    private const float minUpAccValue = -0.8f;
    private const float minDownAccValue = -0.5f;

    private float resetTime = 1f;
    public float elapsedTime = 0f;

    private bool startCount = false;
    private bool thrown = false;

    private void Start()
    {
        elapsedTime = 0f;
        startCount = false;
    }

    private void Update()
    {
        if (acceleration.y <= minUpAccValue)
        {
            startCount = true;
            elapsedTime = 0;
        }
        if (startCount)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= resetTime)
            {
                startCount = false;
                elapsedTime = 0;
            }

            if (acceleration.y <= minDownAccValue)
            {
                if (GameManager.Instance.controlType == GameManager.ControlType.PICTURE)
                {
                    Vector2 size = GameManager.Instance.webCamera.GetPhotoSize();
                    //EventManager.onCapturedPhotoSent?.Invoke(GameManager.Instance.webCamera.GetCapturedPhoto(), size);
                    EventManager.onEncodedCapturedPhotoSent?.Invoke(GameManager.Instance.GetClientId(), GameManager.Instance.webCamera.GetEncodedCapturedPhoto(), size);
                    thrown = true;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            acceleration.x = Input.acceleration.x;
            acceleration.y = Input.acceleration.y;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            acceleration.x = -Input.acceleration.y;
            acceleration.y = Input.acceleration.z;
        }
    }

    private void OnGUI()
    {
        if (!GameManager.Instance.IsServer)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 50;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(40, 40, 300, 60), "isThrown : " + thrown, style);
        }
    }
}
