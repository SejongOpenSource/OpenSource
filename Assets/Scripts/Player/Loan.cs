using UnityEngine;

public class Loan : MonoBehaviour
{
    public StoreManager storeManager;

    [Header("대출 정보")]
    private int _maxLoanAmount = 400000;   // 대출 최대 한도
    private int _onceLoanAmount = 200000;  // 1회당 대출 가능액

    // 대출 이자율
    // LoanPanelController에서도 같은 값을 사용하도록 public const로 관리
    public const float InterestRate = 0.03f;

    // 1회 대출 가능 금액 반환
    public int GetOnceLoanAmount()
    {
        return _onceLoanAmount;
    }

    // 전체 대출 최대 한도 반환
    public int GetMaxLoanAmount()
    {
        return _maxLoanAmount;
    }

    // 남은 총 대출 가능 금액 반환
    public int GetRemainingLoanLimit()
    {
        // StoreManager가 연결되지 않았으면 대출 가능 금액 없음
        if (storeManager == null)
        {
            return 0;
        }

        int remainingLimit = _maxLoanAmount - storeManager.currentDebt;

        if (remainingLimit < 0)
        {
            remainingLimit = 0;
        }

        return remainingLimit;
    }

    // 현재 실제로 선택 가능한 최대 대출 금액 반환
    // 1회 한도와 남은 전체 한도 중 더 작은 값
    public int GetAvailableLoanAmount()
    {
        int remainingLimit = GetRemainingLoanLimit();
        return System.Math.Min(_onceLoanAmount, remainingLimit);
    }

    // [대출] 한도 내에서 대출 실행
    public bool TakeOutLoan(int amount)
    {
        // StoreManager가 연결되지 않았으면 대출 실행 불가
        if (storeManager == null)
        {
            Debug.LogError("StoreManager가 연결되지 않았습니다.");
            return false;
        }

        // 대출 금액이 0원이면 실행하지 않음
        if (amount <= 0)
        {
            Debug.Log("대출 금액이 0원입니다.");
            return false;
        }

        // 현재 실제 대출 가능한 최대 금액 계산
        int availableLoanAmount = GetAvailableLoanAmount();

        // 슬라이더를 쓰더라도 외부에서 잘못된 값이 들어올 수 있으므로 한 번 더 방어
        if (amount > availableLoanAmount)
        {
            Debug.LogWarning($"대출 가능 금액 초과: 요청 {amount:N0}원 / 가능 {availableLoanAmount:N0}원");
            return false;
        }

        // 대출 실행
        storeManager.UpdateDebt(amount);
        storeManager.AddMoney(amount);

        Debug.Log($"{amount:N0}원 대출 완료. 현재 부채: {storeManager.currentDebt:N0}원");
        return true;
    }

    // [이자] 대출금의 3%만큼 이자 추가
    public void AddInterest()
    {
        // StoreManager가 연결되지 않았으면 이자 계산 불가
        if (storeManager == null)
        {
            Debug.LogError("StoreManager가 연결되지 않았습니다.");
            return;
        }

        // 소수점 이하를 단순 절삭하지 않고 반올림
        int interest = Mathf.RoundToInt(storeManager.currentDebt * InterestRate);

        storeManager.UpdateDebt(interest);

        Debug.Log($"이자 추가: {interest:N0}원. 현재 부채: {storeManager.currentDebt:N0}원");
    }

    // [상환] 대출금을 갚고 보유 금액에서 차감
    public void RepayLoan(int amount)
    {
        // StoreManager가 연결되지 않았으면 상환 불가
        if (storeManager == null)
        {
            Debug.LogError("StoreManager가 연결되지 않았습니다.");
            return;
        }

        // 상환 금액이 0 이하이거나 부채가 없으면 실행하지 않음
        if (amount <= 0 || storeManager.currentDebt <= 0)
        {
            return;
        }

        // 실제 상환 금액은 입력 금액과 현재 부채 중 더 작은 값
        int actualPayment = System.Math.Min(amount, storeManager.currentDebt);

        // 돈이 충분할 때만 부채를 차감
        if (storeManager.SpendMoney(actualPayment))
        {
            storeManager.UpdateDebt(-actualPayment);
            Debug.Log($"{actualPayment:N0}원 상환 완료. 현재 부채: {storeManager.currentDebt:N0}원");
        }
        else
        {
            Debug.Log("잔액이 부족하여 상환할 수 없습니다.");
        }
    }
}