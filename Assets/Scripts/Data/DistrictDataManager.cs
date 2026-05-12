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
        foreach (var data in districts)
        {
            if (data != null) districtDict[data.districtName] = data;
            else Debug.LogError("District data doesn't exist");
        }
    }

    public DistrictData GetDistrict(string name)
    {
        districtDict.TryGetValue(name, out DistrictData result);
        return result;
    }
}
