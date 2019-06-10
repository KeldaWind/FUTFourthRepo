using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryManager
{
    MapDocksInterface mapDocksInterface;
    public void SetUp(MapDocksInterface docksInterface)
    {
        mapDocksInterface = docksInterface;
    }

    [Header("Inventory Content")]
    [SerializeField] List<ShipEquipment> playerEquipments;

    public void SetPlayerEquipments(List<ShipEquipment> equipments)
    {
        if(equipments == null)
        {
            return;
        }
        playerEquipments = equipments;
    }

    public void AddNewEquipmentToInventory(ShipEquipment newEquipment)
    {
        playerEquipments.Add(newEquipment);
    }

    public void RemoveEquipmentFromInventory(ShipEquipment equipmentToRemove)
    {
        playerEquipments.Remove(equipmentToRemove);

        mapDocksInterface.OpenPlayerInventoryPanel(playerEquipments);
    }

    public void UpdatePlayerEquipmentValue()
    {
        if (!Opened)
            return;

        playerEquipments = new List<ShipEquipment>();

        foreach (EquipmentSlotInventory slot in mapDocksInterface.GetPlayerInventorySlots)
        {
            if (slot.GetPlacedEquipment != null)
                playerEquipments.Add(slot.GetPlacedEquipment);
        }
    }

    public bool Opened
    {
        get
        {
            return mapDocksInterface.PlayerInventoryOpened;
        }
    }

    public List<ShipEquipment> GetPlayerInventoryEquipments
    {
        get
        {
            return playerEquipments;
        }
    }
}
