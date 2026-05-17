using UnityEngine;

public class DistrictSystem : MonoBehaviour
{
    [Header("참조 설정")]
    public StoreManager storeManager; // 자본금 및 현재 상태를 관리하는 매니저
    
    /// <summary>
    /// 상권을 업그레이드 하는 메인 함수
    /// </summary>
    /// <param name="c">변경하려는 대상 상권 타입</param>
    public void UpgradeCommerceZone(DistrictType c)
    {
        // 1. 중복 체크: 이미 동일한 상권인 경우 불필요한 비용 지출 방지
        if (storeManager.currentZone == c && storeManager.currentDistrictData != null) 
        {
            Debug.Log($"이미 {c} 상권이 적용 중입니다.");
            return; 
        }

        // 2. 데이터 매핑: 선택한 상권 타입에 맞는 데이터 오브젝트(SO)를 가져옴
        DistrictData targetData = DistrictData.CreateInstance<DistrictData>(); // => DataManager.DistrictDataManager로 변경

        // 데이터가 비어있으면(Null) 로직 중단
        if (targetData == null) return;

        // 3. 결제 시도: StoreManager를 통해 자본금 확인 및 차감 시도
        // 결제에 성공(true 반환)했을 때만 상권 데이터 교체 진행
        if (storeManager.SpendMoney(targetData.investmentCost))
        {
            // 4. 데이터 갱신: 현재 적용된 상권 타입과 데이터 참조(Reference) 업데이트
            storeManager.SetDistrict(c, targetData);
            Debug.Log($"{c} 상권으로 업그레이드 완료!");
        }
    }
}