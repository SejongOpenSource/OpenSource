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
        Dictionary<ItemType, float> probabilities = new Dictionary<ItemType, float>();
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
            probabilities[type] = 0.2f;

        if (district != null)
        {
            probabilities[ItemType.Onigiri] *= district.onigiriMult;
            probabilities[ItemType.Noodle] *= district.noodleMult;
            probabilities[ItemType.Drink] *= district.drinkMult;
            probabilities[ItemType.Bento] *= district.bentoMult;
            probabilities[ItemType.Umbrella] *= district.umbrellaMult;
        }

        ApplyWeatherProductWeights(probabilities, morning, true);
        ApplyWeatherProductWeights(probabilities, afternoon, false);

        float totalProb = 0;
        foreach (float p in probabilities.Values) totalProb += p;

        float roll = Random.Range(0f, totalProb);
        float cumulative = 0;
        
        foreach (var kvp in probabilities)
        {
            cumulative += kvp.Value;
            if (roll <= cumulative) return kvp.Key;
        }

        return null;
    }

    private void ApplyWeatherProductWeights(Dictionary<ItemType, float> probs, Weather weather, bool isMorning)
    {
        switch (weather)
        {
            case Weather.Rainy:
                probs[ItemType.Umbrella] *= 2.0f;
                break;
            case Weather.Heatwave:
                probs[ItemType.Drink] *= 1.5f;
                break;
        }
    }
}
