using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objet physique correspondant à un équipement de tir du joueur
/// </summary>
public class EquipmentObjectShoot : EquipmentObject
{
    /// <summary>
    /// Origines de tir utilisées lorsqu'un tir est effectué sur cet objet
    /// </summary>
    [SerializeField] ShootOrigin[] shootOrigins;
    bool showPreview;

    /// <summary>
    /// Initialisation de l'équipement
    /// </summary>
    /// <param name="equipmentParents">Parent de cet équipement/param>
    public override void SetUp(Transform equipmentParents, Ship ship)
    {
        base.SetUp(equipmentParents, ship);

        foreach(ShootOrigin shootOrigin in shootOrigins)
        {
            shootOrigin.SetUp(equipmentParents, ship);
        }
    }

    /// <summary>
    /// Commence un tir sur cet équipement
    /// </summary>
    /// <param name="shootParameters">Paramètres du tir</param>
    /// <param name="aimPos">Position visée</param>
    public void Shoot(ShootParameters shootParameters, Vector3 aimPos, bool lockPosition, ProjectileSpecialParameters projectileSpecialParameters)
    {
        foreach(ShootOrigin shootOrigin in shootOrigins)
        {
            shootOrigin.SetProjectileSpecialParameters(projectileSpecialParameters);
            shootOrigin.StartShooting(shootParameters, aimPos, lockPosition);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input">Entre -1 et 1, correspond au joystick du canon</param>
    /// <param name="angleAmplitude">Amplitude de visée maximum</param>
    public void UpdateShootOriginsModifiedDirection(float input, float angleAmplitude)
    {
        float minAngle = -angleAmplitude / 2;
        float maxAngle = angleAmplitude / 2;

        float coeff = (input + 1) / 2;
        float angle = Mathf.Lerp(minAngle, maxAngle, coeff);

        foreach (ShootOrigin origin in shootOrigins)
        {
            origin.UpdateModifiedDirection(angle);
        }
    }

    #region Aiming
    /// <summary>
    /// Actualise la visée de cet objet lorsqu'un tir est en cours de préparation
    /// </summary>
    /// <param name="aimPosition">Position visée par le doigt</param>
    /// <param name="aiming">Ce tir vise précisément la position du doigt</param>
    public void UpdateAiming(Vector3 aimPosition, bool aiming)
    {
        if (CheckIfBeingUsed())
            return;

        foreach (ShootOrigin shootOrigin in shootOrigins)
        {
            shootOrigin.UpdateShootingAim(aimPosition, aiming);
        }
    }
    #endregion

    #region Being Used
    /// <summary>
    /// Vérifie si cet objet est actuellement en cours d'utilisation
    /// </summary>
    /// <returns>L'objet est en cours d'utilisation</returns>
    public override bool CheckIfBeingUsed()
    {
        bool beingUsed = false;

        foreach(ShootOrigin origin in shootOrigins)
        {
            if (origin.BeingUsed)
            {
                beingUsed = true;
                break;
            }
        }

        return beingUsed;
    }
    #endregion

    #region Preview
    /// <summary>
    /// Commence la prévisualisation de la préparation d'une compétence
    /// </summary>
    /// <param name="salvo">Salve à prévisualiser</param>
    public void StartShowPreview(Salvo salvo)
    {
        foreach (ShootOrigin shootOrigin in shootOrigins)
        {
            shootOrigin.ShowPreparePreview(salvo);
        }
    }

    /// <summary>
    /// Finit la prévisualisation de la préparation d'une compétence
    /// </summary>
    public override void EndShowPreview()
    {
        foreach (ShootOrigin shootOrigin in shootOrigins)
        {
            shootOrigin.HidePreparePreview();
        }
    }

    /// <summary>
    /// Commence la prévisualisation du lancement d'une compétence
    /// </summary>
    public override void StartLaunchedPreview()
    {
        foreach (ShootOrigin shootOrigin in shootOrigins)
        {
            shootOrigin.StartLaunchedPreview();
        }
    }

    /*public void StartLaunchedPreview(Salvo salvo)
    {
        foreach (ShootOrigin shootOrigin in shootOrigins)
        {
            shootOrigin.StartLaunchedPreview(salvo);
        }
    }*/
    #endregion 
}
