using UnityEngine;
using System.Collections.Generic;

// 게임에 등장하는 모든 상품(ItemData) ScriptableObject를 목록으로 보관하고
// ItemType으로 빠르게 조회할 수 있도록 제공하는 매니저.
// DataManager의 하위 컴포넌트로 동작하며, DataManager.GetItem()을 통해 외부에 노출된다.
public class ItemDataManager : MonoBehaviour
{
    // Inspector에서 상품별 ScriptableObject(ItemData)를 직접 등록하는 목록.
    // 삼각김밥, 컵라면, 음료수, 도시락, 우산 5종을 순서에 관계없이 추가한다.
    [Header("Item SO List")]
    public List<ItemData> items = new List<ItemData>();

    // ItemType enum 값으로 해당 상품 데이터를 찾아 반환한다.
    // 리스트에서 itemType 필드가 일치하는 첫 번째 ItemData를 반환하며,
    // 없으면 null을 반환한다.
    public ItemData GetItem(ItemType type)
    {
        return items.Find(i => i != null && i.itemType == type);
    }
}
