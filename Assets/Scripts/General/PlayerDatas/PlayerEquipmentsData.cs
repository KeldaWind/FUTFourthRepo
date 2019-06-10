using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerEquipmentsDatas
{
    [SerializeField] string playerEquipedEquipments;
    [SerializeField] string playerInventoryEquipments;

    public void DebugDatas()
    {
        Debug.Log("equiped : " + playerEquipedEquipments);
        Debug.Log(GetPlayerEquipmentsSet.GetHullEquipment);
        Debug.Log(GetPlayerEquipmentsSet.GetMainWeaponEquipment);
        Debug.Log(GetPlayerEquipmentsSet.GetSecondaryWeaponEquipment);

        Debug.Log("inventory : " + playerInventoryEquipments);
        foreach (ShipEquipment equip in GetPlayerEquipmentsInventory)
        {
            Debug.Log(equip);
        }
    }

    public void SetPlayerEquipmentsData(EquipmentsSet equipedEquipments, List<ShipEquipment> inventoryEquipments, int armorAmount, int goldAmount)
    {
        PlayerInventory playerInventory = new PlayerInventory(inventoryEquipments);

        #region Set
        List<int> playerEquipedEquipmentsIndexes = new List<int>();
        if(equipedEquipments.GetHullEquipment != null)
            playerEquipedEquipmentsIndexes.Add(equipedEquipments.GetHullEquipment.GetEquipmentSaveIndex);
        else
            playerEquipedEquipmentsIndexes.Add(-1);

        if (equipedEquipments.GetMainWeaponEquipment != null)
            playerEquipedEquipmentsIndexes.Add(equipedEquipments.GetMainWeaponEquipment.GetEquipmentSaveIndex);
        else
            playerEquipedEquipmentsIndexes.Add(-1);

        if (equipedEquipments.GetSecondaryWeaponEquipment != null)
            playerEquipedEquipmentsIndexes.Add(equipedEquipments.GetSecondaryWeaponEquipment.GetEquipmentSaveIndex);
        else
            playerEquipedEquipmentsIndexes.Add(-1);

        playerEquipedEquipments = Serializations.SerializeListOfInt(playerEquipedEquipmentsIndexes);
        #endregion

        #region Inventory
        List<int> playerInventoryEquipmentsIndexes = new List<int>();
        
        foreach(ShipEquipment equip in inventoryEquipments)
        {
            playerInventoryEquipmentsIndexes.Add(equip.GetEquipmentSaveIndex);
        }

        playerInventoryEquipments = Serializations.SerializeListOfInt(playerInventoryEquipmentsIndexes);
        #endregion

        playerArmorAmout = armorAmount;

        playerGoldAmount = goldAmount;
    }

    public EquipmentsSet GetPlayerEquipmentsSet
    {
        get
        {
            List<int> playerEquipedEquipmentsIndexes = Serializations.UnserializeListOfInt(playerEquipedEquipments);
            AllGameEquipments allEquipments = AllGameEquipmentsManager.manager.GetAllGameEquipments;

            return new EquipmentsSet(allEquipments.GetEquipment(playerEquipedEquipmentsIndexes[0]), allEquipments.GetEquipment(playerEquipedEquipmentsIndexes[1]), allEquipments.GetEquipment(playerEquipedEquipmentsIndexes[2]));
        }
    }

    public List<ShipEquipment> GetPlayerEquipmentsInventory
    {
        get
        {
            List<ShipEquipment> inventory = new List<ShipEquipment>();

            List<int> playerInventoryEquipmentsIndexes = Serializations.UnserializeListOfInt(playerInventoryEquipments);

            if (AllGameEquipmentsManager.manager != null)
            {
                AllGameEquipments allEquipments = AllGameEquipmentsManager.manager.GetAllGameEquipments;

                foreach (int index in playerInventoryEquipmentsIndexes)
                {
                    ShipEquipment equip = allEquipments.GetEquipment(index);

                    if (equip != null)
                        inventory.Add(equip);
                }
            }

            return inventory;
        }
    }
    public void SetPlayerInventory(List<ShipEquipment> newInventory)
    {
        List<int> playerInventoryEquipmentsIndexes = new List<int>();

        foreach (ShipEquipment equip in newInventory)
        {
            playerInventoryEquipmentsIndexes.Add(equip.GetEquipmentSaveIndex);
        }

        playerInventoryEquipments = Serializations.SerializeListOfInt(playerInventoryEquipmentsIndexes);
    }

    #region Armor
    [SerializeField] int playerArmorAmout;
    public int GetPlayerArmorAmount { get { return playerArmorAmout; } }
    public void SetPlayerArmorAmount(int amount)
    {
        playerArmorAmout = amount;
    }
    #endregion

    #region Gold
    [SerializeField] int playerGoldAmount;
    public int GetPlayerGoldAmount { get { return playerGoldAmount; } }
    public void SetPlayerGoldAmount(int amount)
    {
        playerGoldAmount = amount;
    }
    #endregion
}
