using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("Sub Data Managers")]
    public WeatherDataManager weatherDataManager;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        if (weatherDataManager == null) Debug.LogError("DataManager: WeatherDataManager SO reference is missing! Assign in Inspector.");
        else weatherDataManager.Initialize();
    }
}
