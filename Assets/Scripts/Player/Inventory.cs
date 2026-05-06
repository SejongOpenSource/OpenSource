using UnityEngine;

public enum ProductType { Onigiri, Ramen, Drink, Lunchbox, Umbrella }

public class Inventory : MonoBehaviour
{
    // 원가: Onigiri, Ramen, Drink, Lunchbox, Umbrella — TradeData 완성 후 교체
    private readonly int[] _costPrice = { 800, 700, 500, 3500, 2000 };

    private int[] _stock = new int[5];

    private StoreManager _storeManager;

    private void Start()
    {
        _storeManager = GetComponent<StoreManager>();
    }

    public int GetStock(ProductType type) => _stock[(int)type];

    public bool Order(ProductType type, int quantity)
    {
        int cost = _costPrice[(int)type] * quantity;
        if (!_storeManager.SpendMoney(cost)) return false;
        _stock[(int)type] += quantity;
        return true;
    }
}
