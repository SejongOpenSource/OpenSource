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
                break;
            case TurnPhase.Order:
                CurrentPhase = TurnPhase.Simulation;
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
