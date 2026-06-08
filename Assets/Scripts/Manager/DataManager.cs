using UnityEngine;

// 게임 내 모든 데이터 조회를 중앙에서 담당하는 싱글톤 매니저.
// ItemDataManager, DistrictDataManager, WeatherDataManager를 하위에 두고
// 각 시스템이 DataManager.Instance 하나만 참조해 데이터를 가져올 수 있도록 한다.
// ItemDataManager, WeatherDataManager는 같은 GameObject의 컴포넌트로,
// DistrictDataManager는 Inspector에서 ScriptableObject로 연결한다.
[RequireComponent(typeof(ItemDataManager))]
[RequireComponent(typeof(WeatherDataManager))]
public class DataManager : MonoBehaviour
{
    // 씬 어디서든 DataManager.Instance로 접근 가능한 싱글톤 인스턴스
    public static DataManager Instance { get; private set; }

    [Header("Sub Data Managers")]
    // 상품(ItemData) ScriptableObject 목록을 관리하는 매니저 (컴포넌트 자동 연결)
    public ItemDataManager itemDataManager;

    // 상권(DistrictData) ScriptableObject를 관리하는 매니저 (Inspector에서 직접 연결)
    public DistrictDataManager districtDataManager;

    // 날씨별 방문객 보정 수치를 관리하는 매니저 (컴포넌트 자동 연결)
    public WeatherDataManager weatherDataManager;

    private void Awake()
    {
        // 씬 중복 로드 시 두 번째 인스턴스를 즉시 파기해 싱글톤 유일성 보장
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        // DontDestroyOnLoad를 적용하려면 루트 오브젝트여야 함
        if (transform.parent != null) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // 같은 GameObject에 붙은 하위 매니저를 코드로 연결
        itemDataManager = GetComponent<ItemDataManager>();
        weatherDataManager = GetComponent<WeatherDataManager>();

        // 필수 컴포넌트·참조 누락 시 즉시 오류 로그 출력
        if (itemDataManager == null)
            Debug.LogError("DataManager: ItemDataManager component is missing!");
        if (districtDataManager == null)
            Debug.LogError("DataManager: DistrictDataManager SO reference is missing! Assign in Inspector.");
        if (weatherDataManager == null)
            Debug.LogError("DataManager: WeatherDataManager component is missing!");

        // DistrictDataManager는 ScriptableObject이므로 명시적으로 초기화 호출
        if (districtDataManager != null) districtDataManager.Initialize();
    }

    // 상권 이름(string)으로 DistrictData를 조회한다.
    // districtDataManager가 없으면 null을 반환한다.
    public DistrictData GetDistrict(string districtName) =>
        districtDataManager != null ? districtDataManager.GetDistrict(districtName) : null;

    // 상권 타입(DistrictType enum)으로 DistrictData를 조회한다.
    // districtDataManager가 없으면 null을 반환한다.
    public DistrictData GetDistrict(DistrictType zone) =>
        districtDataManager != null ? districtDataManager.GetDistrict(zone) : null;

    // 아이템 타입(ItemType enum)으로 ItemData를 조회한다.
    // itemDataManager가 없으면 null을 반환한다.
    public ItemData GetItem(ItemType type) =>
        itemDataManager != null ? itemDataManager.GetItem(type) : null;

    // 날씨 타입(WeatherType enum)에 해당하는 방문객 수 보정 배수를 반환한다.
    // weatherDataManager가 없으면 보정 없음을 의미하는 1.0f를 반환한다.
    public float GetWeatherModifier(WeatherType weather) =>
        weatherDataManager != null ? weatherDataManager.GetModifier(weather) : 1.0f;
}
