using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("아이템 데이터 (Inspector에서 연결)")]
    public ItemData[] Items = new ItemData[System.Enum.GetNames(typeof(ItemType)).Length];

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

    // 영업 결과에 따른 재고 차감
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
        for (int i = 0; i < _stock.Length; i++)
        {
            _stock[i] += _pendingOrder[i];
            _pendingOrder[i] = 0;
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
}
