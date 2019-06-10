using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCompetenceProtection", menuName = "Equipments and Co/Competences/CompetenceProtection", order = 3)]
public class CompetenceProtection : Competence
{
    #region Usability
    [Header("Protection")]
    [SerializeField] ProtectionParameters protectionParameters;

    public bool UseCompetence(Ship relatedShip)
    {
        relatedShip.SetCurrentProtectionParameters(protectionParameters);

        return true;
    }
    #endregion

    #region Preview
    SpherePreview spawnedPreview;

    public void StartShowPreview(Ship relatedShip)
    {
        GameManager.gameManager.SlwMoManager.StartAimSlowMo(EquipmentType.Hull);

        if (protectionParameters.GeneratesProtectionSphere)
        {
            spawnedPreview = (GameManager.gameManager.PoolManager.GetPreview(PreviewPoolTag.Sphere, PoolInteractionType.GetFromPool) as SpherePreview);
            if (spawnedPreview != null)
                spawnedPreview.ShowPreparePreview(relatedShip, protectionParameters.GetProtectionSphereRadius);
        }
    }

    public override void EndShowPreview()
    {
        GameManager.gameManager.SlwMoManager.StopAimSlowMo();

        if (spawnedPreview != null)
        {
            spawnedPreview.HidePreparePreview();
            spawnedPreview = null;
        }
    }
    #endregion
}
