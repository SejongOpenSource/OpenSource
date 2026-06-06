using UnityEngine;

public enum TurnPhase { Upgrade, Order, Simulation, Result }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Upgrade;
    public int CurrentTurn { get; private set; } = 1;
    public int MaxTurns { get; private set; } = 30;

    // Phase 변경 시 발생하는 이벤트
    // UI 등에서 구독 가능
    public event System.Action<TurnPhase> OnPhaseChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

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
                // 다음 날씨 생성
                GameManager.Instance?.weatherSystem?.GenerateWeather();

                // 영업 시뮬레이션 실행
                SalesAlgorithm.Instance?.RunSimulation();

                // Result 페이즈 진입 전에 대출 이자 적용
                ApplyLoanInterest();

                // 결과 화면으로 이동
                CurrentPhase = TurnPhase.Result;
                break;

            case TurnPhase.Result:
                if (GameManager.Instance == null)
                {
                    Debug.LogError("TurnManager: GameManager.Instance가 null입니다. TurnPhase.Result를 완료할 수 없습니다.");
                    return;
                }

                if (GameManager.Instance.OnTurnEnd(CurrentTurn, MaxTurns))
                {
                    return;
                }

                CurrentTurn++;
                CurrentPhase = TurnPhase.Upgrade;
                break;
        }

        OnPhaseChanged?.Invoke(CurrentPhase);
    }

    private void ApplyLoanInterest()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("TurnManager: GameManager.Instance가 null이라 대출 이자를 적용할 수 없습니다.");
            return;
        }

        if (GameManager.Instance.loan == null)
        {
            Debug.LogError("TurnManager: Loan이 연결되지 않아 대출 이자를 적용할 수 없습니다.");
            return;
        }

        if (GameManager.Instance.storeManager == null)
        {
            Debug.LogError("TurnManager: StoreManager가 연결되지 않아 대출 이자를 적용할 수 없습니다.");
            return;
        }

        // 부채가 있을 때만 이자 적용
        if (GameManager.Instance.storeManager.currentDebt > 0)
        {
            GameManager.Instance.loan.AddInterest();
        }
    }
}