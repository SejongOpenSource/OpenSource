using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [Header("Base Settings")]
    public int baseVisitors = 20;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public int CalculateTotalVisitors()
    {
        Weather morning = WeatherSystem.Instance.morningWeather;
        Weather afternoon = WeatherSystem.Instance.afternoonWeather;
        Commerce currentZone = StoreManager.Instance.currentZone;
        DistrictData district = DataManager.Instance.districtDataManager.GetDistrict(currentZone);

        float visitorCount = baseVisitors;

        // 상권 보너스
        if (district != null)
        {
            visitorCount *= (1f + district.visitorBonus);
        }

        // 날씨 영향 (WeatherDataManager 참조)
        float morningMod = DataManager.Instance.weatherDataManager.GetModifier(morning);
        float afternoonMod = DataManager.Instance.weatherDataManager.GetModifier(afternoon);
        float weatherMod = (morningMod + afternoonMod) / 2f;
        
        visitorCount *= weatherMod;

        return Mathf.RoundToInt(visitorCount);
    }
}
