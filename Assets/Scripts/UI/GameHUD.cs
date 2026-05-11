using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("Status Texts")]
    public Text moneyText;
    public Text turnText;
    public Text phaseText;
    public Text weatherText;
    public Text totalSalesText;

    private void Update()
    {
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (GameManager.Instance == null) return;

        // 자산 정보
        if (moneyText != null)
            moneyText.text = $"자산: {GameManager.Instance.storeManager.currentMoney:N0}원";

        if (totalSalesText != null)
            totalSalesText.text = $"누적 매출: {GameManager.Instance.TotalSales:N0}원 / {GameManager.Instance.TargetSales:N0}원";

        // 턴 및 페이즈 정보
        if (TurnManager.Instance != null)
        {
            if (turnText != null)
                turnText.text = $"Turn {TurnManager.Instance.CurrentTurn} / {TurnManager.Instance.MaxTurns}";

            if (phaseText != null)
                phaseText.text = $"단계: {GetPhaseName(TurnManager.Instance.CurrentPhase)}";
        }

        // 날씨 정보
        if (GameManager.Instance.weatherSystem != null && weatherText != null)
        {
            var ws = GameManager.Instance.weatherSystem;
            weatherText.text = $"날씨: {ws.morningWeather} (오전) / {ws.afternoonWeather} (오후)";
        }
    }

    private string GetPhaseName(TurnPhase phase)
    {
        return phase switch
        {
            TurnPhase.Upgrade => "상권 업그레이드",
            TurnPhase.Order => "상품 발주",
            TurnPhase.Simulation => "영업 중",
            TurnPhase.Result => "정산",
            _ => "알 수 없음"
        };
    }
}
