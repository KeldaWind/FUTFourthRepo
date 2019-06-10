using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Projectile pouvant être tiré et ayant une trajectoire en cloche
/// </summary>
public class ProjectileBoulder : Projectile
{
    /// <summary>
    /// Trajectoire du boulet en y
    /// </summary>
    [SerializeField] AnimationCurve yPosWithTime;
    /// <summary>
    /// Trajectoire du boulet en y
    /// </summary>
    public AnimationCurve GetYPosCurve
    {
        get
        {
            return yPosWithTime;
        }
    }

    /// <summary>
    /// Durée de vie du projectile en fonction de la distance qu'il va parcourir horizontalement
    /// </summary>
    [SerializeField] AnimationCurve lifeTimeCurveDependingOnDistance;
    /// <summary>
    /// Définit pendant à partir de quel moment le projectile inflige des dégats (par exemple, 0.1f signifie que le projectile va infliger des dégats uniquement sur les 10 derniers % de sa trajectoire)
    /// </summary>
    [SerializeField] float damageDurationCoeff = 0.1f;

    /// <summary>
    /// Position à laquelle le boulet a été tiré
    /// </summary>
    Vector3 boulderStartPosition;
    /// <summary>
    /// Position visée par le boulet
    /// </summary>
    Vector3 boulderEndPosition;

    /// <summary>
    /// Tire le projectile en lui appliquant différents paramètres
    /// </summary>
    /// <param name="newParam">Paramètres de tirs appliqués à ce projectile</param>
    /// <param name="startPosition">Position depuis laquelle ce projectile sera tiré</param>
    /// <param name="endPosition">Position visée par ce projectile</param>
    public override void ShootProjectile(ProjectileParameters newParam, Vector3 startPosition, Vector3 endPosition)
    {
        gameObject.SetActive(true);

        projectileParameters = newParam;
        boulderStartPosition = startPosition;
        boulderEndPosition = endPosition;

        projectileParameters.SetUpParameters();
        projectileBody.velocity = Vector3.zero;
        projectileSizeParent.localScale = Vector3.one * projectileParameters.GetCurrentProjectileSize;
        lifetimeEnded = false;

        projectileParameters.SetLifeTime(GetLifeTimeWithDistance(Vector3.Distance(startPosition, endPosition)));

        relatedHitbox.SetUp(this);
        relatedHitbox.gameObject.SetActive(false);

        projectileRenderer.gameObject.SetActive(true);

        projectileFunctionEnded = false;
        projectileReturned = false;

        ResetAllSpecialEffects();
        SetUpAirRotation();
    }

    public override void Update()
    {
        base.Update();

        if (projectileFunctionEnded)
            return;

        if (!persistingPlaced)
        {
            Vector3 boulderPos = Vector3.Lerp(boulderStartPosition, boulderEndPosition, projectileParameters.LifetimeProgression);
            boulderPos.y = yPosWithTime.Evaluate(projectileParameters.LifetimeProgression) + Mathf.Lerp(boulderStartPosition.y, boulderEndPosition.y, projectileParameters.LifetimeProgression);

            transform.position = boulderPos;
        }

        if (1 - projectileParameters.LifetimeProgression < damageDurationCoeff && !relatedHitbox.gameObject.activeInHierarchy)
            relatedHitbox.gameObject.SetActive(true);
    }

    /// <summary>
    /// Renvoie la durée de vie du projectile en fonction de la distance renseignée
    /// </summary>
    /// <param name="distance">Distance Horizontale que parcourera le projectile</param>
    /// <returns>Durée de vie du projectile en fonction de cette distance</returns>
    public float GetLifeTimeWithDistance(float distance)
    {
        return lifeTimeCurveDependingOnDistance.Evaluate(distance);
    }

    public AnimationCurve GetLifeTimeWithDistanceCurve
    {
        get
        {
            return lifeTimeCurveDependingOnDistance;
        }
    }

    /*[Header("Rendering")]
    [SerializeField] float rotationSpeed;
    float baseYRotation;
    float currentXRotation;

    public void SetUpSelfRotation()
    {
        Vector3 moveDirection = boulderEndPosition - boulderStartPosition;
        moveDirection.y = 0;
        moveDirection.Normalize();
        baseYRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        currentXRotation = 0;

        transform.rotation = Quaternion.Euler(new Vector3(currentXRotation, baseYRotation, 0));
    }

    public void UpdateSelfRotation()
    {
        currentXRotation += rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(new Vector3(currentXRotation, baseYRotation, 0));
    }*/

    public override Vector3 GetProjectileDirection
    {
        get
        {
            return boulderEndPosition - boulderStartPosition;
        }
    }

    public override float GetProjectileAfterLandingSpeed
    {
        get
        {
            return Vector3.Distance(boulderStartPosition, boulderEndPosition) / 5;
        }
    }
}
