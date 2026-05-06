using UnityEngine;

public partial class PlayerEconomy : MonoBehaviour
{
    [Header("자산 관리")] 
    public float currentMoney = 500000;  // 현재 보유 금액
    public float currentDebt = 0;        // 현재 대출금
    private int _maxLoanAmount = 400000; // 대출 최대 한도
    private int _onceLoanAmount = 200000; // 1회당 대출 가능액

    // [소비] 보유 금액이 충분하면 차감 후 true 반환
    public bool SpendMoney(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            return true;
        }

        Debug.Log("자본금 부족");
        return false;
    }

    // [수익] 보유 금액 증가
    public void AddMoney(float amount)
    {
        currentMoney += amount;
    }
    
    // [대출] 한도 내에서 대출 실행 
    public bool Loan(float amount)
    {
        // 입력값을 0 ~ 1회 제한량 사이로 고정
        amount = Mathf.Clamp(amount, 0, _onceLoanAmount);
        
        // 최대 한도를 초과하지 않는지 확인
        if (currentDebt + amount <= _maxLoanAmount)
        {
            currentDebt += amount;
            AddMoney(amount);
            return true;    
        }
        
        Debug.Log("대출 한도 초과!");
        return false;
    }

    // [이자] 대출금의 3%만큼 이자 추가
    public void AddInterest()
    {
        currentDebt += (currentDebt * 0.03f);
    }

    // [상환] 대출금을 갚고 보유 금액에서 차감
    public void RepayLoan(float amount)
    {
        if (amount <= 0) return;
        
        // 상환액이 남은 대출금보다 크지 않도록 설정
        float actualPayment = Mathf.Min(amount, currentDebt);
        
        currentDebt -= actualPayment;
        currentMoney -= actualPayment;
        
        Debug.Log($"{actualPayment}원을 상환했습니다. 남은 대출금: {currentDebt}");
    }
}
