using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ItemDataManager))]
[RequireComponent(typeof(DistrictDataManager))]
[RequireComponent(typeof(WeatherDataManager))]
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("Sub Data Managers")]
    public ItemDataManager itemDataManager;
    public DistrictDataManager districtDataManager;
    public WeatherDataManager weatherDataManager;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        
        itemDataManager = GetComponent<ItemDataManager>();
        districtDataManager = GetComponent<DistrictDataManager>();
        weatherDataManager = GetComponent<WeatherDataManager>();

        if (itemDataManager == null) Debug.LogError("DataManager: ItemDataManager component is missing!");
        if (districtDataManager == null) Debug.LogError("DataManager: DistrictDataManager component is missing!");
        if (weatherDataManager == null) Debug.LogError("DataManager: WeatherDataManager component is missing!");
    }
}
