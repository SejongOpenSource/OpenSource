using UnityEngine;
using System.Collections.Generic;

public class ItemDataManager : MonoBehaviour
{
    [Header("Item SO List")]
    [SerializeField] private List<TradeData> items = new List<TradeData>();
    private Dictionary<ItemType, TradeData> itemDict = new Dictionary<ItemType, TradeData>();

    public void Initialize()
    {
        itemDict.Clear();
        foreach (var item in items)
        {
            if (item != null) itemDict[item.itemType] = item;
        }
    }

    public TradeData GetItem(ItemType type)
    {
        itemDict.TryGetValue(type, out TradeData result);
        return result;
    }
}
