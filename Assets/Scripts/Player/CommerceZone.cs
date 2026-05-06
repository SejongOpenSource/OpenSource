using UnityEngine;

public enum Commerce { Resident, Academy, Campus, Business, Tourist }

public class CommerceZone : MonoBehaviour
{
    public StoreManager storeManager;
    public void UpgradeCommerceZone(Commerce c)
    {
        switch (c)
        {
            
        }
        storeManager.currentZone = c;
    }
}
