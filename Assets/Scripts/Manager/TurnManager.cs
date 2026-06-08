using UnityEngine;

// 턴 흐름과 페이즈 전환을 관리하는 싱글톤 매니저.
// 한 턴은 Upgrade → Order → Simulation → Result 순서로 진행된다.
// 페이즈가 바뀔 때마다 OnPhaseChanged 이벤트를 발생시켜 UI 등이 반응할 수 있도록 한다.
public enum TurnPhase { Upgrade, Order, Simulation, Result }

public class TurnManager : MonoBehaviour
{
    // 씬 어디서든 TurnManager.Instance로 접근 가능한 싱글톤 인스턴스
    public static TurnManager Instance { get; private set; }

    // 현재 진행 중인 페이즈 (읽기 전용, 외부에서 직접 변경 불가)
    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Upgrade;

    // 현재 턴 번호 (1부터 시작)
    public int CurrentTurn { get; private set; } = 1;

    // 게임 최대 턴 수 (30턴 초과 시 패배)
    public int MaxTurns { get; private set; } = 30;

    // 페이즈가 변경될 때 발생하는 이벤트.
    // PhasePanelManager 등 UI가 구독해 화면 전환에 활용한다.
    public event System.Action<TurnPhase> OnPhaseChanged;

    private void Awake()
    {
        // 씬 중복 로드 시 두 번째 인스턴스를 즉시 파기해 싱글톤 유일성 보장
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // DontDestroyOnLoad를 적용하려면 루트 오브젝트여야 함
        if (transform.parent != null) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    // 현재 페이즈를 다음 단계로 전환한다.
    // 각 페이즈 전환 시 필요한 로직(날씨 생성, 판매 시뮬레이션, 대출 이자)을 함께 처리한다.
    // Result → Upgrade 전환 시 GameManager.OnTurnEnd를 호출해 승패를 확인한다.
    public void AdvancePhase()
    {
        switch (CurrentPhase)
        {
            // Upgrade(상권 선택) 완료 → Order(발주) 진입
            case TurnPhase.Upgrade:
                CurrentPhase = TurnPhase.Order;
                break;

            // Order(발주) 완료 → Simulation(영업) 진입
            case TurnPhase.Order:
                CurrentPhase = TurnPhase.Simulation;
                break;

            // Simulation(영업) 완료 → Result(결산) 진입
            // 이 시점에 날씨 생성, 판매 시뮬레이션, 대출 이자를 순서대로 처리한다.
            case TurnPhase.Simulation:
                // 다음 날씨 생성 (오전·오후 날씨 결정)
                GameManager.Instance?.weatherSystem?.GenerateWeather();

                // 방문객 수 계산 및 상품별 판매 처리
                SalesAlgorithm.Instance?.RunSimulation();

                // Result 페이즈 진입 전에 대출 이자 적용
                ApplyLoanInterest();

                // 결과 화면으로 이동
                CurrentPhase = TurnPhase.Result;
                break;

            // Result(결산) 완료 → 다음 턴 Upgrade 진입
            // 승패 조건 확인 후 게임이 계속되면 턴 카운트를 증가시킨다.
            case TurnPhase.Result:
                if (GameManager.Instance == null)
                {
                    Debug.LogError("TurnManager: GameManager.Instance가 null입니다. TurnPhase.Result를 완료할 수 없습니다.");
                    return;
                }

                // 승패 조건 충족 시 OnTurnEnd가 true를 반환하고 게임 종료 처리
                if (GameManager.Instance.OnTurnEnd(CurrentTurn, MaxTurns))
                {
                    return;
                }

                // 게임이 계속되면 다음 턴으로 진행
                CurrentTurn++;
                CurrentPhase = TurnPhase.Upgrade;
                break;
        }

        // 페이즈 전환 완료 후 구독자(UI 등)에게 변경된 페이즈를 알림
        OnPhaseChanged?.Invoke(CurrentPhase);
    }

    // Simulation 페이즈 종료 시 대출 이자를 자본금에서 차감한다.
    // 부채가 0원이면 이자를 적용하지 않는다.
    private void ApplyLoanInterest()
    {
        var gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("TurnManager: GameManager.Instance가 null이라 대출 이자를 적용할 수 없습니다.");
            return;
        }

        if (gameManager.loan == null)
        {
            Debug.LogError("TurnManager: Loan이 연결되지 않아 대출 이자를 적용할 수 없습니다.");
            return;
        }

        if (gameManager.storeManager == null)
        {
            Debug.LogError("TurnManager: StoreManager가 연결되지 않아 대출 이자를 적용할 수 없습니다.");
            return;
        }

        // 부채가 있을 때만 이자 적용 (불필요한 연산 방지)
        if (gameManager.storeManager.currentDebt > 0)
        {
            gameManager.loan.AddInterest();
        }
    }
}
