using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DistrictDataManager", menuName = "GameData/DistrictDataManager")]
public class DistrictDataManager : ScriptableObject
{

    public List<DistrictData> districts;
    private Dictionary<string, DistrictData> districtDict = new Dictionary<string, DistrictData>();

    public void Initialize()
    {
        districtDict.Clear();
        if (districts == null) return;
        foreach (var data in districts)
        {
            if (data != null && !string.IsNullOrEmpty(data.districtName)) districtDict[data.districtName] = data;
            else Debug.LogError("District data doesn't exist");
        }
    }

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
}
