using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DistrictDataManager", menuName = "GameData/DistrictDataManager")]
public class DistrictDataManager : ScriptableObject
{
    // 각 지역(District)의 데이터를 관리하는 리스트입니다.
    [SerializeField] private List<DistrictData> districts;

    // 지역 이름으로 조회를 빠르게 하기 위한 딕셔너리입니다.
    private Dictionary<string, DistrictData> districtDict = new Dictionary<string, DistrictData>();

    // 데이터를 초기화하고 딕셔너리를 구축합니다.
    public void Initialize()
    {
        districtDict.Clear();
        if (districts == null) return;
        foreach (var data in districts)
        {
            if (data != null && !string.IsNullOrEmpty(data.districtName)) 
                districtDict[data.districtName] = data;
            else 
                Debug.LogError("District data doesn't exist or has no name");
        }
    }

    // 이름을 기반으로 특정 지역 데이터를 가져옵니다.
    public DistrictData GetDistrict(string districtName)
    {
        if (string.IsNullOrEmpty(districtName))
        {
            Debug.LogError("District name is null or empty");
            return null;
        }
        districtDict.TryGetValue(districtName, out DistrictData result);
        return result;
    }

    // DistrictType을 기반으로 특정 지역 데이터를 가져옵니다.
    public DistrictData GetDistrict(DistrictType zone)
    {
        return districts.Find(d => d != null && d.zone == zone);
    }
}
