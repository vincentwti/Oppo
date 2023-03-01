using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkyController : MonoBehaviour
{
    private RectTransform rectTransform;
    public Material daySkyboxMaterial;
    public Material nightSkyboxMaterial;

    public Material cloudyDaySkyboxMaterial;
    public Material cloudyNightSkyboxMaterial;

    public Material snowyDaySkyboxMaterial;
    public Material snowyNightSkyboxMaterial;

    public GameObject rainVfxObject;
    public GameObject snowVfxObject;

    public Button dayButton;
    public Button nightButton;

    public Button sunnyButton;
    public Button rainyButton;
    public Button snowyButton;

    public enum TimeType
    {
        morning,
        day,
        night
    }
    public TimeType timeType;

    public enum WeatherType
    {
        sunny = 0,
        cloudy = 1,
        rainy = 2,
        snowy = 3
    }
    public WeatherType weatherType;

    //private void Start()
    //{
    //    rectTransform = (RectTransform)transform;
    //    Debug.LogWarning("width : " + Screen.width);
    //}

    private void Start()
    {
        rainVfxObject.SetActive(false);
        snowVfxObject.SetActive(false);

        dayButton.onClick.AddListener(() => SwitchDaySkybox());
        nightButton.onClick.AddListener(() => SwitchNightSkybox());

        sunnyButton.onClick.AddListener(() => SwitchSunnyWeather());
        rainyButton.onClick.AddListener(() => SwitchCloudyWeather());
        snowyButton.onClick.AddListener(() => SwitchSnowyWeather());
    }

    public void CheckCurrentTimeWeather(Weather.WeatherType weather)
    {
        switch (weather)
        {
            case Weather.WeatherType.Clear:
                OnWeatherChanged(WeatherType.sunny);
                break;
            case Weather.WeatherType.Drizzle:
                OnWeatherChanged(WeatherType.rainy);
                break;
            case Weather.WeatherType.Rain:
                OnWeatherChanged(WeatherType.rainy);
                break;
            case Weather.WeatherType.Cloudy:
                OnWeatherChanged(WeatherType.cloudy);
                break;
            case Weather.WeatherType.Snow:
                OnWeatherChanged(WeatherType.snowy);
                break;
            default:
                OnWeatherChanged(WeatherType.sunny);
                break;
        }
        CheckCurrentHour();
    }

    public void OnPartOfDayChanged(int part)
    {
        timeType = (TimeType)part;
        switch ((TimeType)part)
        {
            case TimeType.morning:
                SwitchDaySkybox(false);
                break;
            case TimeType.day:
                SwitchDaySkybox(false);
                break;
            case TimeType.night:
                SwitchNightSkybox(false);
                break;
        }
    }

    public void OnWeatherChanged(int weather)
    {
        weatherType = (WeatherType)weather;
        switch ((WeatherType)weather)
        {
            case WeatherType.sunny:
                SwitchSunnyWeather(false);
                break;
            case WeatherType.rainy:
                SwitchCloudyWeather(false);
                break;
            case WeatherType.snowy:
                SwitchSnowyWeather(false);
                break;
        }
    }

    public void OnWeatherChanged(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.sunny:
                SwitchSunnyWeather(false);
                break;
            case WeatherType.cloudy:
                SwitchCloudyWeather(false);
                break;
            case WeatherType.rainy:
                SwitchRainyWeather(false);
                break;
            case WeatherType.snowy:
                SwitchSnowyWeather(false);
                break;
        }
    }

    public void SwitchMorningSkybox(bool isSync = true)
    {
        timeType = TimeType.day;

        if (weatherType == WeatherType.sunny)
        {
            SwitchSunnyWeather(isSync);
        }
        else if (weatherType == WeatherType.cloudy)
        {
            SwitchCloudyWeather(isSync);
        }
        else if (weatherType == WeatherType.rainy)
        {
            SwitchRainyWeather(isSync);
        }
        else if (weatherType == WeatherType.snowy)
        {
            SwitchSnowyWeather(isSync);
        }

        if (isSync)
        {
            EventManager.onPartOfDayChanged?.Invoke(GameManager.Instance.GetClientId(), (int)timeType);
        }
    }

    public void SwitchDaySkybox(bool isSync = true)
    {
        timeType = TimeType.day;

        if (weatherType == WeatherType.sunny)
        {
            SwitchSunnyWeather(isSync);
        }
        else if (weatherType == WeatherType.cloudy)
        {
            SwitchCloudyWeather(isSync);
        }
        else if (weatherType == WeatherType.rainy)
        {
            SwitchRainyWeather(isSync);
        }
        else if (weatherType == WeatherType.snowy)
        {
            SwitchSnowyWeather(isSync);
        }

        if (isSync)
        {
            EventManager.onPartOfDayChanged?.Invoke(GameManager.Instance.GetClientId(), (int)timeType);
        }
    }

    public void SwitchNightSkybox(bool isSync = true)
    {
        timeType = TimeType.night;

        if (weatherType == WeatherType.sunny)
        {
            SwitchSunnyWeather(isSync);
        }
        else if (weatherType == WeatherType.cloudy)
        {
            SwitchCloudyWeather(isSync);
        }
        else if (weatherType == WeatherType.rainy)
        {
            SwitchRainyWeather(isSync);
        }
        else if (weatherType == WeatherType.snowy)
        {
            SwitchSnowyWeather(isSync);
        }

        if (isSync)
        {
            EventManager.onPartOfDayChanged?.Invoke(GameManager.Instance.GetClientId(), (int)timeType);
        }
    }

    public void SwitchSunnyWeather(bool isSync = true)
    {
        weatherType = WeatherType.sunny;
        rainVfxObject.SetActive(false);
        snowVfxObject.SetActive(false);

        if (timeType == TimeType.day)
        {
            RenderSettings.skybox = daySkyboxMaterial;
        }
        else
        {
            RenderSettings.skybox = nightSkyboxMaterial;
        }

        if (isSync)
        {
            EventManager.onWeatherChanged?.Invoke(GameManager.Instance.GetClientId(), (int)weatherType);
        }
    }

    public void SwitchCloudyWeather(bool isSync = true)
    {
        weatherType = WeatherType.cloudy;
        rainVfxObject.SetActive(false);
        snowVfxObject.SetActive(false);

        if (timeType == TimeType.day)
        {
            RenderSettings.skybox = cloudyDaySkyboxMaterial;
        }
        else
        {
            RenderSettings.skybox = cloudyNightSkyboxMaterial;
        }

        if (isSync)
        {
            EventManager.onWeatherChanged?.Invoke(GameManager.Instance.GetClientId(), (int)weatherType);
        }
    }

    public void SwitchRainyWeather(bool isSync = true)
    {
        weatherType = WeatherType.rainy;
        rainVfxObject.SetActive(true);
        snowVfxObject.SetActive(false);

        if (timeType == TimeType.day)
        {
            RenderSettings.skybox = cloudyDaySkyboxMaterial;
        }
        else
        {
            RenderSettings.skybox = cloudyNightSkyboxMaterial;
        }

        if (isSync)
        {
            EventManager.onWeatherChanged?.Invoke(GameManager.Instance.GetClientId(), (int)weatherType);
        }
    }

    public void SwitchSnowyWeather(bool isSync = true)
    {
        weatherType = WeatherType.snowy;
        rainVfxObject.SetActive(false);
        snowVfxObject.SetActive(true);

        if (timeType == TimeType.day)
        {
            RenderSettings.skybox = snowyDaySkyboxMaterial;
        }
        else
        {
            RenderSettings.skybox = snowyNightSkyboxMaterial;
        }

        if (isSync)
        {
            EventManager.onWeatherChanged?.Invoke(GameManager.Instance.GetClientId(), (int)weatherType);
        }
    }

    public void OnValueChanged(float value)
    {
        //float width = rectTransform.sizeDelta.x * transform.localScale.x;
        //float maxX = width - ((RectTransform)rectTransform.root.transform).sizeDelta.x;

        //Vector2 pos = rectTransform.anchoredPosition;
        //pos.x = value * -maxX;
        //rectTransform.anchoredPosition = pos;

        //skyBoxMaterial.SetFloat("_Rotation", value * 300);
    }

    private void CheckCurrentHour()
    {
        int hour = DateTime.Now.Hour;
        if (hour >= 5 && hour < 11)
        {
            timeType = TimeType.morning;
        }
        else if (hour >= 11 && hour < 19)
        {
            timeType = TimeType.day;
        }
        else
        {
            timeType = TimeType.night;
        }
        OnPartOfDayChanged((int)timeType);
    }

}
