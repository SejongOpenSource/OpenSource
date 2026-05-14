using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private int[] _stock;
    private int[] _pendingOrder;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        int n = System.Enum.GetNames(typeof(ItemType)).Length;
        _stock = new int[n];
        _pendingOrder = new int[n];
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // CalculateStockPenalty 매서드 구현에 필요한 매서드
    private bool TryGetItemData(ItemType type, out ItemData data)
    {
        data = null;
        if (DataManager.Instance == null || DataManager.Instance.itemDataManager == null)
            return false;
        data = DataManager.Instance.itemDataManager.GetItem(type);
        return data != null;
    }

    // 영업 결과에 따른 재고 차감
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

    // 발주 확정 → 임시 수량을 실제 재고로 전환
    public void FinalizeOrder()
    {
        for (int i = 0; i < _stock.Length; i++)
        {
            _stock[i] += _pendingOrder[i];
            _pendingOrder[i] = 0;
        }
    }

    // 게임 종료 시 남은 재고 원가 합산 (점수 차감용) || 해당 매서드를 GameManager에서 호출하여 점수 계산에 사용.
    // 여기서 만들어야하는 이유는 확인 필요.
    public int CalculateStockPenalty()
    {
        int penalty = 0;
        // ItemType 열거형에 정의된 모든 값을 한 번씩 돈다. (enum 값 = _stock 인덱스와 1:1)
        // 재고 수량은 _stock[(int)type], 원가 등 메타는 DataManager → ItemData에서 조회한다.
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            if (!TryGetItemData(type, out var item))
                continue;
            penalty += GetStock(type) * item.cost;
        }
        return penalty;
    }

    // 현재 재고 조회 (UI 연결용)
    public int GetStock(ItemType type) => _stock[(int)type];

    // 발주 대기 수량 조회 (UI 연결용)
    public int GetPendingOrder(ItemType type) => _pendingOrder[(int)type];
}
