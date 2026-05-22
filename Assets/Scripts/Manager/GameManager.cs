using UnityEngine;

[RequireComponent(typeof(StoreManager))]
[RequireComponent(typeof(Loan))]
[RequireComponent(typeof(WeatherSystem))]
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

        if (storeManager == null) Debug.LogError("GameManager: StoreManager component is missing!");
        if (loan == null) Debug.LogError("GameManager: Loan component is missing!");
        if (weatherSystem == null) Debug.LogError("GameManager: WeatherSystem component is missing!");
    }

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
        if (SalesAlgorithm.Instance == null)
        {
            Debug.LogError("SalesAlgorithm 인스턴스를 찾을 수 없습니다.");
            return false;
        }
        if (SalesAlgorithm.Instance.TotalSales < TargetSales) return false;
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
