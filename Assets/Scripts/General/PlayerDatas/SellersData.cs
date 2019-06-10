using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SellersData
{
    [SerializeField] string allSellersDatasSerialized;
    AllSellerData allSellerData;

    public void SetSellersData(List<ShopWithShopParameters> shopParams)
    {
        allSellerData = new AllSellerData(shopParams);
        allSellersDatasSerialized = JsonUtility.ToJson(allSellerData);
    }

    public void SetSellersData(ShopWithShopParameters shopParam)
    {
        bool foundElement = false;
        foreach(SingleSellerData singleData in allSellerData.GetAllSellersDatas)
        {
            if(shopParam.GetShopName == singleData.GetShopName)
            {
                singleData.ModifyValue(shopParam.GetSoldItems);
                foundElement = true;
                break;
            }
        }

        if (!foundElement)
        {
            allSellerData.AddSellerData(shopParam);
        }

        allSellersDatasSerialized = JsonUtility.ToJson(allSellerData);
    }

    public List<SingleSellerData> GetAllSellersDatas
    {
        get
        {
            allSellerData = JsonUtility.FromJson<AllSellerData>(allSellersDatasSerialized);
            if (allSellerData != null)
                return allSellerData.GetAllSellersDatas;
            else
                return new List<SingleSellerData>();
        }
    }

    public void DebugData()
    {
        Debug.Log(allSellersDatasSerialized);
    }
}

[System.Serializable]
public class AllSellerData
{
    [SerializeField] List<SingleSellerData> allSellersDatas;
    public AllSellerData(List<ShopWithShopParameters> shops)
    {
        allSellersDatas = new List<SingleSellerData>();
        foreach (ShopWithShopParameters shop in shops)
        {
            SingleSellerData newSingleSellerData = new SingleSellerData(shop.GetShopName, shop.GetSoldItems);
            allSellersDatas.Add(newSingleSellerData);
        }
    }

    public void AddSellerData(ShopWithShopParameters shop)
    {
        if(allSellersDatas == null)
            allSellersDatas = new List<SingleSellerData>();

        SingleSellerData newSingleSellerData = new SingleSellerData(shop.GetShopName, shop.GetSoldItems);
        allSellersDatas.Add(newSingleSellerData);
    }

    public List<SingleSellerData> GetAllSellersDatas { get { return allSellersDatas; } }
}

[System.Serializable]
public class SingleSellerData
{
    [SerializeField] string shopName;
    public string GetShopName { get { return shopName; } }
    [SerializeField] List<int> shopSoldItems;

    public SingleSellerData(string name, List<ShipEquipment> soldItems)
    {
        shopName = name;
        shopSoldItems = new List<int>();

        if (AllGameEquipmentsManager.manager != null)
            shopSoldItems = AllGameEquipmentsManager.manager.EquipmentsToIndexes(soldItems);
    }

    public void ModifyValue(List<ShipEquipment> soldItems)
    {
        shopSoldItems = new List<int>();

        if (AllGameEquipmentsManager.manager != null)
            shopSoldItems = AllGameEquipmentsManager.manager.EquipmentsToIndexes(soldItems);
    }

    public List<ShipEquipment> GetAllSoldEquipments
    {
        get
        {
            if (AllGameEquipmentsManager.manager != null)
                return AllGameEquipmentsManager.manager.IndexesToEquipments(shopSoldItems);
            else
                return new List<ShipEquipment>();
        }
    }
}
