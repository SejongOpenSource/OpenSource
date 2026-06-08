using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeatherData", menuName = "GameData/WeatherData")]
public class WeatherData : ScriptableObject
{
    // 날씨의 고유 타입 (Enum)
    [SerializeField] private WeatherType weatherType;
    
    // 화면에 표시될 날씨 이름
    [SerializeField] private string weatherName;
    
    // 해당 날씨일 때 손님 방문 수에 적용되는 배수
    [SerializeField] private float visitorMultiplier;
    
    [Header("발생 가중치 (오전 결정용)")]
    // 날씨가 선택될 확률을 결정하는 가중치
    [SerializeField] private float weight; 

    [Header("전이 데이터 (오후 결정용)")]
    // 현재 날씨 이후에 올 수 있는 가능한 날씨 목록
    [SerializeField] private List<WeatherType> nextWeatherOptions; 

    // 날씨 타입을 반환합니다.
    public WeatherType Type => weatherType;
    
    // 날씨 이름을 반환합니다.
    public string WeatherName => weatherName;
    
    // 방문자 수 배율을 반환합니다.
    public float VisitorMultiplier => visitorMultiplier;
    
    // 발생 가중치를 반환합니다.
    public float Weight => weight;
    
    // 전환 가능한 다음 날씨 목록을 반환합니다.
    public IReadOnlyList<WeatherType> NextWeatherOptions => nextWeatherOptions;
}
