using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int TotalSales { get; private set; } = 0;
    public int TargetSales { get; private set; } = 5000000;

    // Sub-systems registry (Singletons are preferred, but this composition is also fine as per current code)
    [HideInInspector] public StoreManager storeManager;
    [HideInInspector] public Loan loan;
    [HideInInspector] public WeatherSystem weatherSystem;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        storeManager = GetComponent<StoreManager>();
        loan = GetComponent<Loan>();
        weatherSystem = GetComponent<WeatherSystem>();
    }

    public void AddSales(int amount) => TotalSales += amount;

    // TurnManager에서 Result 페이즈 종료 시 호출
    public bool OnTurnEnd(int currentTurn, int maxTurns)
    {
        if (CheckWin()) return true;
        if (CheckLose(currentTurn, maxTurns)) return true;
        return false;
    }

    public int CalculateScore(int remainingStockCost, int remainingDebt)
    {
        return (TurnManager.Instance.MaxTurns - TurnManager.Instance.CurrentTurn) * 10000 
               - remainingStockCost - remainingDebt;
    }

    private bool CheckWin()
    {
        if (TotalSales < TargetSales) return false;
        Debug.Log("승리!");
        return true;
    }

    private bool CheckLose(int currentTurn, int maxTurns)
    {
        if (currentTurn < maxTurns) return false;
        Debug.Log("패배!");
        return true;
    }
}
