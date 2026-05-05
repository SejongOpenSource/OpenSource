using UnityEngine;

public partial class PlayerEconomy : MonoBehaviour
{
    [Header("자산")] 
    public int currentMoney = 500000;
    public int currentLoanAmount = 0;
    private int _maxLoanAmount = 400000;
    private int _onceLoanAmount = 200000;
    // 소비
    public bool SpendMoney(int amount)
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
    public void AddMoney(int amount)
    {
        currentMoney += amount;
    }
    
    // 대출
    public bool Loan(int amount)
    {
        amount = Mathf.Clamp(amount, 0, _onceLoanAmount);
        
        if (currentLoanAmount < _maxLoanAmount)
        {
            currentLoanAmount += amount;
            return true;    
        }
        
        Debug.Log("대출 한도 초과!");
        return false;
    }
}
