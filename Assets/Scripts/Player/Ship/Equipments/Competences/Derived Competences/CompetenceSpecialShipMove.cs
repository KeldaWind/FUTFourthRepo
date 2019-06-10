using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCompetenceSpecialShipMove", menuName = "Equipments and Co/Competences/CompetenceSpecialShipMove", order = 4)]
public class CompetenceSpecialShipMove : Competence
{
    #region Usability
    [Header("Special Move")]
    [SerializeField] SpecialShipMoveParameters specialShipMoveParameters;

    public bool UseCompetence(Ship relatedShip)
    {
        relatedShip.ShipMvt.StartNewSpecialMove(specialShipMoveParameters);

        return true;
    }
    #endregion

    #region Preview
    Preview spawnedPreview;

    public void StartShowPreview(Ship relatedShip)
    {
        GameManager.gameManager.SlwMoManager.StartAimSlowMo(EquipmentType.Hull);

        if (specialShipMoveParameters.ShipIsAnchored)
        {
            spawnedPreview = GameManager.gameManager.PoolManager.GetPreview(PreviewPoolTag.Anchor, PoolInteractionType.GetFromPool);
            AnchorPreview anchorPreview = spawnedPreview as AnchorPreview;
            if (anchorPreview != null)
            {
                anchorPreview.ShowPreparePreview(relatedShip.transform);
            }
        }
        else if (specialShipMoveParameters.ShipBoost)
        {

        }
    }

    public override void EndShowPreview()
    {
        GameManager.gameManager.SlwMoManager.StopAimSlowMo();

        if (specialShipMoveParameters.ShipIsAnchored)
        {
            AnchorPreview anchorPreview = spawnedPreview as AnchorPreview;
            if (anchorPreview != null)
            {
                anchorPreview.StartLaunchedPreview(Vector3.zero, new List<Vector3>(), specialShipMoveParameters.GetDuration);
            }
        }
        else if (specialShipMoveParameters.ShipBoost)
        {

        }
    }
    #endregion
}
