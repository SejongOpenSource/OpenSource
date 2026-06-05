using UnityEngine;

[RequireComponent(typeof(StoreManager))]
[RequireComponent(typeof(Loan))]
[RequireComponent(typeof(WeatherSystem))]
[RequireComponent(typeof(InventoryManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int TargetSales { get; private set; } = 5000000;

    // 게임 종료 시 발생 (true = 승리, false = 패배). UI(결과 화면 등)에서 구독해 전환 처리.
    public event System.Action<bool> OnGameOver;

    [HideInInspector] public StoreManager storeManager;
    [HideInInspector] public Loan loan;
    [HideInInspector] public WeatherSystem weatherSystem;
    [HideInInspector] public InventoryManager inventoryManager;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        storeManager = GetComponent<StoreManager>();
        loan = GetComponent<Loan>();
        weatherSystem = GetComponent<WeatherSystem>();
        inventoryManager = GetComponent<InventoryManager>();
        if (loan != null) loan.storeManager = storeManager;

        if (storeManager == null) Debug.LogError("GameManager: StoreManager component is missing!");
        if (loan == null) Debug.LogError("GameManager: Loan component is missing!");
        if (weatherSystem == null) Debug.LogError("GameManager: WeatherSystem component is missing!");
        if (inventoryManager == null) Debug.LogError("GameManager: InventoryManager is missing!");
    }

    private void Start()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogError("GameManager: DataManager.Instance가 null입니다. 초기화 순서를 확인하세요.");
            return;
        }

        if (inventoryManager != null) inventoryManager.Initialize();
        if (storeManager != null)
            storeManager.Initialize();
        else
            Debug.LogError("GameManager: storeManager가 null — Initialize 불가");
    }

    // TurnManager에서 Result 페이즈 종료 시 호출
    public bool OnTurnEnd(int currentTurn, int maxTurns)
    {
        if (CheckWin()) { OnGameOver?.Invoke(true); return true; }
        if (CheckLose(currentTurn, maxTurns)) { OnGameOver?.Invoke(false); return true; }
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
        // 자본금 고갈 패배
        if (storeManager != null && storeManager.currentMoney <= 0)
        {
            Debug.Log("패배! (자본금 고갈)");
            return true;
        }

        // 턴 소진 패배 (30턴 초과)
        if (currentTurn < maxTurns) return false;
        Debug.Log("패배! (턴 소진)");
        return true;
    }
}
