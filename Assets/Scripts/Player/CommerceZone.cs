using UnityEngine;

public enum Commerce { Resident, Academy, Campus, Business, Tourist }

public class CommerceZone : MonoBehaviour
{
    public StoreManager storeManager;
    
    [Header("상권 데이터")]
    public DistrictData residentData;
    public DistrictData  academyData;
    public DistrictData  campusData;
    public DistrictData  businessData;
    public DistrictData  touristData;
    
    public void UpgradeCommerceZone(Commerce c)
    {
        DistrictData targetData = GetDistrictData(c); 

        if (targetData == null) return;

        if (storeManager.SpendMoney(targetData.investmentCost))
        {
            storeManager.currentZone = c;
            
            // 요소 값 변경 로직 추가
        }
    }

    private DistrictData GetDistrictData(Commerce c)
    {
        return c switch
        {
            Commerce.Resident => residentData,
            Commerce.Academy => academyData,
            Commerce.Campus => campusData,
            Commerce.Business => businessData,
            Commerce.Tourist => touristData,
            _ => null
        };
    }
}
