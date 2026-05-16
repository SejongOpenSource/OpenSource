using UnityEngine;
using UnityEngine.UI;

public class WeatherPanelController : MonoBehaviour
{
    // 날씨를 생성하고 저장하는 WeatherSystem
    public WeatherSystem weatherSystem;

    // 오전 날씨 이미지
    public Image morningWeatherImage;

    // 오후 날씨 이미지
    public Image afternoonWeatherImage;

    // 날씨별 스프라이트
    public Sprite sunnySprite;
    public Sprite rainySprite;
    public Sprite heatwaveSprite;
    public Sprite cloudySprite;
    public Sprite snowySprite;

    // 테스트용: 시작할 때 날씨를 새로 생성할지 여부
    public bool generateOnStart = true;

    private void Start()
    {
        // WeatherSystem이 연결되어 있지 않으면 실행하지 않음
        if (weatherSystem == null)
        {
            Debug.LogWarning("WeatherSystem이 연결되지 않았습니다.");
            return;
        }

        // 테스트용으로 시작 시 날씨 생성
        // 나중에는 턴/페이즈 시작 시점에서 GenerateWeather를 호출하는 구조로 바꿀 수 있음
        if (generateOnStart)
        {
            weatherSystem.GenerateWeather();
        }

        // 생성된 날씨에 맞춰 이미지 변경
        UpdateWeatherImages();
    }

    public void UpdateWeatherImages()
    {
        // WeatherSystem이 없으면 변경할 날씨 데이터가 없음
        if (weatherSystem == null)
        {
            return;
        }

        // 오전 날씨 이미지 변경
        if (morningWeatherImage != null)
        {
            morningWeatherImage.sprite = GetWeatherSprite(weatherSystem.morningWeather);
        }

        // 오후 날씨 이미지 변경
        if (afternoonWeatherImage != null)
        {
            afternoonWeatherImage.sprite = GetWeatherSprite(weatherSystem.afternoonWeather);
        }
    }

    private Sprite GetWeatherSprite(Weather weather)
    {
        // Weather enum 값에 맞는 스프라이트 반환
        switch (weather)
        {
            case Weather.Sunny:
                return sunnySprite;

            case Weather.Rainy:
                return rainySprite;

            case Weather.Heatwave:
                return heatwaveSprite;

            case Weather.Cloudy:
                return cloudySprite;

            case Weather.Snowy:
                return snowySprite;

            default:
                return null;
        }
    }
}