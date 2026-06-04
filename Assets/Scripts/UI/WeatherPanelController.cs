using UnityEngine;
using UnityEngine.UI;

public class WeatherPanelController : MonoBehaviour
{
    // 날씨를 생성하고 저장하는 WeatherSystem
    public WeatherSystem weatherSystem;

    // 오전 날씨 이미지가 표시될 UI Image
    public Image morningWeatherImage;

    // 오후 날씨 이미지가 표시될 UI Image
    public Image afternoonWeatherImage;

    [Header("날씨별 스프라이트")]
    // 맑음 스프라이트
    public Sprite sunnySprite;

    // 비 스프라이트
    public Sprite rainySprite;

    // 폭염 스프라이트
    public Sprite heatwaveSprite;

    // 흐림 스프라이트
    public Sprite cloudySprite;

    // 눈 스프라이트
    public Sprite snowySprite;

    [Header("날씨 아이콘 크기")]
    // 오전/오후 날씨 아이콘을 이 크기로 고정
    public Vector2 weatherIconSize = new Vector2(60f, 60f);

    [Header("테스트 설정")]
    // 시작할 때 날씨를 새로 생성할지 여부
    // 실제 게임 흐름에서는 TurnManager가 날씨를 생성하므로 보통 OFF
    public bool generateOnStart = false;

    private void Start()
    {
        // WeatherSystem이 비어 있으면 GameManager에서 자동으로 가져옴
        AutoConnectWeatherSystem();

        // 테스트용으로만 시작 시 날씨 생성
        if (generateOnStart && weatherSystem != null)
        {
            weatherSystem.GenerateWeather();
        }

        // 시작 시 날씨 이미지 갱신
        UpdateWeatherImages();
    }

    private void OnEnable()
    {
        // 패널이 켜질 때 최신 날씨와 크기 반영
        AutoConnectWeatherSystem();
        UpdateWeatherImages();

        // 날씨가 바뀌어도 이미지가 따라오도록 반복 갱신
        InvokeRepeating(nameof(UpdateWeatherImages), 0.2f, 0.2f);
    }

    private void OnDisable()
    {
        // 패널이 꺼질 때 반복 갱신 중지
        CancelInvoke(nameof(UpdateWeatherImages));
    }

    private void OnValidate()
    {
        // Inspector에서 Weather Icon Size 값을 바꿀 때
        // 에디터 상태에서도 바로 크기가 반영되도록 함
        ApplyIconSize(morningWeatherImage);
        ApplyIconSize(afternoonWeatherImage);
    }

    private void AutoConnectWeatherSystem()
    {
        // 이미 연결되어 있으면 다시 찾지 않음
        if (weatherSystem != null)
        {
            return;
        }

        // GameManager에 연결된 WeatherSystem을 자동으로 가져옴
        if (GameManager.Instance != null)
        {
            weatherSystem = GameManager.Instance.weatherSystem;
        }
    }

    public void UpdateWeatherImages()
    {
        // WeatherSystem이 없으면 변경할 날씨 데이터가 없음
        if (weatherSystem == null)
        {
            return;
        }

        // 오전 날씨 이미지 변경
        ApplyWeatherImage(morningWeatherImage, weatherSystem.morningWeather);

        // 오후 날씨 이미지 변경
        ApplyWeatherImage(afternoonWeatherImage, weatherSystem.afternoonWeather);
    }

    private void ApplyWeatherImage(Image weatherImage, WeatherType weather)
    {
        // Image가 연결되지 않았으면 실행하지 않음
        if (weatherImage == null)
        {
            return;
        }

        // 날씨에 맞는 스프라이트 가져오기
        Sprite weatherSprite = GetWeatherSprite(weather);

        // 스프라이트가 연결되지 않았으면 기존 이미지 유지
        if (weatherSprite == null)
        {
            Debug.LogWarning($"WeatherPanelController: {weather} 스프라이트가 연결되지 않았습니다.");
            return;
        }

        // 날씨 스프라이트 적용
        weatherImage.sprite = weatherSprite;

        // 아이콘 크기 고정
        ApplyIconSize(weatherImage);
    }

    private void ApplyIconSize(Image weatherImage)
    {
        // Image가 연결되지 않았으면 실행하지 않음
        if (weatherImage == null)
        {
            return;
        }

        // RectTransform 크기 고정
        weatherImage.rectTransform.sizeDelta = weatherIconSize;

        // 스프라이트 비율 때문에 크기가 달라 보이지 않도록 비율 고정 해제
        weatherImage.preserveAspect = false;

        // Layout Group이 있는 부모 밑에 있을 경우 sizeDelta가 무시될 수 있음
        // LayoutElement가 있으면 preferred size도 같이 맞춰줌
        LayoutElement layoutElement = weatherImage.GetComponent<LayoutElement>();

        if (layoutElement != null)
        {
            layoutElement.preferredWidth = weatherIconSize.x;
            layoutElement.preferredHeight = weatherIconSize.y;
        }
    }

    private Sprite GetWeatherSprite(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Sunny:
                return sunnySprite;

            case WeatherType.Rainy:
                return rainySprite;

            case WeatherType.Heatwave:
                return heatwaveSprite;

            case WeatherType.Cloudy:
                return cloudySprite;

            case WeatherType.Snowy:
                return snowySprite;

            default:
                return null;
        }
    }
}