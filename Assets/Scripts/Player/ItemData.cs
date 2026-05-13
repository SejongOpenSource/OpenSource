using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "GameData/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public int cost;   // 원가
    public int price;  // 판매가
    public Sprite icon;
}
