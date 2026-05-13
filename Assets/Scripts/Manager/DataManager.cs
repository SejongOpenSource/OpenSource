using UnityEngine;
using System.Collections.Generic;

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
    }
}
