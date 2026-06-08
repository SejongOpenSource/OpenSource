using UnityEngine;
using System.Collections.Generic;

// 영업 시뮬레이션을 담당하는 싱글톤 매니저.
// CustomerManager로부터 일일 방문객 수를 받아 각 손님이 구매할 상품을
// 상권·날씨 가중치 기반 확률로 결정하고 재고와 자본금을 갱신한다.
// 누적 매출(TotalSales)과 당일 매출(LastDailyRevenue)을 외부에 노출해
// 승패 판정 및 UI 표시에 활용된다.
public class SalesAlgorithm : MonoBehaviour
{
    // 씬 어디서든 SalesAlgorithm.Instance로 접근 가능한 싱글톤 인스턴스
    public static SalesAlgorithm Instance { get; private set; }

    // 게임 시작 이후 전체 누적 매출액 (승리 조건 판단에 사용)
    public int TotalSales { get; private set; } = 0;

    // 직전 영업일의 매출액 (Result 화면 표시용)
    public int LastDailyRevenue { get; private set; } = 0;

    // ItemType 열거형 전체 값을 캐싱 — 매 프레임 Enum.GetValues 호출 비용 절감
    private static readonly ItemType[] _itemTypes = (ItemType[])System.Enum.GetValues(typeof(ItemType));

    // 각 상품의 선택 확률 (상권·날씨 가중치 적용 후 갱신됨)
    private readonly Dictionary<ItemType, float> _probabilities = new Dictionary<ItemType, float>();

    // 당일 상품별 판매 수량 (Result UI에서 상품별 판매량 표시에 사용)
    private readonly Dictionary<ItemType, int> _lastSoldCounts = new Dictionary<ItemType, int>();

    // 시뮬레이션 시작 시점의 자본금 스냅샷 (당일 손익 계산용)
    public int MoneyBeforeSimulation { get; private set; }

    private void Awake()
    {
        // 씬 중복 로드 시 두 번째 인스턴스를 즉시 파기해 싱글톤 유일성 보장
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        if (transform.parent != null) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // 판매량 딕셔너리를 0으로 초기화
        foreach (ItemType t in _itemTypes)
            _lastSoldCounts[t] = 0;
    }

    // 누적 매출에 amount를 더한다. (외부에서 직접 매출을 추가할 때 사용)
    public void AddSales(int amount) => TotalSales += amount;

    // 영업 시뮬레이션을 실행한다.
    // 1) 시뮬레이션 시작 시점 자본금을 스냅샷으로 저장
    // 2) CustomerManager에서 일일 방문객 수를 받음
    // 3) 각 손님마다 상품을 확률로 선택 → 재고 확인 → 판매 처리
    // 4) 당일 매출을 자본금에 반영하고 TotalSales에 누적
    public void RunSimulation()
    {
        // 시뮬레이션 시작 시점 자본금 저장 (당일 손익 = 현재 자본금 - MoneyBeforeSimulation)
        MoneyBeforeSimulation = GameManager.Instance != null && GameManager.Instance.storeManager != null
            ? GameManager.Instance.storeManager.currentMoney
            : 0;

        // 당일 매출 및 판매량 초기화
        LastDailyRevenue = 0;
        foreach (ItemType t in _itemTypes)
            _lastSoldCounts[t] = 0;

        if (GameManager.Instance.customerManager == null)
        {
            Debug.LogError("CustomerManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // 상권·날씨 보정을 반영한 일일 방문객 수 계산
        int totalVisitors = GameManager.Instance.customerManager.CalculateVisitors();
        int dailyTotalRevenue = 0;

        DistrictData district = GameManager.Instance.storeManager.currentDistrictData;
        WeatherType morning = GameManager.Instance.weatherSystem.morningWeather;
        WeatherType afternoon = GameManager.Instance.weatherSystem.afternoonWeather;
        InventoryManager inventory = GameManager.Instance.inventoryManager;

        if (inventory == null)
        {
            Debug.LogError("SalesAlgorithm: InventoryManager가 GameManager에 연결되지 않았습니다.");
            return;
        }

        // 방문객 한 명씩 순차 처리: 상품 선택 → 재고 확인 → 판매
        for (int i = 0; i < totalVisitors; i++)
        {
            // 상권·날씨 가중치 기반으로 구매할 상품을 확률적으로 선택
            ItemType? chosenItem = PickItem(district, morning, afternoon);
            if (!chosenItem.HasValue) continue;

            // 재고가 없으면 해당 손님의 구매는 무효 처리
            if (inventory.GetStock(chosenItem.Value) <= 0) continue;

            ItemData item = DataManager.Instance.GetItem(chosenItem.Value);
            if (item == null) continue;

            // 재고 1 차감 및 당일 판매량 집계
            inventory.UpdateStock(chosenItem.Value, 1);
            _lastSoldCounts[chosenItem.Value]++;
            dailyTotalRevenue += item.price;
        }

        // 당일 매출을 자본금에 반영하고 누적 매출에 추가
        GameManager.Instance.storeManager.AddMoney(dailyTotalRevenue);
        AddSales(dailyTotalRevenue);
        LastDailyRevenue = dailyTotalRevenue;

        Debug.Log($"오늘의 총 매출: {dailyTotalRevenue}원");
    }

    // 상권·날씨 가중치를 적용한 확률 분포를 구성하고
    // 랜덤 롤을 통해 구매할 상품 타입을 반환한다.
    // 확률 총합이 0이거나 결과가 없으면 null을 반환한다.
    private ItemType? PickItem(DistrictData district, WeatherType morning, WeatherType afternoon)
    {
        _probabilities.Clear();

        // 기본 확률: 모든 상품 동일(0.2f = 각 20%)
        foreach (ItemType type in _itemTypes)
            _probabilities[type] = 0.2f;

        // 상권 보너스 적용 — 상권별 특화 상품의 확률이 높아짐
        if (district != null)
        {
            _probabilities[ItemType.Onigiri]  *= district.onigiriMult;
            _probabilities[ItemType.Noodle]   *= district.noodleMult;
            _probabilities[ItemType.Drink]    *= district.drinkMult;
            _probabilities[ItemType.Bento]    *= district.bentoMult;
            _probabilities[ItemType.Umbrella] *= district.umbrellaMult;
        }

        // 오전·오후 날씨 가중치를 순서대로 적용 (isMorning 플래그는 향후 확장용)
        ApplyWeatherProductWeights(_probabilities, morning, true);
        ApplyWeatherProductWeights(_probabilities, afternoon, false);

        // 확률 누적합(룰렛 휠 방식)으로 상품 선택
        float totalProb = 0;
        foreach (float p in _probabilities.Values) totalProb += p;

        float roll = Random.Range(0f, totalProb);
        float cumulative = 0;

        foreach (var kvp in _probabilities)
        {
            cumulative += kvp.Value;
            if (roll <= cumulative) return kvp.Key;
        }

        return null;
    }

    // 날씨에 따라 특정 상품의 선택 확률 가중치를 조정한다.
    // 현재 적용 규칙:
    //   - 비(Rainy): 우산 확률 ×2.0 (수요 급증)
    //   - 폭염(Heatwave): 음료수 확률 ×1.5 (갈증 수요 증가)
    //   - 그 외 날씨: 상품 확률 변경 없음
    private void ApplyWeatherProductWeights(Dictionary<ItemType, float> probs, WeatherType weather, bool isMorning)
    {
        switch (weather)
        {
            case WeatherType.Rainy:
                probs[ItemType.Umbrella] *= 2.0f;
                break;
            case WeatherType.Heatwave:
                probs[ItemType.Drink] *= 1.5f;
                break;
        }
    }

    // 당일 특정 상품의 판매 수량을 반환한다. (Result UI 표시용)
    // 등록되지 않은 타입이면 0을 반환한다.
    public int GetLastSoldCount(ItemType type) =>
        _lastSoldCounts.TryGetValue(type, out int v) ? v : 0;
}
