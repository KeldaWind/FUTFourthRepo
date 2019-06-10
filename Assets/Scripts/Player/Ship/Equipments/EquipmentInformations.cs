using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EquipmentInformations
{
    [SerializeField] string equipmentName;
    public string GetEquipmentName
    {
        get
        {
            return equipmentName;
        }
    }

    [TextArea(3, 10)]
    [SerializeField] string equipmentDescription;
    public string GetEquipmentDescription
    {
        get
        {
            return equipmentDescription;
        }
    }

    [SerializeField] Sprite equipmentIcon;
    public Sprite GetEquipmentIcon
    {
        get
        {
            return equipmentIcon;
        }
    }

    [SerializeField] WeaponInformationType weaponType;
    public WeaponInformationType GetWeaponType
    {
        get
        {
            return weaponType;
        }
    }
    [SerializeField] WeaponInformationShotType weaponShotType;
    public WeaponInformationShotType GetWeaponShotType
    {
        get
        {
            return weaponShotType;
        }
    }
    [SerializeField] WeaponInformationEffect weaponEffect;
    public WeaponInformationEffect GetWeaponEffect
    {
        get
        {
            return weaponEffect;
        }
    }
}
