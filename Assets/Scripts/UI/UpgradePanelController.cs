using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelController : MonoBehaviour
{
    [Header("상권 시스템")]
    // 실제 상권 투자 로직을 처리하는 스크립트
    // Player 오브젝트에 붙어 있는 DistrictSystem을 연결하면 됨
    public DistrictSystem districtSystem;

    [Header("현재 상권 표시")]
    // 현재 적용 중인 상권을 보여주는 텍스트
    // 예: 주거지 | 기본 상권
    public Text currentDistrictText;

    [Header("투자하기 버튼")]
    // 학원가 투자하기 버튼
    public Button academyInvestButton;

    // 대학가 투자하기 버튼
    public Button campusInvestButton;

    // 오피스가 투자하기 버튼
    public Button businessInvestButton;

    // 관광지 투자하기 버튼
    public Button touristInvestButton;

    private void Start()
    {
        // 학원가 버튼 클릭 시 학원가 투자 실행
        if (academyInvestButton != null)
        {
            academyInvestButton.onClick.AddListener(InvestAcademy);
        }

        // 대학가 버튼 클릭 시 대학가 투자 실행
        if (campusInvestButton != null)
        {
            campusInvestButton.onClick.AddListener(InvestCampus);
        }

        // 오피스가 버튼 클릭 시 오피스가 투자 실행
        if (businessInvestButton != null)
        {
            businessInvestButton.onClick.AddListener(InvestBusiness);
        }

        // 관광지 버튼 클릭 시 관광지 투자 실행
        if (touristInvestButton != null)
        {
            touristInvestButton.onClick.AddListener(InvestTourist);
        }

        // 게임 시작 시 현재 상권 텍스트를 한 번 갱신
        UpdateCurrentDistrictText();
    }

    private void InvestAcademy()
    {
        // 학원가 상권으로 투자
        InvestDistrict(DistrictType.Academy);
    }

    private void InvestCampus()
    {
        // 대학가 상권으로 투자
        InvestDistrict(DistrictType.Campus);
    }

    private void InvestBusiness()
    {
        // 오피스가 상권으로 투자
        InvestDistrict(DistrictType.Business);
    }

    private void InvestTourist()
    {
        // 관광지 상권으로 투자
        InvestDistrict(DistrictType.Tourist);
    }

    private void InvestDistrict(DistrictType districtType)
    {
        // DistrictSystem이 연결되지 않았으면 투자 로직 실행 불가
        if (districtSystem == null)
        {
            Debug.LogError("UpgradePanelController: DistrictSystem이 연결되지 않았습니다.");
            return;
        }

        // 실제 상권 투자 로직 실행
        // 내부에서 돈이 충분한지 확인하고, 성공하면 StoreManager의 현재 상권을 바꿈
        districtSystem.UpgradeCommerceZone(districtType);

        // 투자 시도 후 현재 상권 텍스트 갱신
        UpdateCurrentDistrictText();
    }

    private void UpdateCurrentDistrictText()
    {
        // 텍스트가 연결되지 않았으면 표시할 수 없음
        if (currentDistrictText == null)
        {
            return;
        }

        // GameManager 또는 StoreManager가 없으면 기본 문구 표시
        if (GameManager.Instance == null || GameManager.Instance.storeManager == null)
        {
            currentDistrictText.text = "주거지 | 기본 상권";
            return;
        }

        StoreManager storeManager = GameManager.Instance.storeManager;
        DistrictData districtData = storeManager.currentDistrictData;

        // 현재 상권 데이터가 있으면 상권 이름을 표시
        if (districtData != null)
        {
            currentDistrictText.text = $"{districtData.districtName} | 현재 적용 중";
        }
        else
        {
            // 상권 데이터가 없을 때는 enum 값 기준으로 한글 이름 표시
            currentDistrictText.text = $"{GetDistrictName(storeManager.currentZone)} | 기본 상권";
        }
    }

    private string GetDistrictName(DistrictType districtType)
    {
        // DistrictType enum 값을 화면에 보여줄 한글 이름으로 변환
        switch (districtType)
        {
            case DistrictType.Resident:
                return "주거지";

            case DistrictType.Academy:
                return "학원가";

            case DistrictType.Campus:
                return "대학가";

            case DistrictType.Business:
                return "오피스가";

            case DistrictType.Tourist:
                return "관광지";

            default:
                return "알 수 없음";
        }
    }
}