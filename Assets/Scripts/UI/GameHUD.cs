using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("Status Texts")]
    // 현재 보유 자산 표시 텍스트
    public Text moneyText;

    // 현재 부채 표시 텍스트
    public Text debtText;

    // 현재 턴 표시 텍스트
    public Text turnText;

    // 오늘 날씨 표시 텍스트
    public Text weatherText;

    // totalSalesText는 sales 이슈 완료 후 재연결 예정

    private void Start()
    {
        // 화면이 처음 켜졌을 때 HUD 내용을 한 번 갱신
        UpdateHUD();
    }

    private void Update()
    {
        // 자산, 부채, 턴, 날씨 정보가 바뀔 수 있으므로 매 프레임 갱신
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        // GameManager가 없으면 자산, 부채, 날씨 정보를 가져올 수 없음
        if (GameManager.Instance == null)
        {
            return;
        }

        // 현재 자산 표시
        if (moneyText != null)
        {
            moneyText.text = "자산: " + GameManager.Instance.storeManager.currentMoney.ToString("N0") + "원";
        }

        // 현재 부채 표시
        if (debtText != null)
        {
            debtText.text = "부채: " + GameManager.Instance.storeManager.currentDebt.ToString("N0") + "원";
        }

        // 현재 턴 표시
        if (TurnManager.Instance != null && turnText != null)
        {
            turnText.text =
                TurnManager.Instance.CurrentTurn
                + "일차 / "
                + TurnManager.Instance.MaxTurns
                + "일";
        }

        // 오늘 오전 / 오후 날씨 표시
        if (GameManager.Instance.weatherSystem != null && weatherText != null)
        {
            WeatherSystem weatherSystem = GameManager.Instance.weatherSystem;

            weatherText.text =
                "날씨: "
                + GetWeatherName(weatherSystem.morningWeather)
                + "(오전) / "
                + GetWeatherName(weatherSystem.afternoonWeather)
                + "(오후)";
        }
    }

    private string GetWeatherName(WeatherType weather)
    {
        // WeatherType enum 값을 화면에 보여줄 한글 이름으로 변환
        switch (weather)
        {
            case WeatherType.Sunny:
                return "맑음";

            case WeatherType.Rainy:
                return "비";

            case WeatherType.Heatwave:
                return "폭염";

            case WeatherType.Cloudy:
                return "흐림";

            case WeatherType.Snowy:
                return "눈";

            default:
                return "알 수 없음";
        }
    }
}