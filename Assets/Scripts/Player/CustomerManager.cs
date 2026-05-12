using System;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public StoreManager storeManager;
    private int baseVisitor = 20;

    private void Start()
    {
        storeManager = GetComponent<StoreManager>();
    }

    public int CalculateDailyVisitors()
    {
        switch (storeManager.currentZone)
        {
            // GameData 업데이트 시 CSV GameData 내부 
            case Commerce.Academy:
                return baseVisitor + (int)(baseVisitor * 0.25);
            case Commerce.Campus:
                return baseVisitor + (int)(baseVisitor * 0.5); 
            case Commerce.Business:
                return baseVisitor + (int)(baseVisitor * 0.75);
            case Commerce.Tourist:
                return baseVisitor + baseVisitor;
            default:
                return baseVisitor;
        }
    }

    public int CalculateWeatherVisitor()
    {
        WeatherSystem weather = GetComponent<WeatherSystem>(); //  => GameData로 변경
        Weather morningWeather = weather.morningWeather;
        Weather afternoonweather = weather.afternoonWeather;
        
        int currnetVisitor = CalculateDailyVisitors();
        float morningVisitor = CalculateWeatherVisitorWeight(morningWeather,currnetVisitor / 2f);
        float afternoonVisitor = CalculateWeatherVisitorWeight(afternoonweather,currnetVisitor / 2f);
        
        return (int)(morningVisitor + afternoonVisitor);
    }

    public float CalculateWeatherVisitorWeight(Weather weather, float visitor)
    {
        switch (weather)
        {
            case Weather.Rainy:
                return visitor *= 1.05f;
            case Weather.Cloudy:
                return visitor *= 0.95f;
            case Weather.Heatwave:
                return visitor *= 1.15f;
            case Weather.Snowy:
                return visitor *= 0.85f;
            default: return visitor;
        }
    }
}
