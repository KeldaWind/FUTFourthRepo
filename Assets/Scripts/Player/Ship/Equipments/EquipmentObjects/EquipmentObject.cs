using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objet physique correspondant à un équipement du joueur
/// </summary>
public class EquipmentObject : MonoBehaviour
{
    /// <summary>
    /// Initialisation de l'équipement
    /// </summary>
    /// <param name="equipmentParents">Parent de cet équipement/param>
    public virtual void SetUp(Transform equipmentParents, Ship ship)
    {

    }

    #region Being Used
    /// <summary>
    /// Vérifie si cet objet est actuellement en cours d'utilisation
    /// </summary>
    /// <returns>L'objet est en cours d'utilisation</returns>
    public virtual bool CheckIfBeingUsed()
    {
        return false;
    }
    #endregion

    #region Preview
    /// <summary>
    /// Commence la prévisualisation de préparation d'une compétence
    /// </summary>
    public virtual void StartShowPreview()
    {

    }

    /// <summary>
    /// Actualise la prévisualisation de préparation d'une compétence
    /// </summary>
    /// <param name="aimPosition">Position visée</param>
    public virtual void UpdatePreview(Vector3 aimPosition)
    {

    }

    /// <summary>
    /// Finit la prévisualisation de préparation d'une compétence
    /// </summary>
    public virtual void EndShowPreview()
    {

    }

    /// <summary>
    /// Commence la prévisualisation de lancement d'une compétence
    /// </summary>
    public virtual void StartLaunchedPreview()
    {

    }

    #endregion 
}
