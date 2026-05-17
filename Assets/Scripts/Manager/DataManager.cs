using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ItemDataManager))]
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
        DontDestroyOnLoad(gameObject);

        itemDataManager = GetComponent<ItemDataManager>();

        if (itemDataManager == null) Debug.LogError("DataManager: ItemDataManager component is missing!");
        if (districtDataManager == null) Debug.LogError("DataManager: DistrictDataManager reference is missing! Assign in Inspector.");
        if (weatherDataManager == null) Debug.LogError("DataManager: WeatherDataManager SO reference is missing! Assign in Inspector.");
        else weatherDataManager.Initialize();
    }
}
