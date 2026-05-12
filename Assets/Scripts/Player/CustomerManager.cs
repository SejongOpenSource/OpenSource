using System;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public StoreManager storeManager;
    private int baseVisittor = 20;

    private void Start()
    {
        storeManager = GetComponent<StoreManager>();
    }

    public int CalculateDailyVisitors()
    {
        switch (storeManager.currentZone)
        {
            case Commerce.Academy:
                return baseVisittor + baseVisitor * 0.25;
            case Commerce.Campus:
                return baseVisittor + baseVisitor * 0.5; 
            case Commerce.Business:
                return baseVisittor + baseVisitor * 0.75;
            case Commerce.Tourist:
                return baseVisittor + baseVisitor;
            default:
                return baseVisitor;
        }
    }
}
