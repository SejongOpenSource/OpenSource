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
        float currnetVisitor = CalculateZoneWeight(storeManager.currentZone);
        return CalculateWeatherVisitor(currnetVisitor);
    }

    public float CalculateZoneWeight(Commerce zone)
    {
        switch (zone)
        {
            // GameData 업데이트 시 CSV GameData 내부 
            case Commerce.Academy:
                return baseVisitor + baseVisitor * 0.25f;
            case Commerce.Campus:
                return baseVisitor + baseVisitor * 0.5f; 
            case Commerce.Business:
                return baseVisitor + baseVisitor * 0.75f;
            case Commerce.Tourist:
                return baseVisitor + baseVisitor;
            default:
                return baseVisitor;
        }
    }

    public int CalculateWeatherVisitor(float currentVisitor)
    {
        WeatherSystem weather = GetComponent<WeatherSystem>(); //  => GameData로 변경
        Weather morningWeather = weather.morningWeather;
        Weather afternoonweather = weather.afternoonWeather;
        
        float morningVisitor = CalculateWeatherVisitorWeight(morningWeather,currentVisitor / 2f);
        float afternoonVisitor = CalculateWeatherVisitorWeight(afternoonweather,currentVisitor / 2f);
        
        return (int)(morningVisitor + afternoonVisitor);
    }

    private float CalculateWeatherVisitorWeight(Weather weather, float visitor)
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
    
    public int SalesRevenue()
    {
        return 0;
    }
    
}
