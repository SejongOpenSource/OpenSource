using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [Header("자산 관리")]
    public int currentMoney { get; private set; } // 현재 보유 금액
    public int currentDebt { get; private set; }  // 현재 대출금

    [Header("상권 관리")]
    public DistrictType currentZone { get; private set; }
    public DistrictData currentDistrictData { get; private set; }

    // 구매한 상권 목록
    // UI가 아니라 StoreManager에서 관리해야 구매 상태가 유지됨
    private HashSet<DistrictType> purchasedDistricts = new HashSet<DistrictType>();

    public event Action<int> OnPaymentSuccess;
    public event Action OnPaymentFailed;
    public event Action<int> OnDebtChanged;

    public void Initialize()
    {
        currentMoney = 50000;
        currentDebt = 0;
        currentZone = DistrictType.Resident;

        // 기본 상권은 처음부터 보유 상태
        purchasedDistricts.Clear();
        purchasedDistricts.Add(DistrictType.Resident);

        // DataManager에서 기본 상권 데이터 가져오기
        if (DataManager.Instance != null)
        {
            currentDistrictData = DataManager.Instance.GetDistrict(currentZone);

            if (currentDistrictData != null)
            {
                Debug.Log($"StoreManager: DataManager로부터 [{currentZone}] 데이터 로드 및 초기화 완료!");
            }
            else
            {
                Debug.LogError($"StoreManager: DataManager에 [{currentZone}] 데이터가 없습니다! 인스펙터를 확인하세요.");
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

        if (currentDebt < 0)
        {
            currentDebt = 0;
        }

        OnDebtChanged?.Invoke(currentDebt);
        Debug.Log($"StoreManager: 부채 변동 완료 -> 현재 총 부채: {currentDebt}원");
    }

    // 현재 적용 상권 업데이트
    public void SetDistrict(DistrictType zone, DistrictData data)
    {
        currentZone = zone;
        currentDistrictData = data;
        Debug.Log($"StoreManager: 상권 변경 완료 -> {zone} (보너스 수치: {data?.visitorBonus})");
    }

    // 상권 구매 여부 확인
    public bool IsDistrictPurchased(DistrictType zone)
    {
        return purchasedDistricts.Contains(zone);
    }

    // 구매한 상권 목록 반환
    public List<DistrictType> GetPurchasedDistricts()
    {
        return new List<DistrictType>(purchasedDistricts);
    }

    // 상권 구매 처리
    public bool PurchaseDistrict(DistrictType zone)
    {
        // 이미 구매한 상권이면 다시 구매하지 않음
        if (IsDistrictPurchased(zone))
        {
            Debug.Log($"{zone} 상권은 이미 구매했습니다.");
            return false;
        }

        // DataManager가 없으면 상권 데이터를 가져올 수 없음
        if (DataManager.Instance == null)
        {
            Debug.LogError("StoreManager: DataManager.Instance가 없습니다.");
            return false;
        }

        // 구매하려는 상권 데이터 가져오기
        DistrictData targetData = DataManager.Instance.GetDistrict(zone);

        if (targetData == null)
        {
            Debug.LogError($"StoreManager: {zone} 상권 데이터를 찾을 수 없습니다.");
            return false;
        }

        // 구매 비용 차감
        if (SpendMoney(targetData.investmentCost) == false)
        {
            Debug.LogWarning($"{zone} 상권 구매 실패: 자본금 부족");
            return false;
        }

        // 구매한 상권 목록에 추가
        purchasedDistricts.Add(zone);

        // 기존 판매 계산 구조는 currentDistrictData를 사용하므로
        // 마지막으로 구매한 상권을 현재 적용 상권으로도 설정
        SetDistrict(zone, targetData);

        Debug.Log($"{zone} 상권 구매 완료");
        return true;
    }
}