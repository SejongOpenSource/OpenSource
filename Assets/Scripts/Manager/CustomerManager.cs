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
        float districtBonus = (GameManager.Instance.storeManager?.currentDistrictData != null)
            ? GameManager.Instance.storeManager.currentDistrictData.visitorBonus : 0f;

        float weatherMod = 1f;
        if (GameManager.Instance.weatherSystem != null && DataManager.Instance != null)
        {
            weatherMod = (DataManager.Instance.GetWeatherModifier(GameManager.Instance.weatherSystem.morningWeather)
                        + DataManager.Instance.GetWeatherModifier(GameManager.Instance.weatherSystem.afternoonWeather)) / 2f;
        }

        return Mathf.RoundToInt(baseVisitors * (1f + districtBonus) * weatherMod);
    }
}
