using UnityEngine;

public enum ProductType { Onigiri, Ramen, Drink, Lunchbox, Umbrella }

public class Inventory : MonoBehaviour
{
    public TradeData[] products; // Inspector에서 5종 ScriptableObject 할당

    private int[] _stock = new int[System.Enum.GetValues(typeof(ProductType)).Length];

    private StoreManager _storeManager;

    private void Start()
    {
        _storeManager = GetComponent<StoreManager>();
        
        if (products.Length != System.Enum.GetValues(typeof(ProductType)).Length)
        {
            Debug.LogError($"products 배열의 길이({products.Length})가 ProductType 열거형의 요소 수({System.Enum.GetValues(typeof(ProductType)).Length})와 일치하지 않습니다.");
        }
    }

    public int GetStock(ProductType type) => _stock[(int)type];

    public bool Order(ProductType type, int quantity)
    {
        int cost = products[(int)type].costPrice * quantity;
        if (!_storeManager.SpendMoney(cost)) return false;
        _stock[(int)type] += quantity;
        return true;
    }

    public int Sell(ProductType type, int quantity)
    {
        int actualSold = Mathf.Min(quantity, _stock[(int)type]);
        _stock[(int)type] -= actualSold;

        int revenue = products[(int)type].sellPrice * actualSold;
        _storeManager.AddMoney(revenue);
        GameManager.Instance.AddSales(revenue);
        return revenue;
    }

    public int GetRemainingStockCost()
    {
        int total = 0;
        for (int i = 0; i < _stock.Length; i++)
            total += _stock[i] * products[i].costPrice;
        return total;
    }
}
