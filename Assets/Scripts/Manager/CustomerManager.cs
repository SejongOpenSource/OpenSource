using UnityEngine;

// 턴마다 편의점을 방문하는 고객 수를 계산하는 매니저.
// 기준 방문객 수(baseVisitors)에 현재 상권 보너스와 날씨 보정을 곱해 최종 방문객 수를 결정한다.
// SalesAlgorithm이 이 값을 받아 실제 판매 시뮬레이션을 수행한다.
public class CustomerManager : MonoBehaviour
{
    // 아무 보정도 없을 때의 기본 일일 방문객 수
    // Inspector에서 조정 가능하며, 난이도 밸런싱의 기준이 된다.
    [SerializeField] private int baseVisitors = 50;

    // 현재 상권과 날씨를 반영한 일일 방문객 수를 계산해 반환한다.
    // 공식: baseVisitors × (1 + 상권 보너스) × 날씨 보정 (오전·오후 평균)
    public int CalculateVisitors()
    {
        // 현재 선택된 상권의 방문객 보너스 배율 (예: 학원가 +0.8 → x1.8)
        // 상권 데이터가 없으면 0으로 처리해 기본 방문객 수를 유지
        float districtBonus = (GameManager.Instance.storeManager?.currentDistrictData != null)
            ? GameManager.Instance.storeManager.currentDistrictData.visitorBonus : 0f;

        // 오전·오후 날씨 보정값의 평균을 사용한다.
        // 날씨 시스템 또는 DataManager가 없으면 보정 없음(1.0)으로 처리
        float weatherMod = 1f;
        if (GameManager.Instance.weatherSystem != null && DataManager.Instance != null)
        {
            weatherMod = (DataManager.Instance.GetWeatherModifier(GameManager.Instance.weatherSystem.morningWeather)
                        + DataManager.Instance.GetWeatherModifier(GameManager.Instance.weatherSystem.afternoonWeather)) / 2f;
        }

        // 소수점 이하 반올림하여 정수 방문객 수 반환
        return Mathf.RoundToInt(baseVisitors * (1f + districtBonus) * weatherMod);
    }
}
