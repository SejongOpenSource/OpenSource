using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [SerializeField] private int baseVisitors = 50;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public int CalculateVisitors()
    {
        DistrictData district = GameManager.Instance.storeManager.currentDistrictData;
        float districtBonus = district != null ? district.visitorBonus : 0f;

        WeatherType morning = GameManager.Instance.weatherSystem.morningWeather;
        WeatherType afternoon = GameManager.Instance.weatherSystem.afternoonWeather;
        float weatherMod = (DataManager.Instance.GetWeatherModifier(morning)
                          + DataManager.Instance.GetWeatherModifier(afternoon)) / 2f;

        return Mathf.RoundToInt(baseVisitors * (1f + districtBonus) * weatherMod);
    }
}
