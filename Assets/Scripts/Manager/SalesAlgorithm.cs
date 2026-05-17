using UnityEngine;
using System.Collections.Generic;

public class SalesAlgorithm : MonoBehaviour
{
    public static SalesAlgorithm Instance { get; private set; }

    public int TotalSales { get; private set; } = 0;

    private static readonly ItemType[] _itemTypes = (ItemType[])System.Enum.GetValues(typeof(ItemType));
    private readonly Dictionary<ItemType, float> _probabilities = new Dictionary<ItemType, float>();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddSales(int amount) => TotalSales += amount;

    public void RunSimulation()
    {
        if (CustomerManager.Instance == null)
        {
            Debug.LogError("CustomerManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        int totalVisitors = CustomerManager.Instance.CalculateVisitors();
        int dailyTotalRevenue = 0;

        DistrictData district = GameManager.Instance.storeManager.currentDistrictData;
        WeatherType morning = GameManager.Instance.weatherSystem.morningWeather;
        WeatherType afternoon = GameManager.Instance.weatherSystem.afternoonWeather;
        InventoryManager inventory = InventoryManager.Instance;

        for (int i = 0; i < totalVisitors; i++)
        {
            ItemType? chosenItem = PickItem(district, morning, afternoon);
            if (!chosenItem.HasValue) continue;

            if (inventory == null || inventory.GetStock(chosenItem.Value) <= 0) continue;

            ItemData item = DataManager.Instance.GetItem(chosenItem.Value);
            if (item == null) continue;

            inventory.UpdateStock(chosenItem.Value, 1);
            dailyTotalRevenue += item.price;
        }

        GameManager.Instance.storeManager.AddMoney(dailyTotalRevenue);
        AddSales(dailyTotalRevenue);

        Debug.Log($"오늘의 총 매출: {dailyTotalRevenue}원");
    }

    private ItemType? PickItem(DistrictData district, WeatherType morning, WeatherType afternoon)
    {
        _probabilities.Clear();
        foreach (ItemType type in _itemTypes)
            _probabilities[type] = 0.2f;

        if (district != null)
        {
            _probabilities[ItemType.Onigiri] *= district.onigiriMult;
            _probabilities[ItemType.Noodle] *= district.noodleMult;
            _probabilities[ItemType.Drink] *= district.drinkMult;
            _probabilities[ItemType.Bento] *= district.bentoMult;
            _probabilities[ItemType.Umbrella] *= district.umbrellaMult;
        }

        ApplyWeatherProductWeights(_probabilities, morning, true);
        ApplyWeatherProductWeights(_probabilities, afternoon, false);

        float totalProb = 0;
        foreach (float p in _probabilities.Values) totalProb += p;

        float roll = Random.Range(0f, totalProb);
        float cumulative = 0;

        foreach (var kvp in _probabilities)
        {
            cumulative += kvp.Value;
            if (roll <= cumulative) return kvp.Key;
        }

        return null;
    }

    private void ApplyWeatherProductWeights(Dictionary<ItemType, float> probs, WeatherType weather, bool isMorning)
    {
        switch (weather)
        {
            case WeatherType.Rainy:
                probs[ItemType.Umbrella] *= 2.0f;
                break;
            case WeatherType.Heatwave:
                probs[ItemType.Drink] *= 1.5f;
                break;
        }
    }
}
