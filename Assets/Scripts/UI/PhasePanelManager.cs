using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhasePanelManager : MonoBehaviour
{
    [Header("페이즈 패널")]
    // 상권 투자 화면
    public GameObject upgradePanel;

    // 발주 화면
    public GameObject orderPanel;

    // 시뮬레이션 진행 화면
    public GameObject simulationPanel;

    // 결과 화면
    public GameObject resultPanel;

    [Header("상단 HUD")]
    // 자산, 부채, 날짜, 날씨가 표시되는 상단 HUD
    // SimulationPanel이 켜져 있을 때는 숨김
    public GameObject topHUD;

    [Header("페이즈 버튼")]
    // Upgrade → Order로 넘어가는 버튼
    public Button upgradeConfirmButton;

    // Result → 다음 날로 넘어가는 버튼
    // ResultView에서 이미 처리하고 있으면 비워둬도 됨
    public Button nextTurnButton;

    [Header("발주 화면 상권 수정")]
    // OrderPanel에 있는 상권 수정하기 버튼
    public Button editDistrictButton;

    [Header("시뮬레이션 설정")]
    // SimulationPanel을 보여주는 시간
    // SimulationPanelController의 duration과 맞추면 자연스러움
    public float simulationWaitTime = 1f;

    private Coroutine simulationCoroutine;

    // OrderPanel에서 상권 수정하기를 눌러 UpgradePanel로 들어온 상태인지 저장
    private bool isEditingDistrictFromOrder = false;

    private void Start()
    {
        // TurnManager가 없으면 페이즈 전환을 처리할 수 없음
        if (TurnManager.Instance == null)
        {
            Debug.LogError("PhasePanelManager: TurnManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // 페이즈 변경 이벤트 구독
        TurnManager.Instance.OnPhaseChanged += OnPhaseChanged;

        // 상권 화면의 발주하기 버튼 연결
        if (upgradeConfirmButton != null)
        {
            upgradeConfirmButton.onClick.AddListener(OnUpgradeConfirmButtonClicked);
        }

        // 다음 날 버튼 연결
        // ResultView에서 이미 nextDayButton을 처리하고 있으면 이 칸은 Inspector에서 비워둬도 됨
        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.AddListener(OnNextTurnButtonClicked);
        }

        // OrderPanel의 상권 수정하기 버튼 연결
        if (editDistrictButton != null)
        {
            editDistrictButton.onClick.AddListener(OpenDistrictEditFromOrder);
        }

        // 시작 페이즈에 맞는 패널 표시
        ShowPanel(TurnManager.Instance.CurrentPhase);
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnPhaseChanged -= OnPhaseChanged;
        }

        // 버튼 이벤트 해제
        if (upgradeConfirmButton != null)
        {
            upgradeConfirmButton.onClick.RemoveListener(OnUpgradeConfirmButtonClicked);
        }

        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.RemoveListener(OnNextTurnButtonClicked);
        }

        if (editDistrictButton != null)
        {
            editDistrictButton.onClick.RemoveListener(OpenDistrictEditFromOrder);
        }
    }

    private void OnUpgradeConfirmButtonClicked()
    {
        // OrderPanel에서 상권 수정하기를 눌러 들어온 상태라면
        // 실제 페이즈를 넘기지 않고 다시 OrderPanel로 돌아감
        if (isEditingDistrictFromOrder)
        {
            ReturnToOrderPanelFromDistrictEdit();
            return;
        }

        // 일반적인 Upgrade → Order 진행
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.AdvancePhase();
        }
    }

    private void OnNextTurnButtonClicked()
    {
        // Result → 다음 턴 Upgrade
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.AdvancePhase();
        }
    }

    public void OpenDistrictEditFromOrder()
    {
        // TurnManager가 없으면 현재 페이즈를 확인할 수 없음
        if (TurnManager.Instance == null)
        {
            Debug.LogError("PhasePanelManager: TurnManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // 발주 화면에서만 상권 수정 화면으로 이동
        if (TurnManager.Instance.CurrentPhase != TurnPhase.Order)
        {
            Debug.LogWarning("PhasePanelManager: Order 페이즈가 아니므로 상권 수정 화면으로 이동하지 않습니다.");
            return;
        }

        // 실제 페이즈는 Order로 유지하고 UI 패널만 UpgradePanel로 전환
        isEditingDistrictFromOrder = true;

        ShowDistrictEditPanel();
    }

    private void ShowDistrictEditPanel()
    {
        // 상권 수정 화면 표시
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }

        // 발주 화면 숨김
        if (orderPanel != null)
        {
            orderPanel.SetActive(false);
        }

        // 다른 패널 숨김
        if (simulationPanel != null)
        {
            simulationPanel.SetActive(false);
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        // 상권 수정 화면에서는 HUD 유지
        if (topHUD != null)
        {
            topHUD.SetActive(true);
        }

        Debug.Log("OrderPanel에서 상권 수정 화면으로 이동했습니다.");
    }

    private void ReturnToOrderPanelFromDistrictEdit()
    {
        // 상권 수정 모드 종료
        isEditingDistrictFromOrder = false;

        // 실제 페이즈는 Order 그대로 유지하고 OrderPanel로 복귀
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        if (orderPanel != null)
        {
            orderPanel.SetActive(true);
        }

        if (simulationPanel != null)
        {
            simulationPanel.SetActive(false);
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        if (topHUD != null)
        {
            topHUD.SetActive(true);
        }

        Debug.Log("상권 수정 화면에서 OrderPanel로 복귀했습니다.");
    }

    private void OnPhaseChanged(TurnPhase phase)
    {
        // 실제 페이즈가 바뀌면 상권 수정 모드는 종료
        isEditingDistrictFromOrder = false;

        // 페이즈에 맞는 패널 표시
        ShowPanel(phase);

        // Simulation 페이즈가 되면 잠시 보여준 뒤 자동으로 Result로 이동
        if (phase == TurnPhase.Simulation)
        {
            StartSimulationAutoAdvance();
        }
    }

    private void ShowPanel(TurnPhase phase)
    {
        // UpgradePanel 표시
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(phase == TurnPhase.Upgrade);
        }

        // OrderPanel 표시
        if (orderPanel != null)
        {
            orderPanel.SetActive(phase == TurnPhase.Order);
        }

        // SimulationPanel 표시
        if (simulationPanel != null)
        {
            simulationPanel.SetActive(phase == TurnPhase.Simulation);
        }

        // ResultPanel 표시
        if (resultPanel != null)
        {
            resultPanel.SetActive(phase == TurnPhase.Result);
        }

        // SimulationPanel에서는 TopHUD 숨김
        // 나머지 화면에서는 TopHUD 표시
        if (topHUD != null)
        {
            topHUD.SetActive(phase != TurnPhase.Simulation);
        }
    }

    private void StartSimulationAutoAdvance()
    {
        // 이미 실행 중인 코루틴이 있으면 중복 실행 방지
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
        }

        simulationCoroutine = StartCoroutine(AutoAdvanceSimulation());
    }

    private IEnumerator AutoAdvanceSimulation()
    {
        // SimulationPanel을 잠깐 보여줌
        yield return new WaitForSeconds(simulationWaitTime);

        // Simulation → Result
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.AdvancePhase();
        }

        simulationCoroutine = null;
    }
}