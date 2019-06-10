using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Compétence permettant de lancer une compétence de tie
/// </summary>
[CreateAssetMenu(fileName = "NewCompetenceShoot", menuName = "Equipments and Co/Competences/CompetenceShoot", order = 1)]
public class CompetenceShoot : Competence
{
    bool startedShowPreview;

    /// <summary>
    /// Paramètres de tir de cette compétence
    /// </summary>
    public ShootParameters shootParameters;
    
    /// <summary>
    /// Définit si cette compétence permet au joueur de viser précisément ou non (pour les projectiles autres que les boulders)
    /// </summary>
    public bool aiming;

    /// <summary>
    /// Définit si cette compétence, lorsqu'elle est visée, bloque sa direction et non pas la position lorsqu'elle commence à tirer
    /// </summary>
    public bool lockDirectionAfterShoot;

    /// <summary>
    /// Utilise la compétence
    /// </summary>
    /// <param name="aimingPos">Position actuellement visée</param>
    /// <returns>Renvoie vrai si la compétence a bien été lancée</returns>
    public override bool UseCompetence(Vector3 aimingPos)
    {
        if (GameManager.gameManager == null)
            return false;

        if (!GetLinkEquipment.Usable)
            return false;

        foreach (EquipmentObject equipObject in GetLinkEquipment.GetAllSpawnedEquipments)
        {
            EquipmentObjectShoot shootObject = equipObject as EquipmentObjectShoot;

            if(shootObject != null)
            {
                shootObject.Shoot(shootParameters, aimingPos, lockDirectionAfterShoot, GenerateProjectileSpecialParameters);
            }
        }

        if (startedShowPreview)
        {
            StartLaunchedPreview();
            startedShowPreview = false;
        }

        return true;
    }

    public override bool IsAimCompetence()
    {
        return aiming || GameManager.gameManager.PoolManager.GetProjectile(shootParameters.GetCurrentSalvo.GetProjectileType, PoolInteractionType.PeekPrefab).IsBoulder;
    }

    #region Aiming
    /// <summary>
    /// Actualise la compétence et sa visée
    /// </summary>
    /// <param name="aimingPos">Position actuellement visée</param>
    public override void UpdateCompetence(Vector3 aimingPos)
    {
        if (aiming)
        {
            foreach (EquipmentObject equipObject in GetLinkEquipment.GetAllSpawnedEquipments)
            {
                EquipmentObjectShoot shootObject = equipObject as EquipmentObjectShoot;

                if (shootObject != null)
                {
                    shootObject.UpdateAiming(aimingPos, true);
                }
            }
        }
        else
        {
            foreach (EquipmentObject equipObject in GetLinkEquipment.GetAllSpawnedEquipments)
            {
                EquipmentObjectShoot shootObject = equipObject as EquipmentObjectShoot;

                if (shootObject != null)
                    shootObject.UpdateAiming(aimingPos, false);
            }
        }
    }
    #endregion

    #region Preview
    bool isNotPlayerComp;

    public void SetIsNotPlayerComp()
    {
        isNotPlayerComp = true;
    }

    /// <summary>
    /// Commence à prévisualiser la préparation de la compétence
    /// </summary>
    public override void StartShowPreview()
    {
        foreach (EquipmentObject equipObject in GetLinkEquipment.GetAllSpawnedEquipments)
        {
            EquipmentObjectShoot shootObject = equipObject as EquipmentObjectShoot;

            if(shootObject != null)
                shootObject.StartShowPreview(shootParameters.GetCurrentSalvo);
        }

        /*if (aiming || shootParameters.GetCurrentSalvo.GetProjPrefab.GetProjectileType == ProjectileType.CatapultBoulder)
        {*/
        if(!isNotPlayerComp)
            GameManager.gameManager.SlwMoManager.StartAimSlowMo(GetLinkEquipment.GetEquipmentType);
        //}

        startedShowPreview = true;
    }

    /// <summary>
    /// Finit à prévisualiser la préparation compétence
    /// </summary>
    public override void EndShowPreview()
    {
        foreach (EquipmentObject equipObject in GetLinkEquipment.GetAllSpawnedEquipments)
        {
            equipObject.EndShowPreview();
        }

        //if (aiming || shootParameters.GetCurrentSalvo.GetProjPrefab.GetProjectileType == ProjectileType.CatapultBoulder)
        //{
        if (!isNotPlayerComp)
            GameManager.gameManager.SlwMoManager.StopAimSlowMo();
        //}
    }

    /// <summary>
    /// Commence à prévisualiser la compétence une fois lancée
    /// </summary>
    public override void StartLaunchedPreview()
    {
        foreach (EquipmentObject equipObject in GetLinkEquipment.GetAllSpawnedEquipments)
        {
            equipObject.StartLaunchedPreview();
        }

        //if (aiming || shootParameters.GetCurrentSalvo.GetProjPrefab.GetProjectileType == ProjectileType.CatapultBoulder)
        //{
        if(!isNotPlayerComp)
            GameManager.gameManager.SlwMoManager.StopAimSlowMo();
        //}
    }
    #endregion

    #region Competence Special Effects
    public ProjectileSpecialParameters GenerateProjectileSpecialParameters
    {
        get
        {
            return new ProjectileSpecialParameters(
                new ShipSpeedModifier(speedModifier),
                new ProjectilePiercingParameters(piercingParameters), 
                new ProjectileSkeweringParameters(skeweringParameters), 
                explosionParameters, 
                new SmokeZoneParameters(smokeZoneParameters),
                new SlowingZoneParameters(slowingZoneParameters)
                );
        }
    }

    [Header("Special Effects")]
    [SerializeField] ShipSpeedModifier speedModifier;
    [SerializeField] ProjectilePiercingParameters piercingParameters;
    [SerializeField] ProjectileSkeweringParameters skeweringParameters;
    [SerializeField] ExplosionParameters explosionParameters;
    [SerializeField] SmokeZoneParameters smokeZoneParameters;
    [SerializeField] SlowingZoneParameters slowingZoneParameters;
    #endregion

    public ProjectilePoolTag GetUsedProjectilePoolTag
    {
        get
        {
            return shootParameters.GetCurrentSalvo.GetProjectileType;
        }
    }
}
