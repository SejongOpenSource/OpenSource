using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [Header("자산 관리")] 
    public int currentMoney { get; private set; } = 500000; // 현재 보유 금액
    public int currentDebt { get; private set; } = 0;        // 현재 대출금
    

    // [소비] 보유 금액이 충분하면 차감 후 true 반환
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

    // [수익] 보유 금액 증가
    public void AddMoney(int amount)
    {
        currentMoney += amount;
    }

    public void UpdateDebt(int amount)
    {
        currentDebt += amount;
        if (currentDebt < 0) currentDebt = 0;
    }
    
}
