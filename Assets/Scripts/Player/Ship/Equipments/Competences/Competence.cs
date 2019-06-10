using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCompetence" , menuName = "Equipments and Co/Competences/Competence", order = 0)]
public class Competence : ScriptableObject
{
    /// <summary>
    /// Nom de la compétence
    /// </summary>
    [Header("Informations")]
    [SerializeField] protected string competenceName;
    /// <summary>
    /// Nom de la compétence
    /// </summary>
    public string GetCompetenceName
    {
        get
        {
            return competenceName;
        }
    }

    /// <summary>
    /// Visuel de la compétence
    /// </summary>
    [SerializeField] protected Sprite competenceImage;
    /// <summary>
    /// Visuel de la compétence
    /// </summary>
    public Sprite GetCompetenceImage
    {
        get
        {
            return competenceImage;
        }
    }

    /// <summary>
    /// Actualise la compétence et sa visée
    /// </summary>
    /// <param name="aimingPos">Position actuellement visée</param>
    public virtual void UpdateCompetence(Vector3 aimingPos)
    {

    }

    /// <summary>
    /// Utilise la compétence
    /// </summary>
    /// <param name="aimingPos">Position actuellement visée</param>
    /// <returns>Renvoie vrai si la compétence a bien été lancée</returns>
    public virtual bool UseCompetence(Vector3 aimingPos)
    {
        return true;
    }

    public virtual bool UseCompetence(ShipMovements relatedShip)
    {
        return true;
    }

    public virtual bool IsAimCompetence()
    {
        return false;
    }

    #region Cooldown
    [Header("Cooldown")]
    [SerializeField] float competenceCooldown;
    public float GetCompetenceCooldown
    {
        get
        {
            return competenceCooldown;
        }
    }
    #endregion

    #region Related Equipments
    /// <summary>
    /// Equipement relié à cette compétence
    /// </summary>
    ShipEquipment linkedEquipment;

    /// <summary>
    /// Equipement relié à cette compétence
    /// </summary>
    public ShipEquipment GetLinkEquipment
    {
        get
        {
            return linkedEquipment;
        }
    }

    /// <summary>
    /// Lie à cette compétence un équipement qui servira à la lancer
    /// </summary>
    /// <param name="shipEquipment"></param>
    public void LinkEquipment(ShipEquipment shipEquipment)
    {
        linkedEquipment = shipEquipment;
    }
    #endregion

    #region Preview
    /// <summary>
    /// Commence la prévisualisation de la compétence en préparation
    /// </summary>
    public virtual void StartShowPreview()
    {
        foreach(EquipmentObject equipObject in linkedEquipment.GetAllSpawnedEquipments)
        {
            equipObject.StartShowPreview();
        }
    }

    /// <summary>
    /// Finit la prévisualisation de la compétence en préparation
    /// </summary>
    public virtual void EndShowPreview()
    {
        foreach (EquipmentObject equipObject in linkedEquipment.GetAllSpawnedEquipments)
        {
            equipObject.EndShowPreview();
        }
    }

    /// <summary>
    /// Commence la prévisualisation de la compétence lancée
    /// </summary>
    public virtual void StartLaunchedPreview()
    {
        foreach (EquipmentObject equipObject in linkedEquipment.GetAllSpawnedEquipments)
        {
            equipObject.StartLaunchedPreview();
        }
    }
    #endregion
}
