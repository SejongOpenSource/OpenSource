using UnityEngine;

[CreateAssetMenu(fileName = "TradeData", menuName = "Game/TradeData")]
public class TradeData : ScriptableObject
{
    public ItemType productType;
    public int costPrice;
    public int sellPrice;
    public int Margin => sellPrice - costPrice;
}
