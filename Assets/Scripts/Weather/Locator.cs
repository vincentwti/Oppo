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

    public IEnumerator GetLatitudeLongitude(Action<float, float> onCompleted = null)
    {
        elapsedTime = 0f;
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            if (!Input.location.isEnabledByUser)
            {
                yield break;
            }
        }

        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing && elapsedTime > 0)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        Debug.Log("status : " + Input.location.status.ToString() + " " + elapsedTime);
        if (elapsedTime >= timeOut)
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
            Debug.Log("lat : " + latitude + " , " + longitude);
        }
        onCompleted?.Invoke(latitude, longitude);
    }
}
