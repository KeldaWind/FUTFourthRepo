using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventory
{
    [SerializeField] ShipEquipment[] playerEquipments;
    public PlayerInventory(List<ShipEquipment> inventoryEquipments)
    {
        playerEquipments = new ShipEquipment[inventoryEquipments.Count];
        for (int i = 0; i < playerEquipments.Length; i++)
            playerEquipments[i] = inventoryEquipments[i];
    }

    public List<ShipEquipment> GetPlayerInventory
    {
        get
        {
            List<ShipEquipment> inventory = new List<ShipEquipment>(playerEquipments);

            return inventory;
        }
    }
}
