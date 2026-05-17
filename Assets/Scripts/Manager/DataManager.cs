using UnityEngine;

[RequireComponent(typeof(ItemDataManager))]
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
        DontDestroyOnLoad(gameObject);

        itemDataManager = GetComponent<ItemDataManager>();
        weatherDataManager = GetComponent<WeatherDataManager>();

        if (itemDataManager == null)
            Debug.LogError("DataManager: ItemDataManager component is missing!");
        if (districtDataManager == null)
            Debug.LogError("DataManager: DistrictDataManager SO reference is missing! Assign in Inspector.");
        if (weatherDataManager == null)
            Debug.LogError("DataManager: WeatherDataManager component is missing!");

        if (districtDataManager != null) districtDataManager.Initialize();
    }

    public DistrictData GetDistrict(string districtName) =>
        districtDataManager != null ? districtDataManager.GetDistrict(districtName) : null;

    public DistrictData GetDistrict(Commerce zone) =>
        districtDataManager != null ? districtDataManager.GetDistrict(zone) : null;

    public ItemData GetItem(ItemType type) =>
        itemDataManager != null ? itemDataManager.GetItem(type) : null;

    public float GetWeatherModifier(WeatherType weather) =>
        weatherDataManager != null ? weatherDataManager.GetModifier(weather) : 1.0f;
}
