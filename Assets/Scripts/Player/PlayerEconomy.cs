using UnityEngine;

public partial class PlayerEconomy : MonoBehaviour
{
    [Header("자산")] 
    public float currentMoney = 500000;
    public float currentDebt = 0;
    private int _maxLoanAmount = 400000;
    private int _onceLoanAmount = 200000;
    // 소비
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

    // 수익
    public void AddMoney(float amount)
    {
        currentMoney += amount;
    }
    
    // 대출
    public bool Loan(int amount)
    {
        amount = Mathf.Clamp(amount, 0, _onceLoanAmount);
        
        if (currentDebt + amount <= _maxLoanAmount)
        {
            currentDebt += amount;
            return true;    
        }
        
        Debug.Log("대출 한도 초과!");
        return false;
    }

    public void AddInterest()
    {
        currentDebt *= 0.03f;
    }

    public void RepayLoan(float amount)
    {
        if (amount <= 0) return;
        
        float actualPayment = Mathf.Min(amount, currentDebt);
        currentDebt -= actualPayment;
        currentMoney -= actualPayment;
        
        Debug.Log($"{actualPayment}원을 상환했습니다. 남은 대출금: {currentDebt}");
    }
}
