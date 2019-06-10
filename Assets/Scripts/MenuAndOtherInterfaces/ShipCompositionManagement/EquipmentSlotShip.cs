using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlotShip : EquipmentSlot
{
    [Header("Type")]
    [SerializeField] EquipmentType equipmentType;

    public override void SetPlacedEquipment(EquipmentSlot originSlot, bool replace)
    {
        if (replace)
        {
            if (originSlot.GetEquipmentType == equipmentType)
            {
                base.SetPlacedEquipment(originSlot, replace);
            }
        }
        else
        {
            base.SetPlacedEquipment(originSlot, replace);
        }

        if (MapManager.mapManager != null)
            MapManager.mapManager.UpdatePlayerEquipments();
    }
}
