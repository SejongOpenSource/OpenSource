using UnityEngine;

public class WeatherDataManager : MonoBehaviour
{
    [Header("Weather Modifiers")]
    public float sunnyModifier = 1.0f;
    public float rainyModifier = 0.9f;
    public float heatwaveModifier = 1.05f;
    public float cloudyModifier = 0.8f;
    public float snowyModifier = 0.7f;

    public float GetModifier(WeatherType weather)
    {
        return weather switch
        {
            WeatherType.Sunny => sunnyModifier,
            WeatherType.Rainy => rainyModifier,
            WeatherType.Heatwave => heatwaveModifier,
            WeatherType.Cloudy => cloudyModifier,
            WeatherType.Snowy => snowyModifier,
            _ => 1.0f
        };
    }
}
