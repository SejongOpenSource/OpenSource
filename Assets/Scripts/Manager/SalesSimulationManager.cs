using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SalesSimulationManager : MonoBehaviour
{
    public static SalesSimulationManager Instance { get; private set; }

    [Header("Base Settings")]
    public int baseVisitors = 20;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartSimulation()
    {
        StartCoroutine(SimulationRoutine());
    }

    private IEnumerator SimulationRoutine()
    {
        Debug.Log("영업 시뮬레이션 시작...");

        // 1. 데이터 가져오기
        Weather morning = GameManager.Instance.weatherSystem.morningWeather;
        Weather afternoon = GameManager.Instance.weatherSystem.afternoonWeather;
        Commerce currentCommerce = GameManager.Instance.storeManager.currentZone;
        DistrictData districtData = GameManager.Instance.GetComponent<CommerceZone>().GetDistrictData(currentCommerce);

        // 2. 방문객 수 계산
        int totalVisitors = CalculateVisitorCount(baseVisitors, districtData, morning, afternoon);
        Debug.Log($"오늘의 총 방문객: {totalVisitors}명");

        // 3. 시뮬레이션 진행 (추후 애니메이션 연동을 위해 코루틴으로 구현)
        for (int i = 0; i < totalVisitors; i++)
        {
            SimulateVisitor(districtData, morning, afternoon);
            // 시각적 연동을 위해 짧은 대기 (추후 조정)
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("영업 시뮬레이션 종료.");
        TurnManager.Instance.AdvancePhase();
    }

    private int CalculateVisitorCount(int baseCount, DistrictData district, Weather morning, Weather afternoon)
    {
        float visitorCount = baseCount;

        // 상권 보너스
        if (district != null)
        {
            visitorCount *= (1f + district.visitorBonus);
        }

        // 날씨 영향 (오전/오후 평균으로 적용)
        float weatherMod = (GetWeatherVisitorModifier(morning) + GetWeatherVisitorModifier(afternoon)) / 2f;
        visitorCount *= weatherMod;

        return Mathf.RoundToInt(visitorCount);
    }

    private float GetWeatherVisitorModifier(Weather weather)
    {
        return weather switch
        {
            Weather.Sunny => 1.0f,
            Weather.Rainy => 0.9f,
            Weather.Heatwave => 1.05f,
            Weather.Cloudy => 0.8f,
            Weather.Snowy => 0.7f,
            _ => 1.0f
        };
    }

    private void SimulateVisitor(DistrictData district, Weather morning, Weather afternoon)
    {
        // 각 상품별 구매 확률 계산
        // 기본 확률 (예: 각 20%)
        float[] probabilities = { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };

        // 상권 가중치 적용
        if (district != null)
        {
            probabilities[0] *= district.onigiriMult;
            probabilities[1] *= district.noodleMult;
            probabilities[2] *= district.drinkMult;
            probabilities[3] *= district.bentoMult;
            probabilities[4] *= district.umbrellaMult;
        }

        // 날씨 가중치 적용 (오전/오후 가중치를 어떻게 섞을지? 여기서는 단순화하여 합산 후 평균)
        ApplyWeatherProductWeights(probabilities, morning, true);
        ApplyWeatherProductWeights(probabilities, afternoon, false);

        // 가중치에 따른 상품 선택 (룰렛 휠 선택)
        float totalProb = 0;
        foreach (float p in probabilities) totalProb += p;

        float roll = Random.Range(0, totalProb);
        float cumulative = 0;
        int selectedIndex = -1;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (roll <= cumulative)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex != -1)
        {
            ProductType type = (ProductType)selectedIndex;
            // 재고 확인 후 판매
            if (GameManager.Instance.inventory.GetStock(type) > 0)
            {
                GameManager.Instance.inventory.Sell(type, 1);
                // Debug.Log($"손님이 {type}을(를) 구매했습니다.");
            }
            else
            {
                // Debug.Log($"{type} 재고가 없어 판매에 실패했습니다.");
            }
        }
    }

    private void ApplyWeatherProductWeights(float[] weights, Weather weather, bool isMorning)
    {
        switch (weather)
        {
            case Weather.Rainy:
                weights[4] *= (isMorning ? 3.0f : 2.0f); // 우산
                break;
            case Weather.Heatwave:
                weights[2] *= 2.0f; // 음료수
                break;
            case Weather.Cloudy:
                weights[1] *= 1.2f; // 컵라면
                break;
            case Weather.Snowy:
                // 기획서에는 핫 음료 x1.5라고 되어 있으나 현재 품목에는 없음. 
                // 임시로 컵라면 가중치 적용
                weights[1] *= 1.5f; 
                break;
            case Weather.Sunny:
                weights[4] *= 0.1f; // 맑은 날 우산 수요 급감
                break;
        }
    }
}
