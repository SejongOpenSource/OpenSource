using UnityEngine;

public enum TurnPhase { Upgrade, Order, Simulation, Result }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Upgrade;
    public int CurrentTurn { get; private set; } = 1;
    public int MaxTurns { get; private set; } = 30;

    // Phase 변경 시 발생하는 이벤트 (UI 등에서 구독 가능)
    public event System.Action<TurnPhase> OnPhaseChanged;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // 현재 페이즈를 다시 통지한다. 구독자(UI 패널 컨트롤러 등)가 구독 직후 호출하면
    // 시작 시점의 현재 페이즈에 맞춰 초기 패널 상태를 동기화할 수 있다.
    public void NotifyCurrentPhase()
    {
        OnPhaseChanged?.Invoke(CurrentPhase);
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
                GameManager.Instance.weatherSystem?.GenerateWeather();
                SalesAlgorithm.Instance?.RunSimulation();
                CurrentPhase = TurnPhase.Result;
                break;
            case TurnPhase.Result:
                if (GameManager.Instance.OnTurnEnd(CurrentTurn, MaxTurns)) return;
                CurrentTurn++;
                CurrentPhase = TurnPhase.Upgrade;
                break;
        }

        OnPhaseChanged?.Invoke(CurrentPhase);
    }
}
