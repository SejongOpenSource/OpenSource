using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public WeatherType morningWeather;
    public WeatherType afternoonWeather;

    // 하루 날씨 생성
    public void GenerateWeather()
    {
        morningWeather = GetMorningWeather();
        afternoonWeather = GetAfternoonWeather(morningWeather);
        Debug.Log($"[일기예보] 오전: {morningWeather} | 오후: {afternoonWeather}");
    }
    
    // 오전 날씨 생성
    private WeatherType GetMorningWeather()
    {
        float roll = Random.Range(0f, 100f);
        
        if (roll < 5f)  return WeatherType.Heatwave; // 5%
        if (roll < 10f) return WeatherType.Snowy;    // 5%
        if (roll < 35f) return WeatherType.Rainy;    // 25%
        if (roll < 65f) return WeatherType.Cloudy;   // 30%
        return WeatherType.Sunny;                    // 35%
    } 
    
    // 오후 날씨 생성
    private WeatherType GetAfternoonWeather(WeatherType morning)
    {
        // 50% 확률로 오전 날씨 유지
        if (Random.value < 0.5f) return morning;

        switch (morning)
        {
            case WeatherType.Snowy:
                return GetRandomFrom(WeatherType.Rainy, WeatherType.Cloudy, WeatherType.Sunny);
                
            case WeatherType.Heatwave:
                return GetRandomFrom(WeatherType.Cloudy, WeatherType.Rainy, WeatherType.Sunny);
            
            case WeatherType.Cloudy:
                return GetRandomFrom(WeatherType.Rainy, WeatherType.Sunny, WeatherType.Heatwave, WeatherType.Snowy);
            
            case WeatherType.Rainy:
                return GetRandomFrom(WeatherType.Sunny, WeatherType.Snowy, WeatherType.Cloudy);
            
            case WeatherType.Sunny:
                return GetRandomFrom(WeatherType.Heatwave, WeatherType.Cloudy, WeatherType.Rainy, WeatherType.Snowy);
            
            default:
                return (WeatherType)Random.Range(0, 5);
        }
    }
    
    // 가능한 날씨 선택
    private WeatherType GetRandomFrom(params WeatherType[] options)
    {
        if (options.Length == 0)
        {
            Debug.Log("날씨 오류 발생!!!");
            return WeatherType.Sunny;
        }
        return options[Random.Range(0, options.Length)];
    }
}