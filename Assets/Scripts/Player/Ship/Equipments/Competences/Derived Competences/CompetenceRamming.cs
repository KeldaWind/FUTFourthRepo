using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCompetenceRamming", menuName = "Equipments and Co/Competences/CompetenceRamming", order = 2)]
public class CompetenceRamming : Competence
{
    float minPreviewDistance = 20;

    public RammingParameters rammingParameters;
    ArrowPreview spawnedPreview;

    public override bool UseCompetence(ShipMovements relatedShip)
    {
        if (relatedShip.GetCurrentRammingParameters.IsAttacking)
            return false;

        relatedShip.StartRamming(rammingParameters);
        return true;
    }

    public void StartShowPreview(Vector3 shipPosition, Vector3 shipDirection)
    {
        GameManager.gameManager.SlwMoManager.StartAimSlowMo(EquipmentType.Hull);
        spawnedPreview = GameManager.gameManager.PoolManager.GetPreview(PreviewPoolTag.Arrow, PoolInteractionType.GetFromPool) as ArrowPreview;
        spawnedPreview.gameObject.SetActive(true);

        Vector3 destination = shipDirection * GetTravelledDistance() * (rammingParameters.IsTurnAroundCompetence ? -1 : 1);

        if (destination.magnitude < minPreviewDistance)
            destination = destination.normalized * minPreviewDistance;

        spawnedPreview.GenerateArrowMesh(shipPosition, shipPosition + destination);
    }

    public void UpdateCompetence(Vector3 shipPosition, Vector3 shipDirection)
    {
        Vector3 destination = shipDirection * GetTravelledDistance() * (rammingParameters.IsTurnAroundCompetence ? -1 : 1);

        if (destination.magnitude < minPreviewDistance)
            destination = destination.normalized * minPreviewDistance;

        spawnedPreview.UpdateMesh(shipPosition, shipPosition + destination);
    }

    public override void EndShowPreview()
    {
        GameManager.gameManager.SlwMoManager.StopAimSlowMo();
        spawnedPreview.ClearMesh();
        spawnedPreview.gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnPreview(spawnedPreview);
    }

    public float GetTravelledDistance()
    {
        float coeff = 5 / 5.4f;

        float distance = rammingParameters.AttackSpeed * rammingParameters.AttackDuration * coeff;

        if (distance != 0)
            return distance;
        else
            return 1;
    }
}
