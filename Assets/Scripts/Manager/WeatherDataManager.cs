using UnityEngine;

// 날씨 타입별 방문객 수 보정 배수를 보관하고 조회하는 매니저.
// CustomerManager가 일일 방문객 수를 계산할 때 이 값을 참조한다.
// 각 보정값은 Inspector에서 조정 가능해 밸런싱 작업이 용이하다.
// 기준값(보정 없음) = 1.0, 1.0 미만은 방문객 감소, 1.0 초과는 방문객 증가를 의미한다.
public class WeatherDataManager : MonoBehaviour
{
    [Header("Weather Modifiers")]
    // 맑음: 방문객 수 보정 없음 (기준값)
    public float sunnyModifier = 1.0f;

    // 비: 우산 수요 증가 대신 외출 감소로 방문객 소폭 감소
    public float rainyModifier = 0.9f;

    // 폭염: 음료 수요 증가, 방문객은 소폭 증가
    public float heatwaveModifier = 1.05f;

    // 흐림: 외출 심리 저하로 방문객 감소
    public float cloudyModifier = 0.8f;

    // 눈: 이동 불편으로 방문객 가장 큰 폭 감소
    public float snowyModifier = 0.7f;

    // 날씨 타입에 해당하는 방문객 수 보정 배수를 반환한다.
    // 정의되지 않은 날씨 타입이 들어오면 보정 없음(1.0f)을 반환한다.
    public float GetModifier(WeatherType weather)
    {
        return weather switch
        {
            WeatherType.Sunny    => sunnyModifier,
            WeatherType.Rainy    => rainyModifier,
            WeatherType.Heatwave => heatwaveModifier,
            WeatherType.Cloudy   => cloudyModifier,
            WeatherType.Snowy    => snowyModifier,
            _                    => 1.0f
        };
    }
}
