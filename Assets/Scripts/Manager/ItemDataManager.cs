using UnityEngine;
using System.Collections.Generic;

public class ItemDataManager : MonoBehaviour
{
    [Header("Item SO List")]
    public List<ItemData> items = new List<ItemData>();

    public ItemData GetItem(ItemType type)
    {
        // 간단하게 이름이나 타입 매칭 (ItemData에 ItemType 필드가 있는 것이 좋음)
        // 여기서는 인덱스로 접근하거나 이름을 찾음.
        // ItemData를 확장하여 ItemType을 포함하도록 수정 필요.
        return items.Find(i => i != null && i.itemType == type);
    }
}
