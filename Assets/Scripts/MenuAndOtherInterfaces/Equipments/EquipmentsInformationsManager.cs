using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EquipmentsInformationsManager
{
    MapManager mapManager;
    public void SetUp(MapManager mpManager)
    {
        mapManager = mpManager;

        informationsInterface.SetUp(mapManager.TryEquipSelectedEquipment, mapManager.TryUpgradeSelectedEquipment, mapManager.TryBuySelectedEquipment, mapManager.AskSellSelectedEquipment, CloseInformationsPanel);
    }

    public void SetUpForArena(GameButton.InteractionDeleguate throwInteraction)
    {
        informationsInterface.SetUpOnlyThrow(throwInteraction);
    }

    [SerializeField] EquipmentsInformationsInterface informationsInterface;

    EquipmentSlot currentEquipmentSlot;
    public EquipmentSlot GetCurrentEquipmentSlot
    {
        get
        {
            return currentEquipmentSlot;
        }
    }
    ShipEquipment currentEquipment;

    public void OpenInformationsPanel(EquipmentSlot slot, bool noInteractions)
    {
        currentEquipmentSlot = slot;
        currentEquipment = currentEquipmentSlot.GetPlacedEquipment;
        informationsInterface.OpenInformationPanel(currentEquipment.GetEquipmentInformations, currentEquipment.GetEquipmentType);

        if (noInteractions)
        {
            informationsInterface.CloseAllActions();
        }
        else
        {
            switch (slot.GetEquipmentActionsType)
            {
                case (EquipmentInformationsOpeningType.EquipOrUpgrade):
                    informationsInterface.OpenPlayerInventoryEquipmentActions(currentEquipment.Upgradable, currentEquipment.PriceToUpgrade, currentEquipment.GetSellingPrice);
                    break;

                case (EquipmentInformationsOpeningType.Upgrade):
                    informationsInterface.OpenPlayerEquippedEquipmentActions(currentEquipment.Upgradable, currentEquipment.PriceToUpgrade);
                    break;


                case (EquipmentInformationsOpeningType.Buy):
                    informationsInterface.OpenSellerEquipmentActions(currentEquipment.PriceToBuy);
                    break;

                case (EquipmentInformationsOpeningType.Throw):
                    informationsInterface.OpenThrowEquipmentActions();
                    break;
            }
        }
        
        if(currentEquipment.GetEquipmentType == EquipmentType.Hull)
        {
            ShipEquipmentHull hull = currentEquipment as ShipEquipmentHull;
            if(hull != null)
                informationsInterface.OpenHullInformations(hull.GetShipMaximumLife, hull.GetShipMaximumArmor);
        }
        else
        {
            EquipmentInformations infos = currentEquipment.GetEquipmentInformations;
            informationsInterface.OpenWeaponInformations(infos.GetWeaponType, infos.GetWeaponShotType, infos.GetWeaponEffect);
        }
    }

    public void CloseInformationsPanel()
    {
        currentEquipmentSlot = null;
        currentEquipment = null;

        informationsInterface.CloseInformationsPanel();
    }
}

public enum EquipmentInformationsOpeningType
{
    Upgrade,
    EquipOrUpgrade,
    Buy,
    Throw
}

