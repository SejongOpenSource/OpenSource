using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private int[] _stock;
    private int[] _pendingOrder;

    private void Awake()
    {
        int n = System.Enum.GetNames(typeof(ItemType)).Length;
        _stock = new int[n];
        _pendingOrder = new int[n];
    }

    // 영업 시뮬레이션 결과 재고 차감
    public void UpdateStock(ItemType type, int count)
    {
        int i = (int)type;
        _stock[i] -= count;
        if (_stock[i] < 0) _stock[i] = 0;
    }

    // 판매: 요청 수량만큼 재고가 있을 때만 차감하고, 실제 판매된 수량을 반환한다. 부족하면 경고만 하고 0.
    public int TrySell(ItemType type, int amount)
    {
        if (amount <= 0)
            return 0;

        int i = (int)type;
        int available = _stock[i];
        if (available < amount)
        {
            Debug.LogWarning($"재고 부족: {type} — 요청 {amount}개, 보유 {available}개");
            return 0;
        }

        _stock[i] = available - amount;
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
            if (item != null) totalCost += _pendingOrder[i] * item.cost;
        }

        var store = GameManager.Instance?.storeManager;
        if (store == null || !store.SpendMoney(totalCost))
            return false;

        for (int i = 0; i < _stock.Length; i++)
        {
            _stock[i] += _pendingOrder[i];
            _pendingOrder[i] = 0;
        }
        return true;
    }

    // 게임 종료 시 남은 재고 원가 합산 (점수 차감용) || 해당 매서드를 GameManager에서 호출하여 점수 계산에 사용.
    // 여기서 만들어야하는 이유는 확인 필요.
    public int CalculateStockPenalty()
    {
        if (DataManager.Instance == null || DataManager.Instance.itemDataManager == null)
            return 0;

        int penalty = 0;
        var itemDataManager = DataManager.Instance.itemDataManager;

        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            ItemData item = itemDataManager.GetItem(type);
            if (item != null)
                penalty += GetStock(type) * item.cost;
        }
        return penalty;
    }

    public int GetStock(ItemType type) => _stock[(int)type];

    public int GetPendingOrder(ItemType type) => _pendingOrder[(int)type];
}
