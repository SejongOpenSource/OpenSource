using UnityEngine;

public enum Weather { Sunny, Rainy, Heatwave, Cloudy, Snowy }

public class WeatherSystem : MonoBehaviour
{
    public Weather morningWeather;
    public Weather afternoonWeather;

    // 하루 날씨 생성
    public void GenerateWeather()
    {
        morningWeather = GetMorningWeather();
        afternoonWeather = GetAfternoonWeather(morningWeather);
        Debug.Log($"[일기예보] 오전: {morningWeather} | 오후: {afternoonWeather}");
    }
    
    // 오전 날씨 생성
    private Weather GetMorningWeather()
    {
        float roll = Random.Range(0f, 100f);
        
        if (roll < 5f)  return Weather.Heatwave; // 5%
        if (roll < 10f) return Weather.Snowy;    // 5%
        if (roll < 35f) return Weather.Rainy;    // 25%
        if (roll < 65f) return Weather.Cloudy;   // 30%
        return Weather.Sunny;                    // 35%
    } 
    
    // 오후 날씨 생성
    private Weather GetAfternoonWeather(Weather morning)
    {
        // 50% 확률로 오전 날씨 유지
        if (Random.value < 0.5f) return morning;

        switch (morning)
        {
            case Weather.Snowy:
                return GetRandomFrom(Weather.Rainy, Weather.Cloudy, Weather.Sunny);
                
            case Weather.Heatwave:
                return GetRandomFrom(Weather.Cloudy, Weather.Rainy, Weather.Sunny);
            
            case Weather.Cloudy:
                return GetRandomFrom(Weather.Rainy, Weather.Sunny, Weather.Heatwave, Weather.Snowy);
            
            case Weather.Rainy:
                return GetRandomFrom(Weather.Sunny, Weather.Snowy, Weather.Cloudy);
            
            case Weather.Sunny:
                return GetRandomFrom(Weather.Heatwave, Weather.Cloudy, Weather.Rainy, Weather.Snowy);
            
            default:
                return (Weather)Random.Range(0, 5);
        }
    }
    
    // 가능한 날씨 선택
    private Weather GetRandomFrom(params Weather[] options)
    {
        if (options.Length == 0)
        {
            Debug.Log("날씨 오류 발생!!!");
            return Weather.Sunny;
        }
        return options[Random.Range(0, options.Length)];
    }
}