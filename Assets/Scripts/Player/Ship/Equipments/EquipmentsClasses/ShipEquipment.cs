using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equipement applicable sur le joueur. Se compose de compétences, d'objets, de passifs et de statistiques
/// </summary>
[CreateAssetMenu(fileName = "NewEquipment", menuName = "Equipments and Co/Equipment/BaseEquipment", order = 0)]
public class ShipEquipment : ScriptableObject
{
    [SerializeField] int equipmentSaveIndex;
    public int GetEquipmentSaveIndex { get { return equipmentSaveIndex; } }

    /// <summary>
    /// Type de cet équipement. Définit l'emplacement sur lequel il pourra être placé.
    /// </summary>
    [SerializeField] EquipmentType equipmentType;
    /// <summary>
    /// Type de cet équipement. Définit l'emplacement sur lequel il pourra être placé.
    /// </summary>
    public EquipmentType GetEquipmentType
    {
        get
        {
            return equipmentType;
        }
    }

    [Header("Informations")]
    [SerializeField] EquipmentInformations equipmentInformations;
    public EquipmentInformations GetEquipmentInformations
    {
        get
        {
            return equipmentInformations;
        }
    }

    /// <summary>
    /// Compétences associées à cet équipement
    /// </summary>
    [Header("Competences")]

    [SerializeField] Competence equipmentPrimaryCompetence;
    public Competence GetPrimaryComp
    {
        get
        {
            return equipmentPrimaryCompetence;
        }
    }

    [SerializeField] EquipmentDirection equipmentDirection;
    public EquipmentDirection GetEquipmentDirection
    {
        get
        {
            return equipmentDirection;
        }
    }

    /// <summary>
    /// Passifs associés à cet équipement
    /// </summary>
    [Header("Passives")]    
    [SerializeField] EquipmentPassive[] relatedPassives;

    /// <summary>
    /// Objets physiques associés à cet équipement
    /// </summary>
    [Header("Object")]
    [SerializeField] EquipmentObject[] relatedEquipmentObjectsPrefabs;
    /// <summary>
    /// Objets physiques associés à cet équipement, instantiés au début de la partie
    /// </summary>
    List<EquipmentObject> spawnedEquipments;
    /// <summary>
    /// Objets physiques associés à cet équipement, instantiés au début de la partie
    /// </summary>
    public List<EquipmentObject> GetAllSpawnedEquipments
    {
        get
        {
            return spawnedEquipments;
        }
    }

    /// <summary>
    /// Instantie tous les objets physiques reliés à cet équipement
    /// </summary>
    /// <param name="parent">Objet sur lequel les équipements vont être instanciés</param>
    public List<EquipmentObject> InstantiateAllObjects(Transform parent, Ship ship)
    {
        spawnedEquipments = new List<EquipmentObject>();

        foreach (EquipmentObject equipObject in relatedEquipmentObjectsPrefabs)
        {
            EquipmentObject spawnedEquip = Instantiate(equipObject, parent);

            spawnedEquip.SetUp(parent, ship);

            spawnedEquipments.Add(spawnedEquip);
        }

        return spawnedEquipments;
    }

    /// <summary>
    /// Renvoie si cet équipement est utilisable ou non
    /// </summary>
    public bool Usable
    {
        get
        {
            foreach(EquipmentObject obj in spawnedEquipments)
            {
                if (obj.CheckIfBeingUsed())
                    return false;
            }
            return true; ;
        }
    }

    #region Buyability and Upgradability
    [Header("Buy and Upgrade")]
    [SerializeField] EquipmentQuality equipmentQuality;
    public EquipmentQuality GetEquipmentQuality
    {
        get
        {
            return equipmentQuality;
        }
    }

    [SerializeField] int priceToBuy;
    public int PriceToBuy
    {
        get
        {
            return priceToBuy;
        }
    }

    [SerializeField] int priceToUpgrade;
    public int PriceToUpgrade
    {
        get
        {
            return priceToUpgrade;
        }
    }

    [SerializeField] int sellingPrice;
    public int GetSellingPrice { get { return sellingPrice; } }

    [SerializeField] ShipEquipment upgradedEquipment;
    public ShipEquipment GetUpgradedEquipment
    {
        get
        {
            return upgradedEquipment;
        }
    }

    public bool Upgradable
    {
        get
        {
            return upgradedEquipment != null;
        }
    }
    #endregion
}

public enum EquipmentType
{
    Hull, Canon, Catapult
}

public enum EquipmentDirection
{
    None, Front, Sides, Back
}

public enum EquipmentQuality
{
    Common, Rare, Legendary
}