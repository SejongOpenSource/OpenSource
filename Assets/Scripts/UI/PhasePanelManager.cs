using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhasePanelManager : MonoBehaviour
{
    [Header("페이즈 패널")]
    [SerializeField] private GameObject _upgradePanel;
    [SerializeField] private GameObject _orderPanel;
    [SerializeField] private GameObject _simulationPanel;
    [SerializeField] private GameObject _resultPanel;

    [Header("페이즈 버튼")]
    [SerializeField] private Button _upgradeConfirmButton;  // Upgrade → Order
    [SerializeField] private Button _nextTurnButton;        // Result → Upgrade (다음 턴)

    private void Start()
    {
        TurnManager.Instance.OnPhaseChanged += OnPhaseChanged;

        _upgradeConfirmButton?.onClick.AddListener(() => TurnManager.Instance.AdvancePhase());
        _nextTurnButton?.onClick.AddListener(() => TurnManager.Instance.AdvancePhase());

        ShowPanel(TurnManager.Instance.CurrentPhase);
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(TurnPhase phase)
    {
        ShowPanel(phase);

        if (phase == TurnPhase.Simulation)
            StartCoroutine(AutoAdvanceSimulation());
    }

    private void ShowPanel(TurnPhase phase)
    {
        _upgradePanel?.SetActive(phase == TurnPhase.Upgrade);
        _orderPanel?.SetActive(phase == TurnPhase.Order);
        _simulationPanel?.SetActive(phase == TurnPhase.Simulation);
        _resultPanel?.SetActive(phase == TurnPhase.Result);
    }

    // Simulation 패널 1초 노출 후 자동 전환 (날씨 생성 + 판매 시뮬 실행)
    private IEnumerator AutoAdvanceSimulation()
    {
        yield return new WaitForSeconds(1f);
        TurnManager.Instance?.AdvancePhase();
    }
}
