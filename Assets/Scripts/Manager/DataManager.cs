using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ItemDataManager))]
[RequireComponent(typeof(WeatherDataManager))]
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("Sub Data Managers")]
    [SerializeField] public ItemDataManager itemDataManager;
    [SerializeField] public DistrictDataManager districtDataManager;
    [SerializeField] public WeatherDataManager weatherDataManager;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        
        // Ensure components are assigned or fetched
        if (itemDataManager == null) itemDataManager = GetComponent<ItemDataManager>();
        if (weatherDataManager == null) weatherDataManager = GetComponent<WeatherDataManager>();

        // Initialization sequence
        InitializeAll();

        // Safety checks
        if (itemDataManager == null) Debug.LogError("DataManager: ItemDataManager is missing!");
        if (districtDataManager == null) Debug.LogError("DataManager: DistrictDataManager SO reference is missing!");
        if (weatherDataManager == null) Debug.LogError("DataManager: WeatherDataManager is missing!");
    }

    private void InitializeAll()
    {
        if (itemDataManager != null) itemDataManager.Initialize();
        if (districtDataManager != null) districtDataManager.Initialize();
        if (weatherDataManager != null) weatherDataManager.Initialize();
        Debug.Log("DataManager: All sub-managers initialized.");
    }

    // Bridge Methods
    public TradeData GetItem(ItemType type) => itemDataManager != null ? itemDataManager.GetItem(type) : null;
    public DistrictData GetDistrict(Commerce zone) => districtDataManager != null ? districtDataManager.GetDistrict(zone) : null;
    public DistrictData GetDistrict(string name) => districtDataManager != null ? districtDataManager.GetDistrict(name) : null;
    public float GetWeatherModifier(Weather weather) => weatherDataManager != null ? weatherDataManager.GetModifier(weather) : 1.0f;
}
