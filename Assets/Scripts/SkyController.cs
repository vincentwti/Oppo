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
        day,
        night
    }
    public TimeType timeType;

    public enum WeatherType
    {
        sunny,
        rainy,
        snowy
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

    public void OnPartOfDayChanged(int part)
    {
        timeType = (TimeType)part;
        switch ((TimeType)part)
        {
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

    public void SwitchDaySkybox(bool isSync = true)
    {
        timeType = TimeType.day;

        if (weatherType == WeatherType.sunny)
        {
            SwitchSunnyWeather(isSync);
        }
        else if (weatherType == WeatherType.rainy)
        {
            SwitchCloudyWeather(isSync);
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
        else if (weatherType == WeatherType.rainy)
        {
            SwitchCloudyWeather(isSync);
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
}
