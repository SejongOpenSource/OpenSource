using UnityEngine;

public enum TurnPhase { Upgrade, Order, Simulation, Result }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Upgrade;
    public int CurrentTurn { get; private set; } = 1;
    public int MaxTurns { get; private set; } = 30;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AdvancePhase()
    {
        switch (CurrentPhase)
        {
            case TurnPhase.Upgrade:
                CurrentPhase = TurnPhase.Order;
                GameManager.Instance.weatherSystem.GenerateWeather();
                break;
            case TurnPhase.Order:
                CurrentPhase = TurnPhase.Simulation;
                // TODO: SalesSimulationManager 연결 예정 (별도 이슈)
                break;
            case TurnPhase.Simulation:
                CurrentPhase = TurnPhase.Result;
                break;
            case TurnPhase.Result:
                if (GameManager.Instance.OnTurnEnd(CurrentTurn, MaxTurns)) return;
                CurrentTurn++;
                CurrentPhase = TurnPhase.Upgrade;
                break;
        }
    }
}
