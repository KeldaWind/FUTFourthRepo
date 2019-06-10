using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShipCompositionManager
{
    [SerializeField] EquipmentCompositionInterface compositionInterface;

    public void SetPlayerEquipmentsSet(EquipmentsSet set)
    {

        compositionInterface.SetEquipmentSet(set);
    }

    public void OpenEquipedEquipmentPanel()
    {
        compositionInterface.OpenEquipedEquipmentPanel();
    }

    public void CloseEquipedEquipmentPanel()
    {
        compositionInterface.CloseEquipedEquipmentPanel();
    }


    public EquipmentsSet ComposeEquipmentSet()
    {
        return new EquipmentsSet(compositionInterface.HullSlot.GetPlacedEquipment, compositionInterface.MainWeaponSlot.GetPlacedEquipment, compositionInterface.SecondaryWeaponSlot.GetPlacedEquipment);
    }

    public void EquipSelectedObject(EquipmentSlotInventory equipmentSlotInventory)
    {
        switch (equipmentSlotInventory.GetEquipmentType)
        {
            case (EquipmentType.Hull):
                ShipEquipmentHull hullEquipment = equipmentSlotInventory.GetPlacedEquipment as ShipEquipmentHull;
                if(hullEquipment == null)
                {
                    Debug.LogWarning("Pas une coque : impossible d'équiper");
                    break;
                }

                compositionInterface.HullSlot.SetPlacedEquipment(equipmentSlotInventory, true);
                GameManager.gameManager.PlrInterface.SetLifeBar(hullEquipment.GetShipMaximumLife, Mathf.Clamp(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue, 0, hullEquipment.GetShipMaximumArmor), hullEquipment.GetShipMaximumArmor);
                break;

            case (EquipmentType.Canon):
                compositionInterface.MainWeaponSlot.SetPlacedEquipment(equipmentSlotInventory, true);
                break;

            case (EquipmentType.Catapult):
                compositionInterface.SecondaryWeaponSlot.SetPlacedEquipment(equipmentSlotInventory, true);
                break;
        }
    }
}
