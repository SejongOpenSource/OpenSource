using UnityEngine;

public enum ProductType { Onigiri, Ramen, Drink, Lunchbox, Umbrella }

public class Inventory : MonoBehaviour
{
    private int[] _stock = new int[5];

    public int GetStock(ProductType type) => _stock[(int)type];
}
