using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SellerManager
{
    [SerializeField] List<ShopWithShopParameters> allShopsWithParameters;

    MapDocksInterface mapDocksInterface;
    public void SetUp(MapDocksInterface docksInterface)
    {
        mapDocksInterface = docksInterface;

        foreach (ShopWithShopParameters shopWithParams in allShopsWithParameters)
            shopWithParams.SetUpDockSpot();

        LoadAllShopsDatas();
    }

    SellerShopParameters currentSellerShopParameters;

    public List<ShipEquipment> GetBuyableEquipments
    {
        get
        {
            return currentSellerShopParameters.GetBuyableEquipments;
        }
    }

    public List<ShipEquipment> GetSoldEquipments
    {
        get
        {
            return currentSellerShopParameters.GetSoldEquipments;
        }
    }

    public void OpenSellerInventory(SellerShopParameters shopParameters)
    {
        currentSellerShopParameters = shopParameters;
    }

    public void RemoveEquipmentFromSellerShop(ShipEquipment shipEquipment)
    {
        currentSellerShopParameters.RemoveEquipment(shipEquipment);
    }

    public void AddSoldEquipmentToSellerShop(ShipEquipment shipEquipment)
    {
        currentSellerShopParameters.AddSoldEquipment(shipEquipment);
        if (currentSellerShopParameters.GetSoldEquipments.Count > mapDocksInterface.GetSoldEquipmentsCapacity)
            currentSellerShopParameters.RemoveOldestSoldEquipment();

        foreach(ShopWithShopParameters parameters in allShopsWithParameters)
        {
            if(parameters.GetShopParameters == currentSellerShopParameters)
                SaveShopParameters(parameters);
        }

        //SaveSellerDatas();
    }

    public void RemoveSoldEquipmentToSellerShop(ShipEquipment shipEquipment)
    {
        currentSellerShopParameters.RemoveSoldEquipment(shipEquipment);

        foreach (ShopWithShopParameters parameters in allShopsWithParameters)
        {
            if (parameters.GetShopParameters == currentSellerShopParameters)
                SaveShopParameters(parameters);
        }

        //SaveSellerDatas();
    }

    public void SaveSellerDatas()
    {
        SaveAllShopsDatas();
    }

    public bool Opened
    {
        get
        {
            return mapDocksInterface.SellerInventoryOpened;
        }
    }

    #region 
    SellersData currentSellersData;
    //ne pas oublier d'assigner les sellers data au SetUp
    public void SaveShopParameters(ShopWithShopParameters parameters)
    {
        SellersData sellersData = PlayerDataSaver.LoadShopDatas();
        if (sellersData != null)
        {
            sellersData.SetSellersData(parameters);
            PlayerDataSaver.SaveSellersDatas(sellersData);
        }
        else
            SaveAllShopsDatas();
    }

    public void LoadAllShopsDatas()
    {
        SellersData sellersData = PlayerDataSaver.LoadShopDatas();
        if (sellersData != null)
        {
            if (sellersData.GetAllSellersDatas != null)
            {
                foreach (SingleSellerData singleSellerData in sellersData.GetAllSellersDatas)
                {
                    string shopName = singleSellerData.GetShopName;
                    foreach(ShopWithShopParameters shop in allShopsWithParameters)
                    {
                        if(shopName == shop.GetShopName)
                        {
                            shop.SetSoldItems(singleSellerData.GetAllSoldEquipments);
                            break;
                        }
                    }
                }
            }
        }
        /*if(sellersData != null)
            allShopsWithParameters = sellersData.GetAllSellersDatas*/
    }

    public void SaveAllShopsDatas()
    {
        PlayerDataSaver.SaveSellersDatas(allShopsWithParameters);
    }
    #endregion
}

[System.Serializable]
public struct /*class*/ ShopWithShopParameters
{
    [SerializeField] string shopName;
    public string GetShopName { get { return shopName; } }

    [SerializeField] MapDockSpot dockSpot;
    public MapDockSpot GetDockSpot { get { return dockSpot; } }

    [SerializeField] SellerShopParameters shopParameters;
    public SellerShopParameters GetShopParameters { get { return shopParameters; } }

    public List<ShipEquipment> GetSoldItems { get { return dockSpot.GetShopParameters.GetSoldEquipments; } }
    public void SetUpDockSpot()
    {
        dockSpot.SetShopParameters(shopParameters);
    }
    public void SetSoldItems(List<ShipEquipment> soldItems)
    {
        foreach (ShipEquipment equip in soldItems)
            dockSpot.GetShopParameters.AddSoldEquipment(equip);
    }
}

[System.Serializable]
public class SellerShopParameters
{
    public SellerShopParameters(SellerShopParameters parameters)
    {
        allBuyableEquipments = parameters.allBuyableEquipments;
        selectedBuyableEquipments = new List<ShipEquipment>();

        List<ShipEquipment> equips = new List<ShipEquipment>(allBuyableEquipments);
        int numberOfEquipments = Random.Range(minNumberOfEquipments, maxNumberOfEquipments);
        for (int i = 0; i < numberOfEquipments; i++)
        {
            if (equips.Count == 0)
                break;

            ShipEquipment equip = equips.GetRandomMemberOfTheList();
            selectedBuyableEquipments.Add(equip);
            equips.Remove(equip);
        }

        soldEquipments = new List<ShipEquipment>();
    }
    public void SetUpSelectedObjects()
    {
        selectedBuyableEquipments = new List<ShipEquipment>();

        List<ShipEquipment> equips = new List<ShipEquipment>(allBuyableEquipments);
        int numberOfEquipments = Random.Range(minNumberOfEquipments, maxNumberOfEquipments);
        for (int i = 0; i < numberOfEquipments; i++)
        {
            if (equips.Count == 0)
                break;

            ShipEquipment equip = equips.GetRandomMemberOfTheList();
            selectedBuyableEquipments.Add(equip);
            equips.Remove(equip);
        }
    }

    [SerializeField] List<ShipEquipment> allBuyableEquipments;
    [SerializeField] int minNumberOfEquipments = 3;
    [SerializeField] int maxNumberOfEquipments = 6;
    List<ShipEquipment> selectedBuyableEquipments;
    public List<ShipEquipment> GetBuyableEquipments
    {
        get
        {
            return selectedBuyableEquipments;
        }
    }

    public void RemoveEquipment(ShipEquipment equipmentToRemove)
    {
        if (selectedBuyableEquipments != null)
            selectedBuyableEquipments.Remove(equipmentToRemove);
    }

    [SerializeField] [HideInInspector] List<ShipEquipment> soldEquipments = new List<ShipEquipment>();
    public List<ShipEquipment> GetSoldEquipments
    {
        get
        {
            if (soldEquipments != null)
                return soldEquipments;
            else
                return new List<ShipEquipment>();
        }
    }

    public void AddSoldEquipment(ShipEquipment equipmentToAdd)
    {
        if (soldEquipments == null)
            soldEquipments = new List<ShipEquipment>();

        soldEquipments.Add(equipmentToAdd);
    }

    public void RemoveSoldEquipment(ShipEquipment equipmentToRemove)
    {
        if (soldEquipments != null)
            soldEquipments.Remove(equipmentToRemove);
    }

    public void RemoveOldestSoldEquipment()
    {
        soldEquipments.RemoveAt(0);
    }
}

