using UnityEngine;
using System.Collections.Generic;

public class SalesAlgorithm : MonoBehaviour
{
    public static SalesAlgorithm Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RunSimulation(int totalVisitors)
    {
        int dailyTotalRevenue = 0;
        Dictionary<ItemType, int> salesCount = new Dictionary<ItemType, int>();
        
        // Initialize sales count
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
            salesCount[type] = 0;

        DistrictData district = DataManager.Instance.districtDataManager.GetDistrict(StoreManager.Instance.currentZone);
        Weather morning = WeatherSystem.Instance.morningWeather;
        Weather afternoon = WeatherSystem.Instance.afternoonWeather;

        for (int i = 0; i < totalVisitors; i++)
        {
            ItemType? chosenItem = PickItem(district, morning, afternoon);
            if (chosenItem.HasValue)
            {
                // Check stock in InventoryManager
                if (InventoryManager.Instance.GetStock(chosenItem.Value) > 0)
                {
                    ItemData itemData = DataManager.Instance.itemDataManager.GetItem(chosenItem.Value);
                    int price = InventoryManager.Instance.GetEffectivePrice(chosenItem.Value, district);
                    
                    InventoryManager.Instance.UpdateStock(chosenItem.Value, 1);
                    dailyTotalRevenue += price;
                    salesCount[chosenItem.Value]++;
                }
            }
        }

        // Apply results
        StoreManager.Instance.AddMoney(dailyTotalRevenue);
        GameManager.Instance.AddSales(dailyTotalRevenue);
        
        Debug.Log($"오늘의 총 매출: {dailyTotalRevenue}원");
    }

    private ItemType? PickItem(DistrictData district, Weather morning, Weather afternoon)
    {
        // 기본 확률 (각 20%)
        float[] probabilities = { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };

        // 상권 가중치 적용
        if (district != null)
        {
            probabilities[0] *= district.onigiriMult;
            probabilities[1] *= district.noodleMult;
            probabilities[2] *= district.drinkMult;
            probabilities[3] *= district.bentoMult;
            probabilities[4] *= district.umbrellaMult;
        }

        // 날씨 가중치 적용 (Simplified for backend logic)
        ApplyWeatherProductWeights(probabilities, morning, true);
        ApplyWeatherProductWeights(probabilities, afternoon, false);

        // Roulette wheel selection
        float totalProb = 0;
        foreach (float p in probabilities) totalProb += p;

        float roll = Random.Range(0f, totalProb);
        float cumulative = 0;
        
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (roll <= cumulative) return (ItemType)i;
        }

        return null;
    }

    private void ApplyWeatherProductWeights(float[] probs, Weather weather, bool isMorning)
    {
        // Example logic
        switch (weather)
        {
            case Weather.Rainy:
                probs[4] *= 2.0f; // Umbrella
                break;
            case Weather.Heatwave:
                probs[2] *= 1.5f; // Drink
                break;
        }
    }
}
