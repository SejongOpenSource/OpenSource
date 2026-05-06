using UnityEngine;

public class Loan : MonoBehaviour
{
    public StoreManager storeManager;
    
    [Header("대출 정보")]
    private int _maxLoanAmount = 400000; // 대출 최대 한도
    private int _onceLoanAmount = 200000; // 1회당 대출 가능액
    private float interestRate = 0.03f;

    // [대출] 한도 내에서 대출 실행 
    public bool TakeOutLoan(int amount)
    {
        // 입력값을 0 ~ 1회 제한량 사이로 고정
        amount = Mathf.Clamp(amount, 0, _onceLoanAmount);
        
        // 최대 한도를 초과하지 않는지 확인
        if (storeManager.currentDebt + amount <= _maxLoanAmount)
        {
            storeManager.UpdateDebt(amount);
            storeManager.AddMoney(amount);
            return true;    
        }
        
        Debug.Log("대출 한도 초과!");
        return false;
    }

    // [이자] 대출금의 3%만큼 이자 추가
    public void AddInterest()
    {
        int interest = (int)(storeManager.currentDebt * interestRate);
        storeManager.UpdateDebt(interest);
    }

    // [상환] 대출금을 갚고 보유 금액에서 차감
    public void RepayLoan(int amount)
    {
        if (amount <= 0 || storeManager.currentDebt <= 0) return;

        int actualPayment = Mathf.Min(amount, storeManager.currentDebt);

        // 돈이 충분할 때만 부채를 탕감
        if (storeManager.SpendMoney(actualPayment))
        {
            storeManager.UpdateDebt(-actualPayment);
            Debug.Log($"{actualPayment}원 상환 완료. 잔액: {storeManager.currentDebt}");
        }
        else
        {
            Debug.Log("잔액이 부족하여 상환할 수 없습니다.");
        }
    }
}
