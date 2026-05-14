using UnityEngine;
using System.Collections.Generic;

public class ItemDataManager : MonoBehaviour
{
    [Header("Item SO List")]
    public List<TradeData> items = new List<TradeData>();

    public TradeData GetItem(ItemType type)
    {
        // TradeData list에서 매칭되는 데이터 반환
        return items.Find(i => i != null && i.itemType == type);
    }
}
