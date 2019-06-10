using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestionnaire des équipements en jeu
/// </summary>
[System.Serializable]
public class EquipmentsManager
{
    PlayerShip relatedShip;

    public void SetUpSystem(PlayerShip ship)
    {
        relatedShip = ship;
    }

    /// <summary>
    /// Parent sur lequel seront instantiés les équipements
    /// </summary>
    [Header("Equipments")]
    [SerializeField] Transform equipmentObjectsParent;
    /// <summary>
    /// Rotation de base du parent des équipements
    /// </summary>
    Vector3 normalEquipParentRotation;
    /// <summary>
    /// Direction actuelle du bateau
    /// </summary>
    Vector3 shipDirection;
    /// <summary>
    /// Renvoie la rotation actuelle du parent des équipements
    /// </summary>
    public Quaternion GetShipRotation
    {
        get
        {
            return equipmentObjectsParent.localRotation;
        }
    }

    /// <summary>
    /// Equipements actuellement équipés sur le bateau
    /// </summary>
    [SerializeField] EquipmentsSet equipedEquipments;
    /// <summary>
    /// Equipements actuellement équipés sur le bateau
    /// </summary>
    public EquipmentsSet EquipedEquipments
    {
        get
        {
            return equipedEquipments;
        }
    }

    List<EquipmentObject> instantiatedObjects;

    /// <summary>
    /// Initialisation des équipements du joueur
    /// </summary>
    /// <param name="equipments">Equipements à équiper sur le joueurs</param>
    public void SetUpEquipments(EquipmentsSet equipments)
    {
        ClearInstantiatedObjects();

        ShipEquipment hullEquipmentClone = null;
        if (equipments.GetHullEquipment != null)
        {
            ShipEquipmentHull hullEquip = equipments.GetHullEquipment as ShipEquipmentHull;
            if (hullEquip == null)
            {
                Debug.LogWarning("Pas une coque : impossible d'équiper");
            }
            else
            {
                hullEquipmentClone = ScriptableObject.Instantiate(equipments.GetHullEquipment);
                List<EquipmentObject> hullEquipmentsObjects = hullEquipmentClone.InstantiateAllObjects(/*relatedShip.transform*/equipmentObjectsParent, relatedShip);

                AddObjectsToInstantiatedObjects(hullEquipmentsObjects);
                relatedShip.PlrInterface.SetLifeBar(hullEquip.GetShipMaximumLife, Mathf.Clamp(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue, 0, hullEquip.GetShipMaximumArmor), hullEquip.GetShipMaximumArmor);
                relatedShip.LfManager.SetUpArmorAndLife(hullEquip.GetShipMaximumLife, Mathf.Clamp(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue, 0, hullEquip.GetShipMaximumArmor), hullEquip.GetShipMaximumArmor);

                if(hullEquip.GetHullPrefab != null)
                    relatedShip.ShipRenderManager.InstantiateHullRenderer(hullEquip.GetHullPrefab, (equipments.GetMainWeaponEquipment != null ? equipments.GetMainWeaponEquipment.GetEquipmentInformations.GetWeaponType : WeaponInformationType.MultiCanons));
                if (hullEquip.GetHullHitboxDimensions != Vector3.zero)
                    relatedShip.SetShipBoxColliderDimensions(hullEquip.GetHullHitboxDimensions);
            }
        }

        ShipEquipment mainWeaponEquipmentClone = null;
        if (equipments.GetMainWeaponEquipment != null)
        {
            mainWeaponEquipmentClone = ScriptableObject.Instantiate(equipments.GetMainWeaponEquipment);
            List<EquipmentObject> mainWeaponEquipmentsObjects = mainWeaponEquipmentClone.InstantiateAllObjects(/*relatedShip.transform*/equipmentObjectsParent, relatedShip);

            AddObjectsToInstantiatedObjects(mainWeaponEquipmentsObjects);
        }

        ShipEquipment secondaryWeaponEquipmentClone = null;
        if (equipments.GetSecondaryWeaponEquipment != null)
        {
            secondaryWeaponEquipmentClone = ScriptableObject.Instantiate(equipments.GetSecondaryWeaponEquipment);
            List<EquipmentObject> secondaryWeaponEquipmentsObjects = secondaryWeaponEquipmentClone.InstantiateAllObjects(/*relatedShip.transform*/equipmentObjectsParent, relatedShip);

            AddObjectsToInstantiatedObjects(secondaryWeaponEquipmentsObjects);
        }

        #region Composition deck/compétences
        equipedEquipments = new EquipmentsSet(hullEquipmentClone, mainWeaponEquipmentClone, secondaryWeaponEquipmentClone);

        normalEquipParentRotation = equipmentObjectsParent.localRotation.eulerAngles;
        #endregion

        #region Hull
        ShipEquipmentHull hull = hullEquipmentClone as ShipEquipmentHull;
        if (hull != null)
            relatedShip.ShipMvt.AffectMovementValues(hull.GetMovementParameters);
        else
            Debug.LogWarning("L'équipement affecté sur l'emplacement de la coque n'est pas de type 'Hull'");
        #endregion
    }

    #region Instantiated Objects
    public void ClearInstantiatedObjects()
    {
        if (instantiatedObjects != null)
        {
            foreach (EquipmentObject equipmentObj in instantiatedObjects)
            {
                Object.Destroy(equipmentObj.gameObject);
            }
        }

        instantiatedObjects = new List<EquipmentObject>();
    }

    public void AddObjectsToInstantiatedObjects(List<EquipmentObject> newObjects)
    {
        foreach (EquipmentObject equipmentObj in newObjects)
        {
            instantiatedObjects.Add(equipmentObj);
        }
    }
    #endregion

    #region Armor
    public void UpdateArmorAmountAndLifeInterface()
    {
        ShipEquipmentHull hullEquip = EquipedEquipments.GetHullEquipment as ShipEquipmentHull;
        if (hullEquip != null)
        {
            relatedShip.LfManager.SetUpArmorAndLife(hullEquip.GetShipMaximumLife, Mathf.Clamp(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue, 0, hullEquip.GetShipMaximumArmor), hullEquip.GetShipMaximumArmor);
            relatedShip.PlrInterface.SetLifeBar(hullEquip.GetShipMaximumLife, Mathf.Clamp(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue, 0, hullEquip.GetShipMaximumArmor), hullEquip.GetShipMaximumArmor);

            PlayerEquipmentsDatas equipmentsData = PlayerDataSaver.LoadPlayerEquipmentsDatas();
            equipmentsData.SetPlayerArmorAmount(relatedShip.LfManager.GetCurrentArmorAmount);
            equipmentsData.SetPlayerGoldAmount(IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerGoldAmount);
            PlayerDataSaver.SavePlayerEquipmentsDatas(equipmentsData);
        }
    }

    public bool HullIsRepairable
    {
        get
        {
            ShipEquipmentHull hullEquip = EquipedEquipments.GetHullEquipment as ShipEquipmentHull;
            if (hullEquip != null)
            {
                return IntersceneManager.intersceneManager.GetPlayerDatas.GetPlayerCurrentArmorValue < hullEquip.GetShipMaximumArmor;
            }

            return false;
        }
    }
    #endregion
}
