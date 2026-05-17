using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeatherData", menuName = "GameData/WeatherData")]
public class WeatherData : ScriptableObject
{
    [SerializeField] private WeatherType weatherType;
    [SerializeField] private string weatherName;
    [SerializeField] private float visitorMultiplier;
    
    [Header("발생 가중치 (오전 결정용)")]
    [SerializeField] private float weight; 

    [Header("전이 데이터 (오후 결정용)")]
    [SerializeField] private List<WeatherType> nextWeatherOptions; 

    // 외부에서 읽어갈 때 쓰는 프로퍼티
    public WeatherType Type => weatherType;
    public string WeatherName => weatherName;
    public float VisitorMultiplier => visitorMultiplier;
    public float Weight => weight;
    public IReadOnlyList<WeatherType> NextWeatherOptions => nextWeatherOptions;
}
