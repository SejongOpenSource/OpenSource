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

    // 영업 시뮬레이션 결과 재고 차감
    public void UpdateStock(ItemType type, int count)
    {
        int i = (int)type;
        _stock[i] -= count;
        if (_stock[i] < 0) _stock[i] = 0;
    }

    // UI 직접 판매: 재고 충분 시 차감 후 판매 수량 반환, 부족 시 0 반환
    public int TrySell(ItemType type, int amount)
    {
        if (amount <= 0) return 0;

        int i = (int)type;
        if (_stock[i] < amount)
        {
            Debug.LogWarning($"재고 부족: {type} — 요청 {amount}개, 보유 {_stock[i]}개");
            return 0;
        }

        _stock[i] -= amount;
        return amount;
    }

    public void SetOrder(ItemType type, int count)
    {
        _pendingOrder[(int)type] = count;
    }

    public void FinalizeOrder()
    {
        for (int i = 0; i < _stock.Length; i++)
        {
            _stock[i] += _pendingOrder[i];
            _pendingOrder[i] = 0;
        }
    }

    public int CalculateStockPenalty()
    {
        if (DataManager.Instance == null || DataManager.Instance.itemDataManager == null)
            return 0;

        int penalty = 0;
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            ItemData item = DataManager.Instance.itemDataManager.GetItem(type);
            if (item != null)
                penalty += GetStock(type) * item.cost;
        }
        return penalty;
    }

    public int GetStock(ItemType type) => _stock[(int)type];

    public int GetPendingOrder(ItemType type) => _pendingOrder[(int)type];
}
