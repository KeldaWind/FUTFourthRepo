using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Une entité immobile capable de repérer le joueur, le viser et lui tirer dessus.
/// </summary>
public class Turret : MonoBehaviour
{
    /// <summary>
    /// Les origines de tir reliées à cette tourelle.
    /// </summary>
    [Header("Shoot Parameters")]
    [SerializeField] ShootOrigin[] shootOrigins;
    /// <summary>
    /// Le tir effectué par cette tourelle
    /// </summary>
    [SerializeField] ShootParameters shootParameters;
    /// <summary>
    /// Le temps entre deux tirs de cette tourelle
    /// </summary>
    [SerializeField] float cooldownTime;
    /// <summary>
    /// Variable permettant de faire tourner le cooldown
    /// </summary>
    float currentCooldownTime;

    [SerializeField] float shootImprecision;

    [Header("Attack Preparation")]
    [SerializeField] float attackPreparationTime = 0.5f;
    float currentAttackPreparationTime;

    [SerializeField] float rotationCoeffWhilePreparing;

    [SerializeField] GameObject preparationFeedbackObject;

    /// <summary>
    /// Le prefab de la zone de détection
    /// </summary>
    [Header("Detection")]
    [SerializeField] DetectionZone detectionZonePrefab;
    /// <summary>
    /// La distance maximale de détection du joueur par cette tourelle
    /// </summary>
    [SerializeField] float detectionRange;
    /// <summary>
    /// La zone de détection instantiée lorsque la tourelle est initialisée
    /// </summary>
     DetectionZone relatedDetectionZone;

    #region Projectiles Parameters
    bool isBoulder;
    float canonProjectileSpeed;
    AnimationCurve catapultProjectileDurationDependingOnDistance;
    Projectile projectilePrefab;
    #endregion

    private void Start()
    {
        SetUp();
    }

    private void Update()
    {
        if (relatedDetectionZone.GetTargetShip != null)
        {
            if (Reloading)
                UpdateShootCooldown();
            else if (PreparingAttack)
                UpdateAttackPreparation();

            UpdateCurrentTargetDirection(relatedDetectionZone.GetTargetShip);
        }

        lifeManager.UpdateLifeManagement();
    }

    /// <summary>
    /// Initialisation de cette tourelle
    /// </summary>
    public void SetUp()
    {
        normalAimRotation = transform.localRotation.eulerAngles;

        foreach (ShootOrigin origin in shootOrigins)
        {
            origin.SetUp(transform, this);
        }

        relatedDetectionZone = Instantiate(detectionZonePrefab, transform);
        relatedDetectionZone.SetUp(detectionRange);

        currentCooldownTime = cooldownTime;

        lifeManager.SetUp(turretHitbox);
        turretHitbox.SetUp(this);

        if (preparationFeedbackObject != null)
            preparationFeedbackObject.SetActive(false);

        #region ProjectileParameters
        Projectile shootProjectilePrefab = GameManager.gameManager.PoolManager.GetProjectile(shootParameters.GetCurrentSalvo.GetProjectileType, PoolInteractionType.PeekFromPool);

        isBoulder = shootProjectilePrefab.IsBoulder;
        projectilePrefab = GameManager.gameManager.PoolManager.GetProjectile(shootParameters.GetCurrentSalvo.GetProjectileType, PoolInteractionType.PeekPrefab);
        canonProjectileSpeed = shootParameters.GetCurrentSalvo.GetProjectileParameters.GetEvolutiveSpeed();

        if (isBoulder)
            catapultProjectileDurationDependingOnDistance = (projectilePrefab as ProjectileBoulder).GetLifeTimeWithDistanceCurve;
        #endregion
    }

    #region Shoot
    /// <summary>
    /// Commence un tir avec les paramètres de cette tourelle
    /// </summary>
    public void Shoot()
    {
        foreach (ShootOrigin origin in shootOrigins)
        {
            if (isBoulder)
            {
                origin.StartShooting(shootParameters, transform.position + currentAimDir * Vector3.Distance(currentAimPos, transform.position) + new Vector3(Random.Range(-shootImprecision, shootImprecision), 0, Random.Range(-shootImprecision, shootImprecision)), false);
                origin.ShowPreparePreview(shootParameters.GetCurrentSalvo);
                origin.StartLaunchedPreview();
            }
            else
                origin.StartShooting(shootParameters, currentAimDir, false);
        }
    }
    #endregion

    #region Cooldown and Preparation
    public bool Reloading
    {
        get
        {
            return currentCooldownTime != 0;
        }
    }

    public bool PreparingAttack
    {
        get
        {
            return currentAttackPreparationTime != 0;
        }
    }

    public void UpdateShootCooldown()
    {
        if (currentCooldownTime > 0)
            currentCooldownTime -= Time.deltaTime;
        else
        {
            currentCooldownTime = 0;
            currentAttackPreparationTime = attackPreparationTime;

            if (preparationFeedbackObject != null)
                preparationFeedbackObject.SetActive(true);
        }
    }

    public void UpdateAttackPreparation()
    {
        if (currentAttackPreparationTime > 0)
            currentAttackPreparationTime -= Time.deltaTime;
        else
        {
            Shoot();
            currentAttackPreparationTime = 0;
            currentCooldownTime = cooldownTime;

            if (preparationFeedbackObject != null)
                preparationFeedbackObject.SetActive(false);
        }
    }
    #endregion

    #region Aiming
    /// <summary>
    /// La vitesse de rotation à laquelle cette tourelle va viser le joueur
    /// </summary>
    [Header("Aiming")]
    [SerializeField] float rotationSpeed;
    /// <summary>
    /// Nombre de fois où la tourelle recalcule la position du joueur. Généralement, à partir de 4 ou 5, la position précise du joueur est calculée
    /// </summary>
    [SerializeField] int precision;
    /// <summary>
    /// La rotation de base de cette tourelle (euler)
    /// </summary>
    Vector3 normalAimRotation;
    /// <summary>
    /// La direction actuellement visée par le bateau
    /// </summary>
    Vector3 currentAimDir;
    /// <summary>
    /// La rotation actuelle du bateau (en y)
    /// </summary>
    float currentRotation;

    /// <summary>
    /// Position actuellement visée
    /// </summary>
    Vector3 currentAimPos;

    /// <summary>
    /// On actualise la direction actuellement visée par la tourelle (et donc la rotation actuelle)
    /// </summary>
    /// <param name="target">Joueur visé</param>
    public void UpdateCurrentTargetDirection(PlayerShip target)
    {
        if (Time.timeScale == 0)
            return;

        Vector3 targetAimPos = Aim(target.transform.position, target.GetShipSpeed, target.GetShipVelocity.normalized);

        currentAimPos = targetAimPos;

        Vector3 targetAimDir = transform.position - targetAimPos;
        targetAimDir.y = 0;
        targetAimDir.Normalize();

        if (Vector3.Distance(targetAimDir, currentAimDir) > 0.05f)
        {
            Vector3 rightVector = currentAimDir.GetRightOrthogonalVectorXZ();
            float modifier = PreparingAttack ? rotationCoeffWhilePreparing : 1;
            if (Vector3.Dot(rightVector, targetAimDir) > 0)
                currentRotation -= modifier * rotationSpeed * Time.deltaTime / Time.timeScale;
            else
                currentRotation += modifier * rotationSpeed * Time.deltaTime / Time.timeScale;

            currentAimDir = normalAimRotation + Quaternion.Euler(0, currentRotation, 0) * new Vector3(0, 0, 1);
        }

        TurnTurretTowardVector(currentAimDir);
    }

    /// <summary>
    /// Permet d'obtenir la position du joueur au moment où le projectile va frapper, afin de pouvoir anticiper sa position
    /// </summary>
    /// <param name="targetBasePosition">Position de base de la cible</param>
    /// <param name="targetSpeed">Vitesse de déplacement de la cible</param>
    /// <param name="targetDirection">Direction de déplacement actuel de la cible</param>
    /// <returns>Position qu'aura le joueur au moment où le projectile va frapper</returns>
    public Vector3 Aim(Vector3 targetBasePosition, float targetSpeed, Vector3 targetDirection)
    {
        targetSpeed *= 5 / 5.40f;
        targetBasePosition.y = transform.position.y;
        Vector3 targetEvolutivePosition = targetBasePosition;
        float timeToGetToTarget;

        for (int i = 0; i < precision; i++)
        {
            if (isBoulder)
            {
                timeToGetToTarget = catapultProjectileDurationDependingOnDistance.Evaluate(Vector3.Distance(transform.position, targetBasePosition));
                targetEvolutivePosition = targetBasePosition + targetDirection.normalized * timeToGetToTarget * targetSpeed;
                break;
            }
            else
            {
                timeToGetToTarget = Vector3.Distance(targetEvolutivePosition, transform.position) / canonProjectileSpeed;
                targetEvolutivePosition = targetBasePosition + targetDirection.normalized * timeToGetToTarget * targetSpeed;
            }
        }

        return targetEvolutivePosition;
    }

    /// <summary>
    /// Tourne la tourelle en fonction de sa direction de visée actuelle
    /// </summary>
    /// <param name="aimDir">Direction de visée actuelle</param>
    public void TurnTurretTowardVector(Vector3 aimDir)
    {
        float rotY = Mathf.Atan2(aimDir.x, aimDir.z) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(normalAimRotation + new Vector3(0, rotY, 0));
    }
    #endregion

    #region Hitbox and Life
    [Header("Hitbox and Life")]
    [SerializeField] TurretHitbox turretHitbox;
    [SerializeField] LifeManager lifeManager;
    public LifeManager LfManager
    {
        get
        {
            return lifeManager;
        }
    }
    [SerializeField] AttackTag hitboxTag;
    public AttackTag HitboxTag
    {
        get
        {
            return hitboxTag;
        }
    }

    public IDamageReceiver GetTurretDamageReceiver
    {
        get
        {
            return turretHitbox;
        }
    }

    public void Die()
    {
        //turretHitbox.Die();
        Destroy(gameObject);
    }
    #endregion
}
