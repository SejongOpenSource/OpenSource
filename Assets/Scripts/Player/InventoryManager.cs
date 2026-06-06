using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private int[] _stock;
    private int[] _pendingOrder;

    // 마지막으로 확정된 발주 수량
    // 결과 화면에서 주문수량 표시용으로 사용
    private int[] _lastOrder;

    // 마지막 영업에서 실제 판매된 수량
    // 결과 화면에서 판매수량 표시용으로 사용
    private int[] _lastSold;

    // 마지막 발주 확정 시점의 자본금
    // 결과 화면에서 시작 자본금 표시용으로 사용
    private int _lastStartMoney;

    public void Initialize()
    {
        int n = System.Enum.GetNames(typeof(ItemType)).Length;

        _stock = new int[n];
        _pendingOrder = new int[n];
        _lastOrder = new int[n];
        _lastSold = new int[n];
        _lastStartMoney = 0;
    }

    // 영업 시뮬레이션 결과 재고 차감
    public void UpdateStock(ItemType type, int count)
    {
        if (count <= 0)
        {
            return;
        }

        int i = (int)type;

        // 실제로 차감 가능한 수량만 판매 처리
        int soldCount = System.Math.Min(count, _stock[i]);

        _stock[i] -= soldCount;

        if (_stock[i] < 0)
        {
            _stock[i] = 0;
        }

        // 이번 영업에서 실제로 판매된 수량 저장
        _lastSold[i] += soldCount;
    }

    // 판매: 요청 수량만큼 재고가 있을 때만 차감하고, 실제 판매된 수량을 반환한다. 부족하면 경고만 하고 0.
    public int TrySell(ItemType type, int amount)
    {
        if (amount <= 0)
        {
            return 0;
        }

        int i = (int)type;
        int available = _stock[i];

        if (available < amount)
        {
            Debug.LogWarning($"재고 부족: {type} — 요청 {amount}개, 보유 {available}개");
            return 0;
        }

        _stock[i] = available - amount;

        // TrySell을 사용하는 판매 로직도 판매량에 반영
        _lastSold[i] += amount;

        return amount;
    }

    // 발주 수량 임시 저장
    public void SetOrder(ItemType type, int count)
    {
        _pendingOrder[(int)type] = count;
    }

    // 발주 확정 → 비용 차감 성공 시 임시 수량을 실제 재고로 전환. 자산 부족 시 false 반환.
    public bool FinalizeOrder()
    {
        int totalCost = 0;

        var itemDataManager = DataManager.Instance?.itemDataManager;

        for (int i = 0; i < _pendingOrder.Length; i++)
        {
            ItemData item = itemDataManager?.GetItem((ItemType)i);

            if (item != null)
            {
                totalCost += _pendingOrder[i] * item.cost;
            }
        }

        var store = GameManager.Instance?.storeManager;

        if (store == null)
        {
            return false;
        }

        // 발주 비용이 차감되기 전 자본금을 저장
        // 결과 화면의 시작 자본금으로 사용
        _lastStartMoney = store.currentMoney;

        if (store.SpendMoney(totalCost) == false)
        {
            return false;
        }

        for (int i = 0; i < _stock.Length; i++)
        {
            // 이번 턴에 실제로 확정된 발주 수량 저장
            _lastOrder[i] = _pendingOrder[i];

            // 새 영업 결과를 받기 전에 판매량 초기화
            _lastSold[i] = 0;

            // 발주 수량을 실제 재고에 반영
            _stock[i] += _pendingOrder[i];

            // 임시 발주 수량 초기화
            _pendingOrder[i] = 0;
        }

        return true;
    }

    // 남은 재고 전부 팔았을 때 예상 수익 (파산 판단용)
    public int CalculateStockRevenue()
    {
        if (DataManager.Instance?.itemDataManager == null) return 0;
        int total = 0;
        var itemDataManager = DataManager.Instance.itemDataManager;
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            ItemData item = itemDataManager.GetItem(type);
            if (item != null) total += GetStock(type) * item.price;
        }
        return total;
    }

    // 게임 종료 시 남은 재고 원가 합산
    public int CalculateStockPenalty()
    {
        if (DataManager.Instance == null || DataManager.Instance.itemDataManager == null)
        {
            return 0;
        }

        int penalty = 0;
        var itemDataManager = DataManager.Instance.itemDataManager;

        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            ItemData item = itemDataManager.GetItem(type);

            if (item != null)
            {
                penalty += GetStock(type) * item.cost;
            }
        }

        return penalty;
    }

    public int GetStock(ItemType type)
    {
        return _stock[(int)type];
    }

    public int GetPendingOrder(ItemType type)
    {
        return _pendingOrder[(int)type];
    }

    // 결과 화면에서 마지막 발주량을 가져올 때 사용
    public int GetLastOrder(ItemType type)
    {
        return _lastOrder[(int)type];
    }

    // 결과 화면에서 마지막 판매량을 가져올 때 사용
    public int GetLastSold(ItemType type)
    {
        return _lastSold[(int)type];
    }

    // 결과 화면에서 시작 자본금을 가져올 때 사용
    public int GetLastStartMoney()
    {
        return _lastStartMoney;
    }
}