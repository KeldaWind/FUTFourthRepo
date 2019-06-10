using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MenuSlot
{
    public void SetUp(ShipEquipment equipment)
    {
        placedEquipment = equipment;
        ActualizeObjectImage();
    }

    public bool JustBeenPlace
    {
        get
        {
            return placementTimer != 0;
        }
    }
    float placementTimer;

    private void Update()
    {
        if (placementTimer > 0)
            placementTimer -= Time.deltaTime;
        else if (placementTimer < 0)
            placementTimer = 0;
    }

    [Header("Equipment")]
    [SerializeField] protected ShipEquipment placedEquipment;
    public ShipEquipment GetPlacedEquipment
    {
        get
        {
            return placedEquipment;
        }
    }
    [SerializeField] EquipmentInformationsOpeningType equipmentActionsType;
    public EquipmentInformationsOpeningType GetEquipmentActionsType
    {
        get
        {
            return equipmentActionsType;
        }
    }

    public EquipmentType GetEquipmentType
    {
        get
        {
            return placedEquipment.GetEquipmentType;
        }
    }

    public virtual void SetPlacedEquipment(EquipmentSlot originSlot, bool replace)
    {
        ShipEquipment newEquip = originSlot.placedEquipment;

        if (replace)
            originSlot.SetPlacedEquipment(this, false);

        placedEquipment = newEquip;

        if(placedEquipment != null)
            objectImage.sprite = placedEquipment.GetEquipmentInformations.GetEquipmentIcon;
        else
            objectImage.sprite = null;

        placementTimer = 0.01f;
    }

    public void SetPlacedEquipment(ShipEquipment newEquipment)
    {
        placedEquipment = newEquipment;

        if (placedEquipment != null)
            objectImage.sprite = placedEquipment.GetEquipmentInformations.GetEquipmentIcon;
        else
            objectImage.sprite = null;
    }

    public override bool IsEmpty()
    {
        return placedEquipment == null;
    }

    #region Rendering
    public override Sprite GetSlotIcon()
    {
        if (placedEquipment != null)
            return placedEquipment.GetEquipmentInformations.GetEquipmentIcon;
        else
            return null;
    }
    #endregion
}
