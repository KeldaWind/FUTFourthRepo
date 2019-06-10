using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCompositionInterface : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject equipedEquipmentsPanel;
    [SerializeField] EquipmentSlotShip hullSlot;
    public EquipmentSlotShip HullSlot { get { return hullSlot; } }

    [SerializeField] EquipmentSlotShip mainWeaponSlot;
    public EquipmentSlotShip MainWeaponSlot { get { return mainWeaponSlot; } }

    [SerializeField] EquipmentSlotShip secondaryWeaponSlot;
    public EquipmentSlotShip SecondaryWeaponSlot { get { return secondaryWeaponSlot; } }

    public void SetEquipmentSet(EquipmentsSet set)
    {
        hullSlot.SetPlacedEquipment(set.GetHullEquipment);
        mainWeaponSlot.SetPlacedEquipment(set.GetMainWeaponEquipment);
        secondaryWeaponSlot.SetPlacedEquipment(set.GetSecondaryWeaponEquipment);
    }

    public void OpenEquipedEquipmentPanel()
    {
        equipedEquipmentsPanel.SetActive(true);
    }

    public void CloseEquipedEquipmentPanel()
    {
        equipedEquipmentsPanel.SetActive(false);
    }
}
