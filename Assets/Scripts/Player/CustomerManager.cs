using System;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public StoreManager storeManager;
    private int baseVisitor = 20;

    private void Start()
    {
        storeManager = GetComponent<StoreManager>();
    }

    public int CalculateDailyVisitors()
    {
        switch (storeManager.currentZone)
        {
            // GameData 업데이트 시 CSV GameData 내부 
            case Commerce.Academy:
                return baseVisitor + (int)(baseVisitor * 0.25);
            case Commerce.Campus:
                return baseVisitor + (int)(baseVisitor * 0.5); 
            case Commerce.Business:
                return baseVisitor + (int)(baseVisitor * 0.75);
            case Commerce.Tourist:
                return baseVisitor + baseVisitor;
            default:
                return baseVisitor;
        }
    }

    public int CalculateWeatherVisitor()
    {
        return baseVisitor;
    }
}
