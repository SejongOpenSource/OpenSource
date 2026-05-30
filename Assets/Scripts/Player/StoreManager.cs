using System;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [Header("자산 관리")]
    public int currentMoney { get; private set; }// 현재 보유 금액

    public int currentDebt { get; private set; } // 현재 대출금
    public DistrictType currentZone { get; private set; }
    public DistrictData currentDistrictData { get; private set; }
    public event Action<int> OnPaymentSuccess; 
    public event Action OnPaymentFailed;
    public event Action<int> OnDebtChanged;
    
    public void Initialize()
    {
        currentMoney = 500000;
        currentDebt = 0;
        currentZone = DistrictType.Resident;
        
        // 2. DataManager.Instance에서 직접 Resident(거주구역) 데이터 꺼내오기
        if (DataManager.Instance != null)
        {
            currentDistrictData = DataManager.Instance.GetDistrict(DistrictType.Resident);
            
            if (currentDistrictData != null)
            {
                Debug.Log($"StoreManager: DataManager로부터 [{currentZone}] 데이터 로드 및 초기화 완료!");
            }
            else
            {
                Debug.LogError($"StoreManager: DataManager에 [{DistrictType.Resident}] 데이터가 없습니다! 인스펙터를 확인하세요.");
            }
        }
        else
        {
            Debug.LogError("StoreManager: 초기화 시점에 DataManager.Instance가 존재하지 않습니다! 실행 순서를 확인하세요.");
        }
        
        OnPaymentSuccess?.Invoke(currentMoney);
        OnDebtChanged?.Invoke(currentDebt);
        
        Debug.Log("StoreManager: 자산 및 상권 변수 초기화 완료");
    }
    
    // [소비] 보유 금액이 충분하면 차감 후 true 반환
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnPaymentSuccess?.Invoke(currentMoney);
            return true;
        }
        OnPaymentFailed?.Invoke();
        Debug.Log("자본금 부족");
        return false;
    }

    // [수익] 보유 금액 증가
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnPaymentSuccess?.Invoke(currentMoney);
    }

    // 부채 업데이트
    public void UpdateDebt(int amount)
    {
        currentDebt += amount;
        if (currentDebt < 0) currentDebt = 0;
        
        OnDebtChanged?.Invoke(currentDebt);
        Debug.Log($"StoreManager: 부채 변동 완료 -> 현재 총 부채: {currentDebt}원");
    }

    // 상권 업데이트
    public void SetDistrict(DistrictType zone, DistrictData data)
    {
        currentZone = zone;
        currentDistrictData = data;
        Debug.Log($"StoreManager: 상권 변경 완료 -> {zone} (보너스 수치: {data?.visitorBonus})");
    }
}
