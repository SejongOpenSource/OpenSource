using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeatherData
{
    public Weather weatherType;
    public float modifier;
}

[CreateAssetMenu(fileName = "WeatherDataManager", menuName = "GameData/WeatherDataManager")]
public class WeatherDataManager : ScriptableObject
{
    [Header("Weather Data List")]
    [SerializeField] private List<WeatherData> weatherDataList = new List<WeatherData>();

    private Dictionary<Weather, WeatherData> weatherDict;

    public void Initialize()
    {
        weatherDict = new Dictionary<Weather, WeatherData>();
        foreach (var data in weatherDataList)
        {
            if (data == null) continue;
            if (!weatherDict.ContainsKey(data.weatherType))
                weatherDict[data.weatherType] = data;
            else
                Debug.LogWarning($"[WeatherDataManager] 중복 날씨 타입 무시됨: {data.weatherType}");
        }
    }

    public WeatherData GetWeather(Weather type)
    {
        if (weatherDict == null) Initialize();
        if (weatherDict.TryGetValue(type, out WeatherData data))
            return data;

        Debug.LogError($"[WeatherDataManager] 유효하지 않은 날씨 타입: {type}");
        return null;
    }

    public float GetModifier(Weather type)
    {
        WeatherData data = GetWeather(type);
        return data != null ? data.modifier : 1.0f;
    }
}
