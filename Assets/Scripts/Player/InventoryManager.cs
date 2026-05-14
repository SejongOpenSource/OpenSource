using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("아이템 데이터 (Inspector에서 연결)")]
    public ItemData[] Items = new ItemData[System.Enum.GetNames(typeof(ItemType)).Length];

    private int[] _stock = new int[System.Enum.GetNames(typeof(ItemType)).Length];        // 실제 재고
    private int[] _pendingOrder = new int[System.Enum.GetNames(typeof(ItemType)).Length]; // 발주 대기 수량

    // 영업 결과에 따른 재고 차감 및 매출 기록
    public void Sell(ItemType type, int count)
    {
        int i = (int)type;
        if (Items[i] == null) return;

        // 실제 판매 가능 수량 계산 (재고보다 많이 팔 수 없음)
        int actualSellCount = Mathf.Min(count, _stock[i]);
        
        if (actualSellCount > 0)
        {
            _stock[i] -= actualSellCount;
            int sales = actualSellCount * Items[i].price;
            GameManager.Instance.AddSales(sales);
            GameManager.Instance.storeManager.AddMoney(sales); // 기획에 따라 매출을 즉시 자본금에 더할지 결정
            Debug.Log($"판매: {type} {actualSellCount}개, 매출: {sales}원");
        }
    }

    // (기존 UpdateStock은 하위 호환성을 위해 유지하거나 Sell로 대체 가능)
    public void UpdateStock(ItemType type, int count)
    {
        int i = (int)type;
        _stock[i] -= count;
        if (_stock[i] < 0) _stock[i] = 0;
    }

    // 발주 수량 임시 저장
    public void SetOrder(ItemType type, int count)
    {
        _pendingOrder[(int)type] = count;
    }

    // 발주 확정 → 임시 수량을 실제 재고로 전환
    public void FinalizeOrder()
    {
        int totalCost = 0;
        for (int i = 0; i < _pendingOrder.Length; i++)
        {
            if (_pendingOrder[i] > 0 && Items[i] != null)
            {
                totalCost += _pendingOrder[i] * Items[i].cost;
            }
        }

        if (totalCost > 0)
        {
            if (GameManager.Instance.storeManager.SpendMoney(totalCost))
            {
                for (int i = 0; i < _stock.Length; i++)
                {
                    _stock[i] += _pendingOrder[i];
                    _pendingOrder[i] = 0;
                }
                Debug.Log($"발주 성공: {totalCost}원 차감");
            }
            else
            {
                Debug.Log("발주 실패: 자본금 부족");
                // 발주 실패 시 pendingOrder를 어떻게 처리할지 기획에 따라 다르지만, 
                // 여기서는 초기화하여 재시도하게 함.
                for (int i = 0; i < _pendingOrder.Length; i++) _pendingOrder[i] = 0;
            }
        }
    }

    // 게임 종료 시 남은 재고 원가 합산 (점수 차감용)
    public int CalculateStockPenalty()
    {
        int penalty = 0;
        for (int i = 0; i < _stock.Length; i++)
        {
            if (Items[i] != null) penalty += _stock[i] * Items[i].cost;
        }
        return penalty;
    }

    // 현재 재고 조회 (UI 연결용)
    public int GetStock(ItemType type) => _stock[(int)type];

    // 발주 대기 수량 조회 (UI 연결용)
    public int GetPendingOrder(ItemType type) => _pendingOrder[(int)type];

    // [Test] 로직 검증용 (필요 시 호출)
    public void TestInventorySystem()
    {
        Debug.Log("--- Inventory System Test Start ---");
        // 1. 발주 테스트
        SetOrder(ItemType.Onigiri, 10);
        FinalizeOrder();
        Debug.Log($"Onigiri Stock after order: {GetStock(ItemType.Onigiri)}");

        // 2. 판매 테스트
        Sell(ItemType.Onigiri, 5);
        Debug.Log($"Onigiri Stock after sell: {GetStock(ItemType.Onigiri)}");
        Debug.Log($"Total Sales in GameManager: {GameManager.Instance.TotalSales}");

        // 3. 재고 페널티 계산 테스트
        int penalty = CalculateStockPenalty();
        Debug.Log($"Current Stock Penalty: {penalty}");
        Debug.Log("--- Inventory System Test End ---");
    }
}
