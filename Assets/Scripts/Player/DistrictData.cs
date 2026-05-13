using UnityEngine;

[CreateAssetMenu(fileName = "CommercialDistrictData", menuName = "GameData/District")]
public class DistrictData : ScriptableObject
{
    public string districtName;      // 상권 이름
    public int investmentCost;       // 투자 비용
    public float visitorBonus;       // 방문객 증가율 (예: 0.2f)
    
    [Header("Item Multipliers")]
    public float onigiriMult = 1.0f; // 삼각김밥 배율
    public float noodleMult = 1.0f;  // 컵라면 배율
    public float drinkMult = 1.0f;   // 음료수 배율
    public float bentoMult = 1.0f;   // 도시락 배율
    public float umbrellaMult = 1.0f;
}
