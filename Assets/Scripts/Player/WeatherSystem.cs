using UnityEngine;

// 간단한 하루(오전/오후) 날씨 생성 시스템
public class WeatherSystem : MonoBehaviour
{
    // 오전에 결정된 날씨를 저장합니다.
    public WeatherType morningWeather;

    // 오후에 결정된 날씨를 저장합니다.
    public WeatherType afternoonWeather;

    // 하루 전체의 날씨를 생성하는 공개 API
    // 호출하면 `morningWeather`와 `afternoonWeather`를 결정하고 로그를 출력합니다.
    public void GenerateWeather()
    {
        morningWeather = GetMorningWeather();
        afternoonWeather = GetAfternoonWeather(morningWeather);
        Debug.Log($"[일기예보] 오전: {morningWeather} | 오후: {afternoonWeather}");
    }
    
    // 오전에 발생할 날씨를 확률적으로 결정합니다.
    // 내부적으로 0~100 사이의 난수를 굴려서 다음 비율로 선택합니다:
    // - Heatwave: 5%
    // - Snowy:   5%
    // - Rainy:  25%
    // - Cloudy: 30%
    // - Sunny:  35%
    private WeatherType GetMorningWeather()
    {
        float roll = Random.Range(0f, 100f);
        
        if (roll < 5f)  return WeatherType.Heatwave; // 5%
        if (roll < 10f) return WeatherType.Snowy;    // 5% (5~10)
        if (roll < 35f) return WeatherType.Rainy;    // 25% (10~35)
        if (roll < 65f) return WeatherType.Cloudy;   // 30% (35~65)
        return WeatherType.Sunny;                    // 35% (65~100)
    } 
    
    // 오후 날씨 생성
    private WeatherType GetAfternoonWeather(WeatherType morning)
    {
        // 50% 확률로 오전 날씨 유지
        if (Random.value < 0.5f) return morning;

        // 오전 날씨별로 오후에 올 수 있는 후보들을 정의합니다.
        switch (morning)
        {
            case WeatherType.Snowy:
                // 눈이 올 경우 오후엔 비/흐림/맑음 중 하나로 바뀔 수 있음
                return GetRandomFrom(WeatherType.Rainy, WeatherType.Cloudy, WeatherType.Sunny);
                
            case WeatherType.Heatwave:
                // 폭염일 경우에도 흐림/비/맑음으로 완화될 수 있음
                return GetRandomFrom(WeatherType.Cloudy, WeatherType.Rainy, WeatherType.Sunny);
            
            case WeatherType.Cloudy:
                // 흐림이었을 때는 비/맑음/폭염/눈 등 다양한 방향으로 변할 수 있음
                return GetRandomFrom(WeatherType.Rainy, WeatherType.Sunny, WeatherType.Heatwave, WeatherType.Snowy);
            
            case WeatherType.Rainy:
                // 비였을 경우엔 맑음/눈/흐림으로 바뀔 수 있음
                return GetRandomFrom(WeatherType.Sunny, WeatherType.Snowy, WeatherType.Cloudy);
            
            case WeatherType.Sunny:
                // 맑음은 폭염/흐림/비/눈 등으로 변할 수 있음
                return GetRandomFrom(WeatherType.Heatwave, WeatherType.Cloudy, WeatherType.Rainy, WeatherType.Snowy);
            default:
                // 예외: 정의되지 않은 값이 들어오면 디버그하기 쉽도록 예외 발생
                throw new System.ArgumentOutOfRangeException(nameof(morning), $"Not expected weather type: {morning}");
        }
    }
    
    // 전달된 후보 목록에서 하나를 무작위로 선택합니다.
    // 비어있는 배열이 들어오면 예외를 던집니다.
    private WeatherType GetRandomFrom(params WeatherType[] options)
    {
        if (options == null || options.Length == 0)
        {
            throw new System.ArgumentException("Cannot select from an empty list of options.", nameof(options));
        }
        return options[Random.Range(0, options.Length)];
    }
}