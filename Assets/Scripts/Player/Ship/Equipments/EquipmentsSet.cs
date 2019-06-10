using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EquipmentsSet
{
    /// <summary>
    /// L'équipement placé sur la coque
    /// </summary>
    [Header("Equipments")]
    [SerializeField] ShipEquipment hullEquipment;
    /// <summary>
    /// L'équipement placé en tant qu'arme principale
    /// </summary>
    [SerializeField] ShipEquipment mainWeaponEquipment;
    /// <summary>
    /// L'équipement placé en tant qu'arme secondaire
    /// </summary>
    [SerializeField] ShipEquipment secondaryWeaponEquipment;

    public ShipEquipment GetHullEquipment
    {
        get
        {
            return hullEquipment;
        }
    }

    public ShipEquipment GetMainWeaponEquipment
    {
        get
        {
            return mainWeaponEquipment;
        }
    }

    public ShipEquipment GetSecondaryWeaponEquipment
    {
        get
        {
            return secondaryWeaponEquipment;
        }
    }

    public EquipmentsSet(ShipEquipment hull, ShipEquipment mainWeapon, ShipEquipment secondaryWeapon)
    {
        hullEquipment = hull;
        mainWeaponEquipment = mainWeapon;
        secondaryWeaponEquipment = secondaryWeapon;
    }
}
