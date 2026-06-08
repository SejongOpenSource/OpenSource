using UnityEngine;

// 게임 전체를 총괄하는 최상위 매니저.
// StoreManager, Loan, WeatherSystem, InventoryManager, CustomerManager를
// 한 GameObject에 묶어 싱글톤으로 관리한다.
// 승패 판정과 점수 계산을 담당하며, 게임 종료 시 OnGameOver 이벤트를 발생시킨다.
[RequireComponent(typeof(StoreManager))]
[RequireComponent(typeof(Loan))]
[RequireComponent(typeof(WeatherSystem))]
[RequireComponent(typeof(InventoryManager))]
public class GameManager : MonoBehaviour
{
    // 씬 어디서든 GameManager.Instance로 접근 가능한 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    // 승리에 필요한 누적 매출 목표액 (기본값: 500만원)
    public int TargetSales { get; private set; } = 5000000;

    // 게임 종료 시 발생하는 이벤트. true = 승리, false = 패배.
    // UI(결과 화면 등)에서 구독해 화면 전환 처리를 담당한다.
    public event System.Action<bool> OnGameOver;

    // 하위 매니저 참조 — Awake()에서 GetComponent로 자동 연결되므로 Inspector 노출 불필요
    [HideInInspector] public StoreManager storeManager;
    [HideInInspector] public Loan loan;
    [HideInInspector] public WeatherSystem weatherSystem;
    [HideInInspector] public InventoryManager inventoryManager;
    [HideInInspector] public CustomerManager customerManager;

    private void Awake()
    {
        // 씬 중복 로드 시 두 번째 인스턴스를 즉시 파기해 싱글톤 유일성 보장
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        // DontDestroyOnLoad를 적용하려면 루트 오브젝트여야 함
        if (transform.parent != null) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // 같은 GameObject에 붙은 하위 컴포넌트를 코드로 연결 (Inspector 의존 제거)
        storeManager = GetComponent<StoreManager>();
        loan = GetComponent<Loan>();
        weatherSystem = GetComponent<WeatherSystem>();
        inventoryManager = GetComponent<InventoryManager>();
        customerManager = GetComponent<CustomerManager>();

        // Loan이 StoreManager를 통해 자본금을 차감할 수 있도록 참조 주입
        if (loan != null) loan.storeManager = storeManager;

        // 필수 컴포넌트 누락 시 즉시 오류 로그 출력
        if (storeManager == null) Debug.LogError("GameManager: StoreManager component is missing!");
        if (loan == null) Debug.LogError("GameManager: Loan component is missing!");
        if (weatherSystem == null) Debug.LogError("GameManager: WeatherSystem component is missing!");
        if (inventoryManager == null) Debug.LogError("GameManager: InventoryManager is missing!");
        if (customerManager == null) Debug.LogError("GameManager: CustomerManager component is missing!");
    }

    private void Start()
    {
        // DataManager는 별도 GameObject에 존재하므로 Awake 이후 Start에서 의존성 확인
        if (DataManager.Instance == null)
        {
            Debug.LogError("GameManager: DataManager.Instance가 null입니다. 초기화 순서를 확인하세요.");
            return;
        }

        // InventoryManager 초기화 — DataManager가 준비된 후 아이템 데이터 로드
        if (inventoryManager != null) inventoryManager.Initialize();

        // StoreManager 초기화 — 상권 데이터 로드 및 초기 자본금 설정
        if (storeManager != null)
            storeManager.Initialize();
        else
            Debug.LogError("GameManager: storeManager가 null — Initialize 불가");

        // 게임 시작 시 메인 BGM 재생 (페이드인 1.2초)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(BGMType.Main, fade: true, fadeDuration: 1.2f);
        }
    }

    // TurnManager가 Result 페이즈를 마칠 때 호출한다.
    // 승패 조건을 순서대로 확인하고, 게임이 끝나야 하면 true를 반환한다.
    // true를 반환하면 TurnManager는 다음 턴으로 진행하지 않는다.
    public bool OnTurnEnd(int currentTurn, int maxTurns)
    {
        if (CheckWin()) { OnGameOver?.Invoke(true); return true; }
        if (CheckLose(currentTurn, maxTurns)) { OnGameOver?.Invoke(false); return true; }
        return false;
    }

    // 최종 점수를 계산한다.
    // 공식: (남은 턴 수 × 10,000) - 잔여 재고 원가 - 잔여 대출 잔액
    // 빨리 클리어할수록, 재고·부채가 적을수록 높은 점수를 얻는다.
    public int CalculateScore(int remainingStockCost, int remainingDebt)
    {
        return (TurnManager.Instance.MaxTurns - TurnManager.Instance.CurrentTurn) * 10000
               - remainingStockCost - remainingDebt;
    }

    // 누적 매출이 목표액(500만원) 이상이면 승리
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

    // 자본금 고갈(0원 미만) 또는 턴 소진(maxTurns 도달) 시 패배
    private bool CheckLose(int currentTurn, int maxTurns)
    {
        // 자본금 고갈 패배: 0원은 생존, 음수부터 패배
        if (storeManager != null && storeManager.currentMoney < 0)
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
