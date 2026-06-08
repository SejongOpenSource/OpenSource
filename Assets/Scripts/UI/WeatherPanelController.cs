using UnityEngine;
using UnityEngine.UI;

public class WeatherPanelController : MonoBehaviour
{
    // 날씨 정보를 생성하고 보관하는 WeatherSystem
    // Inspector에서 직접 연결하거나 GameManager에서 자동 연결함
    public WeatherSystem weatherSystem;

    // 오전 날씨 아이콘을 표시할 Image UI
    public Image morningWeatherImage;

    // 오후 날씨 아이콘을 표시할 Image UI
    public Image afternoonWeatherImage;

    [Header("날씨별 스프라이트")]
    // 맑음 날씨일 때 사용할 아이콘
    public Sprite sunnySprite;

    // 비 날씨일 때 사용할 아이콘
    public Sprite rainySprite;

    // 폭염 날씨일 때 사용할 아이콘
    public Sprite heatwaveSprite;

    // 흐림 날씨일 때 사용할 아이콘
    public Sprite cloudySprite;

    // 눈 날씨일 때 사용할 아이콘
    public Sprite snowySprite;

    [Header("날씨 아이콘 크기")]
    // 오전/오후 날씨 아이콘을 고정할 크기
    // UI 배치가 흐트러지지 않도록 같은 크기로 맞춤
    public Vector2 weatherIconSize = new Vector2(60f, 60f);

    [Header("테스트 설정")]
    // true면 시작할 때 날씨를 새로 생성함
    // 실제 게임에서는 TurnManager가 날씨를 생성하므로 보통 false로 둠
    public bool generateOnStart = false;

    private void Start()
    {
        // WeatherSystem이 연결되지 않았다면 GameManager에서 자동으로 가져옴
        AutoConnectWeatherSystem();

        // 테스트용 옵션이 켜져 있을 때만 시작 시 날씨 생성
        if (generateOnStart && weatherSystem != null)
        {
            weatherSystem.GenerateWeather();
        }

        // 시작 시 현재 날씨 정보를 UI에 반영
        UpdateWeatherImages();
    }

    private void OnEnable()
    {
        // 패널이 다시 켜질 때 WeatherSystem 연결을 다시 확인
        AutoConnectWeatherSystem();

        // 현재 저장된 오전/오후 날씨를 즉시 UI에 반영
        UpdateWeatherImages();

        // 날씨가 다른 스크립트에서 바뀌었을 때도 UI가 따라오도록 반복 갱신
        // 0.2초마다 UpdateWeatherImages를 호출
        InvokeRepeating(nameof(UpdateWeatherImages), 0.2f, 0.2f);
    }

    private void OnDisable()
    {
        // 패널이 꺼지면 반복 호출 중지
        // 꺼진 상태에서 불필요하게 계속 갱신되는 것을 방지
        CancelInvoke(nameof(UpdateWeatherImages));
    }

    private void OnValidate()
    {
        // Inspector에서 weatherIconSize 값을 수정했을 때
        // 플레이 모드가 아니어도 에디터에서 바로 아이콘 크기를 반영
        ApplyIconSize(morningWeatherImage);
        ApplyIconSize(afternoonWeatherImage);
    }

    private void AutoConnectWeatherSystem()
    {
        // 이미 WeatherSystem이 연결되어 있으면 다시 찾지 않음
        if (weatherSystem != null)
        {
            return;
        }

        // GameManager가 있으면 GameManager에 연결된 WeatherSystem을 가져옴
        if (GameManager.Instance != null)
        {
            weatherSystem = GameManager.Instance.weatherSystem;
        }
    }

    public void UpdateWeatherImages()
    {
        // WeatherSystem이 없으면 현재 날씨 정보를 가져올 수 없음
        if (weatherSystem == null)
        {
            return;
        }

        // 오전 날씨 타입에 맞는 이미지 적용
        ApplyWeatherImage(morningWeatherImage, weatherSystem.morningWeather);

        // 오후 날씨 타입에 맞는 이미지 적용
        ApplyWeatherImage(afternoonWeatherImage, weatherSystem.afternoonWeather);
    }

    private void ApplyWeatherImage(Image weatherImage, WeatherType weather)
    {
        // Image가 연결되지 않았으면 적용할 대상이 없으므로 종료
        if (weatherImage == null)
        {
            return;
        }

        // 현재 날씨 타입에 맞는 스프라이트 가져오기
        Sprite weatherSprite = GetWeatherSprite(weather);

        // 해당 날씨의 스프라이트가 Inspector에 연결되지 않았으면 기존 이미지 유지
        if (weatherSprite == null)
        {
            Debug.LogWarning($"WeatherPanelController: {weather} 스프라이트가 연결되지 않았습니다.");
            return;
        }

        // Image UI에 날씨 스프라이트 적용
        weatherImage.sprite = weatherSprite;

        // 스프라이트 적용 후 아이콘 크기 고정
        ApplyIconSize(weatherImage);
    }

    private void ApplyIconSize(Image weatherImage)
    {
        // Image가 연결되지 않았으면 크기를 조절할 수 없음
        if (weatherImage == null)
        {
            return;
        }

        // RectTransform의 크기를 weatherIconSize 값으로 고정
        weatherImage.rectTransform.sizeDelta = weatherIconSize;

        // 원본 스프라이트 비율 때문에 크기가 다르게 보이지 않도록 비율 고정 해제
        weatherImage.preserveAspect = false;

        // 부모 오브젝트에 Layout Group이 있으면 sizeDelta가 무시될 수 있음
        // 이 경우 LayoutElement의 preferred size도 함께 맞춰줌
        LayoutElement layoutElement = weatherImage.GetComponent<LayoutElement>();

        if (layoutElement != null)
        {
            layoutElement.preferredWidth = weatherIconSize.x;
            layoutElement.preferredHeight = weatherIconSize.y;
        }
    }

    private Sprite GetWeatherSprite(WeatherType weather)
    {
        // WeatherType 값에 따라 사용할 스프라이트를 반환
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