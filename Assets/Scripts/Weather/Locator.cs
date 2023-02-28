using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    private const float timeOut = 20f;
    private float elapsedTime = 0f;

    private float latitude;
    private float longitude;

    private void Start()
    {
        StartCoroutine(GetLatitudeLongitude(null));
    }

    public IEnumerator GetLatitudeLongitude(Action<float, float> onCompleted = null)
    {
        elapsedTime = 0f;
        if (!Input.location.isEnabledByUser)
        {
            yield break;
        }

        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing && timeOut > 0)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        if (elapsedTime <= 0)
        {
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            yield break;
        }
        else
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
        }
        onCompleted?.Invoke(latitude, longitude);
    }
}
