using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootOrigin : MonoBehaviour
{
    /// <summary>
    /// Parent de ce shoot origin, faisant dépendre la direction visée 
    /// </summary>
    Transform directionParent;
    /// <summary>
    /// Vrai si ce shoot origin vise précisément la position indiquée par le doigt du joueur
    /// </summary>
    bool aimTouchPosition;
    /// <summary>
    /// Position indiquée par le doigt du joueur
    /// </summary>
    Vector3 shootAimPosition;

    Vector3 relativeShootAimPosition;

    /// <summary>
    /// Direction de tir de base de ce shoot origin
    /// </summary>
    [Header("Direction")]
    [SerializeField] Vector3 shootDirection;
    float modifiedAngle;

    public void UpdateModifiedDirection(float modificationAngle)
    {
        modifiedAngle = modificationAngle;
    }

    /// <summary>
    /// Renvoie la vraie direction actuellement visée par le shoot origin, en fonction de la rotation du parent et/ou de si ce shoot otigin vise la position précise du doigt
    /// </summary>
    public Vector3 GetTrueShootDirection
    {
        get
        {
            if (aimTouchPosition)
            {
                if (lockDirectionAfterStartShoot)
                {
                    return (Quaternion.Euler(/*directionParent.localRotation.eulerAngles*/directionParent.rotation.eulerAngles - rotationWhenShot.eulerAngles)) * lockedDirection;
                }
                else
                    return (shootAimPosition - transform.position).normalized;
            }
            else
            {
                Vector3 trueDir = (Quaternion.Euler(0, modifiedAngle, 0) * (/*directionParent.localRotation*/directionParent.rotation * shootDirection).normalized).normalized;
                trueDir.y = 0;
                return trueDir.normalized;
            }
        }
    }

    /// <summary>
    /// Paramètres de tir actuels de ce shoot origin
    /// </summary>
    ShootParameters currentShootParameters;
    /// <summary>
    /// Temps restant avant de passer à la salve suivant
    /// </summary>
    float currentTimeBeforeNextSalvo;

    /// <summary>
    /// Permet de récupérer toutes les directions de tirs dans le cas d'un tir direct
    /// </summary>
    /// <param name="salvo">Salve tirée</param>
    /// <returns>Toutes les directions dans lesquelles tirer</returns>
    public List<Vector3> GetAllShootDirections(Salvo salvo)
    {
        List<Vector3> allShootDirections = new List<Vector3>();

        float minAngleModificator = -salvo.GetAngleBetweenDirections / 2 * (salvo.GetNumberOfDirections - 1);
        for (int i = 0; i < salvo.GetNumberOfDirections; i++)
        {
            float modificationAngle = minAngleModificator + (salvo.GetAngleBetweenDirections) * i;
            allShootDirections.Add(Quaternion.Euler(0, modificationAngle, 0) * GetTrueShootDirection);
        }

        return allShootDirections;
    }

    /// <summary>
    /// Permet de récupérer toutes les positions de tirs dans le cas d'un tir en cloche
    /// </summary>
    /// <param name="salvo">Salve tirée</param>
    /// <returns>Toutes les positions sur lesquelles tirer</returns>
    public List<Vector3> GetAllShootPositions(Salvo salvo)
    {
        List<Vector3> allShootPositions = new List<Vector3>();

        float minAngleModificator = -salvo.GetAngleBetweenDirections / 2 * (salvo.GetNumberOfDirections - 1);
        for (int i = 0; i < salvo.GetNumberOfDirections; i++)
        {
            float modificationAngle = minAngleModificator + (salvo.GetAngleBetweenDirections) * i;
            Vector3 relativePos = lockDirectionAfterStartShoot ? relativeShootAimPosition : shootAimPosition - transform.position;

            Vector3 goodPosition = transform.position + Quaternion.Euler(0, modificationAngle, 0) * relativePos;
            goodPosition.y = GameManager.gameManager.GetSeaLevel;

            allShootPositions.Add(goodPosition);
        }

        return allShootPositions;
    }

    public List<float> GetAllDistances(Salvo salvo)
    {
        List<float> allDistances = new List<float>();

        foreach(Vector3 position in GetAllShootPositions(salvo))
        {
            allDistances.Add(Vector3.Distance(transform.position, position));
        }

        return allDistances;
    }

    /// <summary>
    /// Toutes les positions visées par le tir en cloche qui vient d'être effectué
    /// </summary>
    List<List<Vector3>> shootedPositions;

    /// <summary>
    /// Toutes les preview actuellement instantiées pour prévisualiser le tir
    /// </summary>
    [Header("Preview")]
    List<ShootPreview> spawnedPreviews;

    /// <summary>
    /// Vrai si la preview doit être affichée
    /// </summary>
    bool showPreview;
    /// <summary>
    /// Salve en train d'être prévisualisée
    /// </summary>
    Salvo previewingSalvo;

    /// <summary>
    /// Vrai si la direction visée doit être bloquée au moment du tir
    /// </summary>
    bool lockDirectionAfterStartShoot;
    /// <summary>
    /// La direction bloquée si le tir est de type bloqué
    /// </summary>
    Vector3 lockedDirection;
    /// <summary>
    /// The rotation of the origin when the ship shot
    /// </summary>
    Quaternion rotationWhenShot;


    /// <summary>
    /// Parametres de tis test
    /// </summary>
    [Header("Tests")]
    [SerializeField] ShootParameters testShootParameters;
    /// <summary>
    /// Ce shoot origin est un test
    /// </summary>
    [SerializeField] bool test;

    Ship relatedShip;
    Turret relatedTurret;
    /// <summary>
    /// Initialisation de ce shoot origin
    /// </summary>
    /// <param name="dirParent">Le parent de direction de ce shoot orign</param>
    public void SetUp(Transform dirParent, Ship ship)
    {
        directionParent = dirParent;
        relatedShip = ship;
    }

    public void SetUp(Transform dirParent, Turret turret)
    {
        directionParent = dirParent;
        relatedTurret = turret;
    }

    private void Start()
    {
        shootDirection.Normalize();
    }

    private void Update()
    {
        if(!currentShootParameters.Over)
            UpdateShooting();

        if ((Input.touchCount > 1 || Input.GetMouseButton(0)) && currentShootParameters.Over && test)
        {
            StartShooting(testShootParameters, Vector3.zero, false);
        }

        if (showPreview)
        {
            UpdatePreparePreview(previewingSalvo);
        }

        if (waitingTimeToRelaunchShootEffect > 0)
            waitingTimeToRelaunchShootEffect -= Time.deltaTime;
        else if (waitingTimeToRelaunchShootEffect < 0)
        {
            shootParticleSystem.Play();
            waitingTimeToRelaunchShootEffect = 0;
        }
    }

    #region Shooting
    /// <summary>
    /// Entame un tir sur ce shoot origin
    /// </summary>
    /// <param name="shootParameters">Paramètres du tir</param>
    /// <param name="shAimPosition">Position visée par le tir</param>
    public void StartShooting(ShootParameters shootParameters, Vector3 shAimPosition, bool lockDirection)
    {
        currentShootParameters = shootParameters;
        shootAimPosition = shAimPosition;

        Shoot(currentShootParameters.GetCurrentSalvo);

        lockDirectionAfterStartShoot = lockDirection;

        if (lockDirectionAfterStartShoot)
        {
            lockedDirection = (shootAimPosition - transform.position).normalized;
            relativeShootAimPosition = shootAimPosition - transform.position;
            rotationWhenShot = directionParent.rotation;
        }
    }

    /// <summary>
    /// Actualise le tir en cours
    /// </summary>
    public void UpdateShooting()
    {
        if (currentTimeBeforeNextSalvo > 0)
            currentTimeBeforeNextSalvo -= Time.deltaTime;
        else if (currentTimeBeforeNextSalvo < 0)
        {
            if (!currentShootParameters.Over)
            {
                Shoot(currentShootParameters.GetCurrentSalvo);
            }
            else
            {
                currentTimeBeforeNextSalvo = 0;
            }
        }
    }

    /// <summary>
    /// Lance une salve
    /// </summary>
    /// <param name="salvo">Salve à lancer</param>
    public void Shoot(Salvo salvo)
    {
        currentTimeBeforeNextSalvo = currentShootParameters.GetTimeBewteenSalvos;
        currentShootParameters.IncreaseSalvoIndex();

        shootedPositions = new List<List<Vector3>>();

        #region Calculate Directions
        PoolingManager poolManager = GameManager.gameManager.PoolManager;
        Projectile shootProjectilePrefab = poolManager.GetProjectile(salvo.GetProjectileType , PoolInteractionType.PeekFromPool);
        bool isBoulder = shootProjectilePrefab.IsBoulder;

        List<Vector3> allShootDirections = isBoulder ? GetAllShootPositions(salvo) : GetAllShootDirections(salvo);
        float projectilesSpacing = salvo.GetProjectilesSpacing;
        foreach (Vector3 direction in allShootDirections)
        {
            List<Vector3> thisShootedPos = new List<Vector3>();
            /**/List<Vector3> allPossiblePositions = CirclePositionsGenerator.GetAllPositionsInCircle(salvo.GetProjectileParameters.GetCurrentProjectileSize, projectilesSpacing, salvo.GetImprecisionParameter);
            for (int i = 0; i < salvo.GetNumberOfProjectiles; i++)
            {
                Projectile shootProjectile = poolManager.GetProjectile(salvo.GetProjectileType, PoolInteractionType.GetFromPool);
                shootProjectile.transform.position = transform.position;
                shootProjectile.transform.rotation = Quaternion.identity;

                if (relatedShip != null)
                    shootProjectile.SetSource(relatedShip);
                else if (relatedTurret != null)
                    shootProjectile.SetSource(relatedTurret);

                if (isBoulder)
                {
                    //List<Vector3> allPossiblePositions

                    //Vector3 modifiedPosition = direction + new Vector3(Random.Range(-salvo.GetImprecisionParameter, salvo.GetImprecisionParameter), 0, Random.Range(-salvo.GetImprecisionParameter, salvo.GetImprecisionParameter));
                    /**/
                    int randomIndex = Random.Range(0, allPossiblePositions.Count);
                    Vector3 modifiedPosition = direction + allPossiblePositions[randomIndex] + new Vector3(Random.Range(-projectilesSpacing/2, projectilesSpacing/2), 0, Random.Range(-projectilesSpacing/2, projectilesSpacing/2));
                    allPossiblePositions.RemoveAt(randomIndex);
                    /**/
                    shootProjectile.ShootProjectile(salvo.GetProjectileParameters, transform.position, modifiedPosition);

                    thisShootedPos.Add(modifiedPosition);
                }
                else
                {
                    Vector3 modifiedDirection = Quaternion.Euler(0, Random.Range(-salvo.GetImprecisionParameter, salvo.GetImprecisionParameter), 0) * direction;
                    shootProjectile.ShootProjectile(salvo.GetProjectileParameters, modifiedDirection, GameManager.gameManager.Player.GetShipVelocity);
                }

                shootProjectile.SetProjectileTag(currentShootParameters.GetProjectileTag);

                #region Special Parameters
                if(projectileSpecialParameters != null)
                {
                    shootProjectile.SetProjectileSpecialParameters(
                        new ProjectileSpecialParameters(
                            new ShipSpeedModifier(projectileSpecialParameters.GetSpeedModifier), 
                            new ProjectilePiercingParameters(projectileSpecialParameters.GetPiercingParameters),
                            new ProjectileSkeweringParameters(projectileSpecialParameters.GetSkeweringParameters), 
                            projectileSpecialParameters.GetExplosionParameters, 
                            new SmokeZoneParameters(projectileSpecialParameters.GetSmokeZoneParameters), 
                            new SlowingZoneParameters(projectileSpecialParameters.GetSlowingZoneParameters)
                            ));
                    shootProjectile.GetProjectileSpecialParameters.GetSkeweringParameters.SetSourceProjectile(shootProjectile);
                }
                #endregion

                if(allPossiblePositions.Count == 0 && isBoulder)
                {
                    Debug.LogWarning("couldn't shoot all boulders");
                    break;
                }
            }
            shootedPositions.Add(thisShootedPos);
        }
        #endregion

        #region Feedback
        if (shootParticleSystem != null)
        {
            //ParticleSystem.EmitParams parameters = shootParticleSystem.emission.;
            //if (shootParticleSystem.isPlaying)
            //{
            //Debug.Log("ui");
            //shootParticleSystem.Stop();
            //shootParticleSystem.Play();
            //shootParticleSystem.Emit(8);
            //waitingTimeToRelaunchShootEffect = 0.05f;
            /*}
            else
                shootParticleSystem.Play();*/
            shootParticleSystem.Emit(2);
        }

        if (currentShootParameters.GetProjectileTag == AttackTag.Player)
        {
            Vibration.Vibrate(shootVibrationDuration);
            GameManager.gameManager.ScrshkManager.StartScreenshake(shootShakeParameters);
        }

        if (shootAudioSource != null)
            shootAudioSource.PlaySound(currentShootParameters.GetShootSound);
        #endregion

        if (currentShootParameters.GetCurrentSalvoIndex > 1)
            ContinueLaunchedPreview(salvo);
    }
    #endregion

    #region Aiming
    /// <summary>
    /// Actualise la position visée
    /// </summary>
    /// <param name="aimPosition">La position que doit viser le shoot origin</param>
    /// <param name="aiming">Le shoot origin doit viser la position précise</param>
    public void UpdateShootingAim(Vector3 aimPosition, bool aiming)
    {
        shootAimPosition = aimPosition;
        aimTouchPosition = aiming;
    }
    #endregion

    /// <summary>
    /// Ce shoot oringin est actuellement utilisé (en train d'effectuer un tir)
    /// </summary>
    public bool BeingUsed
    {
        get
        {
            return !currentShootParameters.Over;
        }
    }

    #region Preview
    /// <summary>
    /// Commence la prévisualisation de préparation d'une salve
    /// </summary>
    /// <param name="salvo">Salve à prévisualiser</param>
    public void ShowPreparePreview(Salvo salvo)
    {
        if(!BeingUsed)
            lockDirectionAfterStartShoot = false;

        previewingSalvo = salvo;

        if (spawnedPreviews == null)
            spawnedPreviews = new List<ShootPreview>();

        Projectile previewingProjectile = GameManager.gameManager.PoolManager.GetProjectile(salvo.GetProjectileType, PoolInteractionType.PeekPrefab);
        bool isBoulder = previewingProjectile.IsBoulder;

        List<Vector3> allShootDirections = isBoulder ? GetAllShootPositions(salvo) : GetAllShootDirections(salvo);

        foreach (Vector3 vector in allShootDirections)
        {
            /*Instantiate(previewingProjectile.GetShootPreviewPrefab)*/
            ShootPreview preview = GameManager.gameManager.PoolManager.GetPreview(previewingProjectile.GetShootPreviewType, PoolInteractionType.GetFromPool) as ShootPreview;
            preview.ShowPreparePreview(salvo, currentShootParameters.GetProjectileTag);

            spawnedPreviews.Add(preview);
        }

        showPreview = true;
    }

    /// <summary>
    /// Finit la prévisualisation de préparation d'une salve
    /// </summary>
    public void HidePreparePreview()
    {
        if (spawnedPreviews == null)
            return;

        foreach (Preview preview in spawnedPreviews)
        {
            preview.HidePreparePreview();
        }

        spawnedPreviews = new List<ShootPreview>();

        showPreview = false;
    }

    /// <summary>
    /// Actualise la prévisualisation de préparation d'une salve
    /// </summary>
    /// <param name="salvo">Salve à prévisualiser</param>
    public void UpdatePreparePreview(Salvo salvo)
    {
        Projectile previewingProjectile = GameManager.gameManager.PoolManager.GetProjectile(salvo.GetProjectileType, PoolInteractionType.PeekPrefab);

        for (int i = 0; i < spawnedPreviews.Count; i++)
        {
            Vector3 position = new Vector3();
            Vector3 direction = new Vector3();

            position = transform.position;

            if (previewingProjectile as ProjectileBoulder != null)
                direction = GetAllShootPositions(salvo)[i];
            else
                direction = GetAllShootDirections(salvo)[i];

            spawnedPreviews[i].UpdatePreparePreview(position, direction);
        }
    }

    /// <summary>
    /// Commence la prévisualisation de lancement d'une salve
    /// </summary>
    public void StartLaunchedPreview()
    {
        List<float> allDistances = GetAllDistances(previewingSalvo);

        if (spawnedPreviews == null)
            return;

        Projectile previewingProjectile = GameManager.gameManager.PoolManager.GetProjectile(previewingSalvo.GetProjectileType, PoolInteractionType.PeekPrefab);

        for (int i = 0; i < spawnedPreviews.Count; i++)
        {
            Preview preview = spawnedPreviews[i];

            float parameter = 0;

            if (previewingProjectile as ProjectileBoulder != null)
                parameter = (previewingProjectile as ProjectileBoulder).GetLifeTimeWithDistance(allDistances[i]);

            preview.StartLaunchedPreview(transform.position, shootedPositions[i], parameter);
        }

        spawnedPreviews = new List<ShootPreview>();
    }

    public void ContinueLaunchedPreview(Salvo salvo)
    {
        if (spawnedPreviews == null)
            return;

        Projectile previewingProjectile = GameManager.gameManager.PoolManager.GetProjectile(salvo.GetProjectileType, PoolInteractionType.PeekPrefab);

        List<float> allDistances = GetAllDistances(previewingSalvo);

        bool isBoulder = previewingProjectile.IsBoulder;
        List<Vector3> allShootDirections = isBoulder ? GetAllShootPositions(salvo) : GetAllShootDirections(salvo);

        foreach (Vector3 vector in allShootDirections)
        {
            //ShootPreview preview = Instantiate(previewingProjectile.GetShootPreviewPrefab);
            ShootPreview preview = GameManager.gameManager.PoolManager.GetPreview(previewingProjectile.GetShootPreviewType, PoolInteractionType.GetFromPool) as ShootPreview;
            preview.ShowPreparePreview(salvo, currentShootParameters.GetProjectileTag);

            spawnedPreviews.Add(preview);
        }

        if (spawnedPreviews == null)
            return;

        for (int i = 0; i < spawnedPreviews.Count; i++)
        {
            Preview preview = spawnedPreviews[i];

            float parameter = 0;

            if (previewingProjectile as ProjectileBoulder != null)
                parameter = (previewingProjectile as ProjectileBoulder).GetLifeTimeWithDistance(allDistances[i]);

            preview.StartLaunchedPreview(transform.position, shootedPositions[i], parameter);
        }

        spawnedPreviews = new List<ShootPreview>();
    }
    #endregion

    #region Special Parameters
    ProjectileSpecialParameters projectileSpecialParameters;
    public void SetProjectileSpecialParameters(ProjectileSpecialParameters projSpecialParameters)
    {
        projectileSpecialParameters = projSpecialParameters;
    }
    #endregion

    #region Feedbacks
    [Header("Feedbacks")]
    [SerializeField] ParticleSystem shootParticleSystem;
    [SerializeField] AudioSource shootAudioSource;
    float waitingTimeToRelaunchShootEffect;
    long shootVibrationDuration = 40;
    ScreenshakeParameters shootShakeParameters = new ScreenshakeParameters(1.5f, 0.05f, 0.5f, ScreenshakeDirection.XYZ);
    #endregion
}
