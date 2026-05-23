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

    // HUD 갱신 간격
    // 매 프레임 갱신하지 않고 일정 시간마다 갱신해서 불필요한 호출을 줄임
    private const float HudUpdateInterval = 0.2f;

    private void OnEnable()
    {
        // 오브젝트가 켜졌을 때 HUD를 즉시 한 번 갱신
        UpdateHUD();

        // 이후 일정 간격으로 HUD 갱신
        InvokeRepeating(nameof(UpdateHUD), HudUpdateInterval, HudUpdateInterval);
    }

    private void OnDisable()
    {
        // 오브젝트가 꺼질 때 반복 호출 중지
        CancelInvoke(nameof(UpdateHUD));
    }

    private void UpdateHUD()
    {
        // 자산 / 부채 표시
        UpdateMoneyAndDebtText();

        // 현재 날짜 표시
        UpdateTurnText();

        // 오늘 날씨 표시
        UpdateWeatherText();
    }

    private void UpdateMoneyAndDebtText()
    {
        // GameManager가 없으면 자산과 부채 정보를 가져올 수 없음
        if (GameManager.Instance == null)
        {
            return;
        }

        // StoreManager가 없으면 자산과 부채 정보를 가져올 수 없음
        if (GameManager.Instance.storeManager == null)
        {
            return;
        }

        // 현재 자산 표시
        if (moneyText != null)
        {
            moneyText.text = "자산: "
                + GameManager.Instance.storeManager.currentMoney.ToString("N0")
                + "원";
        }

        // 현재 부채 표시
        if (debtText != null)
        {
            debtText.text = "부채: "
                + GameManager.Instance.storeManager.currentDebt.ToString("N0")
                + "원";
        }
    }

    private void UpdateTurnText()
    {
        // TurnManager가 없으면 날짜 정보를 가져올 수 없음
        if (TurnManager.Instance == null)
        {
            return;
        }

        // 현재 날짜 표시
        if (turnText != null)
        {
            turnText.text =
                TurnManager.Instance.CurrentTurn
                + "일차 / "
                + TurnManager.Instance.MaxTurns
                + "일";
        }
    }

    private void UpdateWeatherText()
    {
        // GameManager가 없으면 날씨 정보를 가져올 수 없음
        if (GameManager.Instance == null)
        {
            return;
        }

        // WeatherSystem이 없으면 날씨 정보를 가져올 수 없음
        if (GameManager.Instance.weatherSystem == null)
        {
            return;
        }

        // WeatherText가 연결되지 않았으면 표시할 수 없음
        if (weatherText == null)
        {
            return;
        }

        WeatherSystem weatherSystem = GameManager.Instance.weatherSystem;

        // 오늘 오전 / 오후 날씨 표시
        weatherText.text =
            "날씨: "
            + GetWeatherName(weatherSystem.morningWeather)
            + "(오전) / "
            + GetWeatherName(weatherSystem.afternoonWeather)
            + "(오후)";
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