using UnityEngine;

public enum TurnPhase { Upgrade, Order, Simulation, Result }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Upgrade;
    public int CurrentTurn { get; private set; } = 1;
    public int MaxTurns { get; private set; } = 30;
    public int TotalSales { get; private set; } = 0;
    public int TargetSales { get; private set; } = 5000000;

    [HideInInspector] public StoreManager storeManager;
    [HideInInspector] public Loan loan;
    [HideInInspector] public WeatherSystem weatherSystem;

    public void AddSales(int amount) => TotalSales += amount;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        storeManager = GetComponent<StoreManager>();
        loan = GetComponent<Loan>();
        weatherSystem = GetComponent<WeatherSystem>();
    }

    public void AdvancePhase()
    {
        switch (CurrentPhase)
        {
            case TurnPhase.Upgrade:
                CurrentPhase = TurnPhase.Order;
                break;
            case TurnPhase.Order:
                CurrentPhase = TurnPhase.Simulation;
                break;
            case TurnPhase.Simulation:
                CurrentPhase = TurnPhase.Result;
                break;
            case TurnPhase.Result:
                if (CheckWin() || CheckLose()) return;
                CurrentTurn++;
                CurrentPhase = TurnPhase.Upgrade;
                break;
        }
    }

    public int CalculateScore(int remainingStockCost, int remainingDebt)
    {
        return (MaxTurns - CurrentTurn) * 10000 - remainingStockCost - remainingDebt;
    }

    private bool CheckWin()
    {
        if (TotalSales < TargetSales) return false;
        Debug.Log("승리!");
        return true;
    }

    private bool CheckLose()
    {
        if (CurrentTurn < MaxTurns) return false;
        Debug.Log("패배!");
        return true;
    }
}
