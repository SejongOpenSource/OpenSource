using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ItemDataManager))]
[RequireComponent(typeof(WeatherDataManager))]
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("Sub Data Managers")]
    public ItemDataManager itemDataManager;
    public DistrictDataManager districtDataManager; // ScriptableObject version
    public WeatherDataManager weatherDataManager;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        
        itemDataManager = GetComponent<ItemDataManager>();
        weatherDataManager = GetComponent<WeatherDataManager>();

        if (itemDataManager == null) Debug.LogError("DataManager: ItemDataManager component is missing!");
        if (districtDataManager == null) Debug.LogError("DataManager: DistrictDataManager reference is missing! Assign in Inspector.");
        if (weatherDataManager == null) Debug.LogError("DataManager: WeatherDataManager component is missing!");
    }
}
