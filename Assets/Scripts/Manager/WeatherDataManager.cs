using UnityEngine;

public class WeatherDataManager : MonoBehaviour
{
    [Header("Weather Modifiers")]
    [SerializeField] private float sunnyModifier = 1.0f;
    [SerializeField] private float rainyModifier = 0.9f;
    [SerializeField] private float heatwaveModifier = 1.05f;
    [SerializeField] private float cloudyModifier = 0.8f;
    [SerializeField] private float snowyModifier = 0.7f;

    private Dictionary<Weather, float> weatherDict = new Dictionary<Weather, float>();

    public void Initialize()
    {
        weatherDict.Clear();
        weatherDict[Weather.Sunny] = sunnyModifier;
        weatherDict[Weather.Rainy] = rainyModifier;
        weatherDict[Weather.Heatwave] = heatwaveModifier;
        weatherDict[Weather.Cloudy] = cloudyModifier;
        weatherDict[Weather.Snowy] = snowyModifier;
    }

    public float GetModifier(Weather weather)
    {
        if (weatherDict.TryGetValue(weather, out float modifier)) return modifier;
        return 1.0f;
    }
}
