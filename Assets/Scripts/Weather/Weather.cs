using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class WeatherData
{
    public int id;
    public string main;
    public string description;
}

[Serializable]
public class WeatherResult
{
    public WeatherData weather;
}

public class Weather : MonoBehaviour
{
    private const string URL = "https://api.openweathermap.org/data/2.5/weather?lat=[LAT]&lon=[LONG]&appid=[APP_ID]";
    private const string SECRET_KEY = "2e1d07536db7b31f962c70b6664485b0";

    private readonly int[] THUNDERSTORM_KEY = { 200, 201, 202, 210, 211, 212, 221, 230, 231, 232 };
    private const string THUNDERSTORM_VALUE = "Thunderstorm";

    private readonly int[] DRIZZLE_KEY = { 300, 301, 302, 310, 311, 312, 313, 314, 321 };
    private const string DRIZZLE_VALUE = "Drizzle";

    private readonly int[] RAIN_KEY = { 500, 501, 502, 503, 504, 511, 520, 521, 522, 531 };
    private const string RAIN_VALUE = "Rain";

    private readonly int[] SNOW_KEY = { 600, 601, 602, 611, 612, 613, 615, 616, 620, 621, 622 };
    private const string SNOW_VALUE = "Snow";

    private readonly int[] ATMOSPHERE_KEY = { 701, 711, 721, 731, 741, 751, 761, 762, 771, 781 };
    private const string ATMOSPHERE_VALUE = "Whatever";

    private readonly int[] CLEAR_KEY = { 800 };
    private const string CLEAR_VALUE = "Clear";

    private readonly int[] CLOUDS_KEY = { 801, 802, 803, 804 };
    private const string CLOUDS_VALUE = "Clouds";

    public WeatherResult weather;

    public IEnumerator RequestWeather(float latitude, float longitude)
    {
        string url = URL.Replace("[LAT]", latitude.ToString()).Replace("[LONG]", longitude.ToString()).Replace("[APP_ID]", SECRET_KEY);

        UnityWebRequest uwr = new UnityWebRequest(url, "GET");
        uwr.downloadHandler = new DownloadHandlerBuffer();
        yield return uwr.SendWebRequest();
        try
        {
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string json = uwr.downloadHandler.text;
                weather = JsonUtility.FromJson<WeatherResult>(json);
                Debug.Log("json : " + json);
                Debug.Log("id : " + weather.weather.id + "__" + weather.weather.main + " " + weather.weather.description);
            }
        }
        catch (Exception exc)
        {
            Debug.LogError(uwr.result.ToString());
            Debug.LogError("err : " + exc.Message);
        }
    }
}
