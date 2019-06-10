using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Les données du joueur qui doivent être conservées entre les scènes (équipements,...)
/// </summary>
[System.Serializable]
public class PlayerIntersceneDatas 
{
    /// <summary>
    /// Liste des équipements actuellement équipés par le joueur
    /// </summary>
    [Header("Equipments")]
    #region Equiped Equipments
    [SerializeField] EquipmentsSet playerEquipedEquipments;
    /// <summary>
    /// Liste des équipements actuellement équipés par le joueur
    /// </summary>
    public EquipmentsSet GetPlayerEquipedEquipments
    {
        get
        {
            return playerEquipedEquipments;
        }
    }

    public void SetEquipedEquipements(EquipmentsSet equipedEquipments)
    {
        playerEquipedEquipments = equipedEquipments;
    }
    #endregion
    [SerializeField] int playerInventoryCapacity;
    public int GetPlayerInventoryCapacity { get { return playerInventoryCapacity; } }

    [Header("Armor")]
    [SerializeField] int playerCurrentArmorValue;
    public int GetPlayerCurrentArmorValue { get { return playerCurrentArmorValue; } }
    /// <summary>
    /// A appeler quand le joueur achète des points d'armure ET quand il quitte une arène 
    /// </summary>
    public void SetPlayerCurrentArmorValue(int armor)
    {
        playerCurrentArmorValue = armor;
    }

    public void IncreamentArmor(int add)
    {
        playerCurrentArmorValue += add;
    }

    [Header("Money")]
    [SerializeField] int playerGoldAmount;
    public int GetPlayerGoldAmount
    {
        get
        {
            return playerGoldAmount;
        }
    }
    public void SetCurrentPlayerGoldAmount(int amount)
    {
        playerGoldAmount = amount;
    }

    public void EarnMoney(int amount)
    {
        playerGoldAmount += Mathf.Abs(amount);
    }

    public void SpendMoney(int amount)
    {
        playerGoldAmount -= Mathf.Abs(amount);
    }

    public void Reinitialize()
    {
        playerGoldAmount = 0;
        playerCurrentArmorValue = 0;
        playerEquipedEquipments = new EquipmentsSet();
    }
}
