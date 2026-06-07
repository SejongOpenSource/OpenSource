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
    [HideInInspector] public CustomerManager customerManager;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        if (transform.parent != null) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        storeManager = GetComponent<StoreManager>();
        loan = GetComponent<Loan>();
        weatherSystem = GetComponent<WeatherSystem>();
        inventoryManager = GetComponent<InventoryManager>();
        customerManager = GetComponent<CustomerManager>();
        if (loan != null) loan.storeManager = storeManager;

        if (storeManager == null) Debug.LogError("GameManager: StoreManager component is missing!");
        if (loan == null) Debug.LogError("GameManager: Loan component is missing!");
        if (weatherSystem == null) Debug.LogError("GameManager: WeatherSystem component is missing!");
        if (inventoryManager == null) Debug.LogError("GameManager: InventoryManager is missing!");
        if (customerManager == null) Debug.LogError("GameManager: CustomerManager component is missing!");
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
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(BGMType.Main, fade: true, fadeDuration: 1.2f);
        }
    }
    
    private void Update()
    {
        // ===================================================================
        // 🔥 [디버그용 치트키] 팀 프로젝트 테스트 후 최종 빌드 시 주석 처리하거나 삭제하세요.
        // ===================================================================

        // 1번 키: 즉시 승리 세팅 (매출을 승리 목표 금액으로 채우고 페이즈 강제 진행)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (SalesAlgorithm.Instance != null)
            {
                // 부족한 매출만큼 딱 채워서 TargetSales(5000000원)로 만듭니다.
                int lackSales = TargetSales - SalesAlgorithm.Instance.TotalSales;
                if (lackSales > 0)
                {
                    SalesAlgorithm.Instance.AddSales(lackSales);
                }
            
                Debug.Log($"[CHEAT] 승리 조건 충족! 현재 누적 매출: {SalesAlgorithm.Instance.TotalSales}원");
                Debug.Log("[CHEAT] 다음 페이즈(Result 턴 엔드) 진입 시 '승리' 화면이 뜹니다.");

                // 현재 씬 구조상 버튼을 안 누르고 즉시 턴을 끝내 승리 화면을 보고 싶다면 아래 주석을 해제하세요.
                // TurnManager.Instance?.AdvancePhase();
            }
        }

        // 2번 키: 즉시 패배 세팅 (자본금을 마이너스로 조작)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (storeManager != null)
            {
                // 자본금을 -1원으로 만들어 CheckLose의 파산 조건에 걸리게 합니다.
                // 만약 storeManager에 돈을 깎는 기능(SpendMoney 등)이 있다면 그걸 쓰셔도 됩니다.
                // 여기서는 안전하게 현재 돈만큼 빼고 추가로 1원을 더 빼서 마이너스로 만듭니다.
                int current = storeManager.currentMoney;
                storeManager.SpendMoney(current + 1); // 0원 미만(음수)으로 강제 변환
            
                Debug.Log($"[CHEAT] 패배 조건(파산) 충족! 현재 자본금: {storeManager.currentMoney}원");
                Debug.Log("[CHEAT] 다음 페이즈(Result 턴 엔드) 진입 시 '패배' 화면이 뜹니다.");

                // 현재 씬 구조상 버튼을 안 누르고 즉시 턴을 끝내 패배 화면을 보고 싶다면 아래 주석을 해제하세요.
                // TurnManager.Instance?.AdvancePhase();
            }
        }
        // ===================================================================
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