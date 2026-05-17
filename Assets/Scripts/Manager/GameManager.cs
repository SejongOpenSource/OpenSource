using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int TargetSales { get; private set; } = 5000000;

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

    // TurnManager가 Result 페이즈 완료 시 호출. 게임 종료면 true 반환.
    public bool OnTurnEnd(int currentTurn, int maxTurns)
    {
        if (CheckLose(currentTurn, maxTurns)) return true;
        return false;
    }

    public int CalculateScore(int remainingStockCost, int remainingDebt)
    {
        return (TurnManager.Instance.MaxTurns - TurnManager.Instance.CurrentTurn) * 10000
               - remainingStockCost - remainingDebt;
    }

    private bool CheckLose(int currentTurn, int maxTurns)
    {
        if (currentTurn < maxTurns) return false;
        Debug.Log("패배!");
        return true;
    }
}
