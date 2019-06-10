using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public class MapDocksManager
{
    MapManager mapManager;
    public void SetUp(MapManager mpManager)
    {
        mapManager = mpManager;

        SetUpPlayerInventoryAndSet();

        docksInterface.SetUp(/*mapManager.ChangeDocksModeToPlayerInventory,*/ mapManager.ChangeDocksModeToSellerInventory, mapManager.ChangeDocksModeToSellerRebuyInventory, mapManager.CloseDocksPanel, TryRepairOneArmor, OpenPlayerInventoryOnly);

        inventoryManager.SetUp(docksInterface);
        sellerManager.SetUp(docksInterface);
    }


    [SerializeField] MapDocksInterface docksInterface;
    DocksMode currentDocksMode;

    public void OpenDocksPanel(CinemachineVirtualCamera virtualCamera, MapDockSpot docksSpot)
    {
        docksInterface.OpenDocksPanel(true);

        virtualCamera.gameObject.SetActive(true);

        mapManager.SetLastActivatedVirtualCamera(virtualCamera);
        mapManager.SetOpenedSpecialPlace(docksSpot);

        mapManager.SetPlayerNotPlayable();

        mapManager.ShipCompoManager.OpenEquipedEquipmentPanel();

        ChangeDocksPanelMode(DocksMode.SellersInventory);
    }

    bool playerInventoryOnly;
    public bool PlayerInventoryOnly { get { return playerInventoryOnly; } }

    public void OpenPlayerInventoryOnly()
    {
        if (!docksInterface.PlayerInventoryOpened)
        {
            docksInterface.OpenDocksPanel(false);
            docksInterface.OpenPlayerInventoryPanel(inventoryManager.GetPlayerInventoryEquipments);
            mapManager.SetPlayerNotPlayable();
            docksInterface.CloseSellerInventoryPanel();
            docksInterface.CloseSellerRebuyInventoryPanel();
            playerInventoryOnly = true;            
        }
        else
        {
            docksInterface.CloseDocksPanel();
            docksInterface.ClosePlayerInventoryPanel();
            mapManager.SetPlayerPlayable();            
        }
    }

    public void ChangeDocksPanelMode(DocksMode docksMode)
    {
        playerInventoryOnly = false;
        if (currentDocksMode == docksMode)
            return;

        currentDocksMode = docksMode;

        if (docksMode == DocksMode.Closed)
        {
            docksInterface.CloseDocksPanel();
            docksInterface.ClosePlayerInventoryPanel();
            docksInterface.CloseSellerInventoryPanel();
            docksInterface.CloseSellerRebuyInventoryPanel();
            return;
        }
        /*else if (docksMode == DocksMode.PlayersInventory)
        {
            docksInterface.OpenPlayerInventoryPanel(inventoryManager.GetPlayerInventoryEquipments);

            docksInterface.CloseSellerInventoryPanel();
            docksInterface.CloseSellerRebuyInventoryPanel();
            sellerManager.OpenSellerInventory((mapManager.GetOpenedSpecialPlace as MapDockSpot).GetShopParameters);
        }*/
        else if (docksMode == DocksMode.SellersInventory)
        {
            docksInterface.OpenPlayerInventoryPanel(inventoryManager.GetPlayerInventoryEquipments);

            //docksInterface.ClosePlayerInventoryPanel();
            docksInterface.CloseSellerRebuyInventoryPanel();

            sellerManager.OpenSellerInventory((mapManager.GetOpenedSpecialPlace as MapDockSpot).GetShopParameters);
            docksInterface.OpenSellerInventoryPanel(sellerManager.GetBuyableEquipments);
        }
        else if(docksMode == DocksMode.Rebuy)
        {
            docksInterface.OpenPlayerInventoryPanel(inventoryManager.GetPlayerInventoryEquipments);

            //docksInterface.ClosePlayerInventoryPanel();
            docksInterface.CloseSellerInventoryPanel();

            sellerManager.OpenSellerInventory((mapManager.GetOpenedSpecialPlace as MapDockSpot).GetShopParameters);
            docksInterface.OpenSellerRebuyInventoryPanel(sellerManager.GetSoldEquipments);
        }
    }

    public void CloseDocksPanel()
    {
        mapManager.GetLastActivatedVirtualCamera?.gameObject.SetActive(false);

        mapManager.SetLastActivatedVirtualCamera(null);
        mapManager.SetOpenedSpecialPlace(null);

        mapManager.SetPlayerPlayable();

        mapManager.ShipCompoManager.CloseEquipedEquipmentPanel();

        mapManager.UpdatePlayerEquipments();

        ChangeDocksPanelMode(DocksMode.Closed);

        docksInterface.CloseDocksPanel();
    }

    #region Player's Inventory
    [SerializeField] InventoryManager inventoryManager;
    public InventoryManager InvtrManager
    {
        get
        {
            return inventoryManager;
        }
    }

    public void SetUpPlayerInventoryAndSet()
    {
        PlayerEquipmentsDatas playerEquipmentsDatas = PlayerDataSaver.LoadPlayerEquipmentsDatas();
        if (playerEquipmentsDatas != null)
        {
            IntersceneManager.intersceneManager.GetPlayerDatas.SetPlayerCurrentArmorValue(playerEquipmentsDatas.GetPlayerArmorAmount);
            IntersceneManager.intersceneManager.GetPlayerDatas.SetCurrentPlayerGoldAmount(playerEquipmentsDatas.GetPlayerGoldAmount);

            mapManager.ShipCompoManager.SetPlayerEquipmentsSet(playerEquipmentsDatas.GetPlayerEquipmentsSet);
            List<ShipEquipment> playerEquipmentsInventory = playerEquipmentsDatas.GetPlayerEquipmentsInventory;
            inventoryManager.SetPlayerEquipments(playerEquipmentsInventory);
            mapManager.UpdatePlayerEquipments();

            docksInterface.UpdatePlayerGoldText();

            ShipEquipmentHull hull = playerEquipmentsDatas.GetPlayerEquipmentsSet.GetHullEquipment as ShipEquipmentHull;
            if (hull != null)
            {
                //docksInterface.UpdatePlayerHullLifeText(hull.GetShipMaximumLife);
                docksInterface.UpdatePlayerHullArmorText(Mathf.Clamp(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue, 0, hull.GetShipMaximumArmor), hull.GetShipMaximumArmor);
            }
        }
    }
    #endregion

    #region Seller's Inventory
    [SerializeField] SellerManager sellerManager;
    public SellerManager SllrManager
    {
        get
        {
            return sellerManager;
        }
    }
    #endregion

    #region Equipments Actions
    public void TryEquipEquipment(EquipmentSlot equipmentSlot)
    {
        ShipEquipment equipment = equipmentSlot.GetPlacedEquipment;

        mapManager.ShipCompoManager.EquipSelectedObject(equipmentSlot as EquipmentSlotInventory);
        mapManager.EquipmentsInfoManager.CloseInformationsPanel();

        inventoryManager.UpdatePlayerEquipmentValue();
        mapManager.SavePlayerDatas();

        ShipEquipmentHull hull = equipment as ShipEquipmentHull;
        if (hull != null)
        {
            //docksInterface.UpdatePlayerHullLifeText(hull.GetShipMaximumLife);
            docksInterface.UpdatePlayerHullArmorText(Mathf.Clamp(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue, 0, hull.GetShipMaximumArmor), hull.GetShipMaximumArmor);
        }
    }

    public void TryUpgradeEquipment(EquipmentSlot equipmentSlot)
    {
        ShipEquipment equipmentToUpgrade = equipmentSlot.GetPlacedEquipment;

        if (IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount >= equipmentToUpgrade.PriceToUpgrade)
        {
            IntersceneManager.intersceneManager.GetPlayerDatas.SpendMoney(equipmentToUpgrade.PriceToUpgrade);
            equipmentSlot.SetPlacedEquipment(equipmentToUpgrade.GetUpgradedEquipment);

            mapManager.EquipmentsInfoManager.OpenInformationsPanel(equipmentSlot, false);

            inventoryManager.UpdatePlayerEquipmentValue();
            mapManager.SavePlayerDatas();

            docksInterface.UpdatePlayerGoldText();
        }
        else
        {
            Debug.Log("Not enough money, kid");
        }
    }

    public void TryBuyEquipment(EquipmentSlot equipmentSlot)
    {
        ShipEquipment equipmentToBuy = equipmentSlot.GetPlacedEquipment;

        if (inventoryManager.GetPlayerInventoryEquipments.Count < IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerInventoryCapacity)
        {
            if (IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount >= equipmentToBuy.PriceToBuy)
            {
                IntersceneManager.intersceneManager.GetPlayerDatas.SpendMoney(equipmentToBuy.PriceToBuy);

                if (docksInterface.SellerInventoryOpened)
                {
                    sellerManager.RemoveEquipmentFromSellerShop(equipmentToBuy);
                    docksInterface.OpenSellerInventoryPanel(sellerManager.GetBuyableEquipments);
                }
                else if(docksInterface.SellerRebuyInventoryOpened)
                {
                    sellerManager.RemoveSoldEquipmentToSellerShop(equipmentToBuy);
                    docksInterface.OpenSellerRebuyInventoryPanel(sellerManager.GetSoldEquipments);
                }

                inventoryManager.AddNewEquipmentToInventory(equipmentToBuy);
                docksInterface.OpenPlayerInventoryPanel(inventoryManager.GetPlayerInventoryEquipments);

                mapManager.EquipmentsInfoManager.CloseInformationsPanel();

                inventoryManager.UpdatePlayerEquipmentValue();

                //docksInterface.OpenPlayerInventoryPanel(inventoryManager.GetPlayerInventoryEquipments);
                mapManager.SavePlayerDatas();

                docksInterface.UpdatePlayerGoldText();
            }
            else
            {
                Debug.Log("Not enough money, kid");
            }
        }
        else
            Debug.Log("trop d'objets dans l'inventaire pour acheter");
    }

    public void TrySellEquipment(EquipmentSlot equipmentSlot)
    {
        IntersceneManager.intersceneManager.GetPlayerDatas.EarnMoney(equipmentSlot.GetPlacedEquipment.GetSellingPrice);

        sellerManager.AddSoldEquipmentToSellerShop(equipmentSlot.GetPlacedEquipment);
        docksInterface.OpenSellerRebuyInventoryPanel(sellerManager.GetSoldEquipments);
        docksInterface.CloseSellerInventoryPanel();
        currentDocksMode = DocksMode.Rebuy;

        inventoryManager.RemoveEquipmentFromInventory(equipmentSlot.GetPlacedEquipment);

        mapManager.EquipmentsInfoManager.CloseInformationsPanel();

        inventoryManager.UpdatePlayerEquipmentValue();
        mapManager.SavePlayerDatas();

        docksInterface.UpdatePlayerGoldText();
    }

    public void UpdateGoldText()
    {
        docksInterface.UpdatePlayerGoldText();
    }
    #endregion

    #region Armor and Repair
    [SerializeField] int repairCost = 250;
    public void TryRepairOneArmor()
    {
        if (GameManager.gameManager.Player.EqpmntManager.HullIsRepairable)
        {
            if (IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount >= repairCost)
            {
                IntersceneManager.intersceneManager.GetPlayerDatas.SpendMoney(repairCost);
                IntersceneManager.intersceneManager.GetPlayerDatas.IncreamentArmor(1);

                GameManager.gameManager.Player.EqpmntManager.UpdateArmorAmountAndLifeInterface();

                docksInterface.UpdatePlayerGoldText();
                docksInterface.UpdatePlayerHullArmorText(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue);
            }
            else
                Debug.Log("Not enough money, kid");
        }
        else
            Debug.Log("Coque déjà au max");
    }
    #endregion
}

public enum DocksMode
{
    Closed,
    /// <summary>
    /// Mode dans lequel le joueur peut équiper et/ou améliorer ses équipements
    /// </summary>
    //PlayersInventory,
    /// <summary>
    /// Mode dans lequel le joueur peut acheter des équipements au vendeur
    /// </summary>
    SellersInventory,
    /// <summary>
    /// Mode dans lequel le joueur peut racheter des équipements qu'il a vendu
    /// </summary>
    Rebuy
}