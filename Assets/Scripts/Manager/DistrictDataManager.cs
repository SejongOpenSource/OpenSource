using UnityEngine;
using System.Collections.Generic;

public class DistrictDataManager : MonoBehaviour
{
    [Header("District SO List")]
    public List<DistrictData> districts = new List<DistrictData>();

    public DistrictData GetDistrict(Commerce zone)
    {
        // Commerce enum과 매칭되는 DistrictData 반환
        return districts.Find(d => d.zone == zone);
    }
}
