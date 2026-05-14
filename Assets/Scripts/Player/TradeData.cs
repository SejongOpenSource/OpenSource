using UnityEngine;

[CreateAssetMenu(fileName = "New TradeData", menuName = "GameData/TradeData")]
public class TradeData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public int cost;
    public int price;
    public Sprite icon;
}
