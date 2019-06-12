using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : Ship
{
    [Header("Enemy Tag")]
    [SerializeField] EnemyShipPoolTag enemyPoolTag;
    public EnemyShipPoolTag GetEnemyPoolTag { get { return enemyPoolTag; } }
    RigidbodyConstraints normalBodyConstrains;

    bool firstSetUpDone;
    public void FirstSetUp()
    {
        if (shipBoxCollider == null)
            shipBoxCollider = GetComponent<BoxCollider>();

        /*normalBodyConstrains = shipBody.constraints;
        Debug.Log(normalBodyConstrains);*/

        firstSetUpDone = true;
        if (equipment != null)
        {
            equipmentCopy = ScriptableObject.Instantiate(equipment);
            equipmentCopy.InstantiateAllObjects(transform, this);

            competenceCopy = ScriptableObject.Instantiate(equipment.GetPrimaryComp);
            competenceCopy.LinkEquipment(equipmentCopy);

            if (equipment.GetEquipmentType != EquipmentType.Catapult)
            {
                switch (equipmentCopy.GetEquipmentDirection)
                {
                    case (EquipmentDirection.Front):
                        enemyDirectionType = EnemyType.Fronter;
                        break;

                    case (EquipmentDirection.Sides):
                        enemyDirectionType = EnemyType.Flanker;
                        break;

                    case (EquipmentDirection.Back):
                        enemyDirectionType = EnemyType.Backer;
                        break;
                }
            }
            else
            {
                enemyDirectionType = EnemyType.Catapulter;
                if (competenceCopy as CompetenceShoot != null)
                {
                    attackMinDistance = (competenceCopy as CompetenceShoot).shootParameters.GetCatapultMinDistance;
                    attackMaxDistance = (competenceCopy as CompetenceShoot).shootParameters.GetCatapultMaxDistance;
                }
            }

            if (competenceCopy as CompetenceShoot != null)
            {
                (competenceCopy as CompetenceShoot).SetIsNotPlayerComp();

                ProjectilePoolTag projTag = (competenceCopy as CompetenceShoot).shootParameters.GetCurrentSalvo.GetProjectileType;
                projectilePrefab = GameManager.gameManager.PoolManager.GetProjectile(projTag, PoolInteractionType.PeekPrefab);

                if (projectilePrefab != null)
                {
                    if (projectilePrefab as ProjectileBoulder != null)
                        projectileLifeTimeDependingOnDistance = (projectilePrefab as ProjectileBoulder).GetLifeTimeWithDistanceCurve;
                }
            }
        }

        if (relatedDetectionZone == null)
            relatedDetectionZone = Instantiate(detectionZonePrefab, transform);

        relatedDetectionZone.SetUp(detectionRange);

        baseMovementParameters = shipMovements.GetCurrentShipMovementParameters;
    }

    #region Overrided Methods

    #region Built In Methods
    public override void Start()
    {
        /*if (!firstSetUpDone)
            FirstSetUp();*/

        if (!externalySetedUp)
            SetUp(watchingRoundParameters.IsNull);
    }

    public override void Update()
    {
        if (!firstSetUpDone)
            FirstSetUp();

        if (alreadyDead)
        {
            if (CheckIfReadyToBeReturnedToPool())
                ReturnToPool();

            return;
        }

        base.Update();

        if(!IsStun && !BeingSkewered && !blind && !(stoppedBecausePlayerHasNoControl && targetShip != null))
            shipMovements.UpdateMovementValues((PreparingAttack ? maniabilityModifierWhilePreparing : 1 )* GetRotationCoeff());
        else
            shipMovements.UpdateMovementValues(0);

        UpdateEnemyBehavior();

        UpdateCooldown();

        #region CHEATS
        if (Input.touchCount > 3)
            if (Input.touches[3].phase == TouchPhase.Began)
                relatedShipHitbox.Die();
        #endregion
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (PreparingAttack)
            attackPreparationWarningAnim.transform.rotation = Quaternion.Euler(new Vector3(30, 0, 0));
    }
    #endregion
    bool externalySetedUp;
    public void ExternalSetUp(EnemyWatchingRoundParameters watchingParameters)
    {
        watchingRoundParameters = watchingParameters;
        SetUp(watchingRoundParameters.IsNull);
        externalySetedUp = true;
    }

    public override void SetUp(bool stopped)
    {
        base.SetUp(stopped);

        gameObject.SetActive(true);

        UpdateEnemyAcquiredDatas();

        /*if (equipment != null)
        {
            if (!spawnedLinkEquipment)
            {
                equipmentCopy = ScriptableObject.Instantiate(equipment);
                equipmentCopy.InstantiateAllObjects(transform, this);

                competenceCopy = ScriptableObject.Instantiate(equipment.GetPrimaryComp);
                competenceCopy.LinkEquipment(equipmentCopy);

                switch (equipmentCopy.GetEquipmentDirection)
                {
                    case (EquipmentDirection.Front):
                        enemyDirectionType = EnemyType.Fronter;
                        break;

                    case (EquipmentDirection.Sides):
                        enemyDirectionType = EnemyType.Flanker;
                        break;

                    case (EquipmentDirection.Back):
                        enemyDirectionType = EnemyType.Backer;
                        break;
                }

                spawnedLinkEquipment = true;
            }
        }*/

        lifeManager.SetUpLifeBar(enemyLifeBar);
        lifeManager.OnLifeChange += CheckForEnemyPhase;

        /*if (relatedDetectionZone == null)
            relatedDetectionZone = Instantiate(detectionZonePrefab, transform);

        relatedDetectionZone.SetUp(detectionRange);*/

        attackPreparationWarningAnim.gameObject.SetActive(false);

        if (!watchingRoundParameters.IsNull)
        {
            watchingRoundParameters.SetUp();
            lastPredictedPosition = watchingRoundParameters.GetAimedWatchingPosition;

            CheckForObstacleOnWatchRoundWay();
        }

        shipBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        if (shipBoxCollider != null)
            shipBoxCollider.enabled = true;
    }
    #endregion

    #region Global Behavior
    /// <summary>
    /// Temps entre chaque "rafraichissement" des données de l'ennemi (moment où il réagit par rapport à la position du joueur,...)
    /// </summary>
    [Header("Global Behavior")]
    [SerializeField] float reactionTime;
    protected float currentReactionTime;

    [Header("Routine Parameters")]
    [SerializeField] float timeBeforeExitRoutine;
    float remainingTimeBeforeExitRoutine;
    [SerializeField] float maxRoutineDirectionCoeff;

    /// <summary>
    /// La distance à laquelle le bateau s'écarte de la position ciblée à la base
    /// </summary>
    [Header("Non Routine Parameters")]
    [SerializeField] float timeBeforeBackToRoutine;
    float remainingTimeBeforeBackToRoutine;
    float outOfRoutineModificationCoeff;

    protected Vector3 lastPredictedPosition;
    Vector3 lastTargetShipPredictedPosition;
    Vector3 lastTargetShipOffsetForShootDuration;

    EnemyType enemyDirectionType;

    bool stoppedBecausePlayerHasNoControl;
    bool fleeBecausePlayerWon;
    public virtual void UpdateEnemyBehavior()
    {
        if (fleeBecausePlayerWon)
            return;

        #region Flee after Player win
        if (GameManager.gameManager.Won && !fleeBecausePlayerWon)
        {
            fleeBecausePlayerWon = true;
            if (targetShip != null)
            {
                Vector3 fleeingPosition = (targetShip.transform.position - transform.position);
                fleeingPosition.y = transform.position.y;
                lastPredictedPosition = -(fleeingPosition.normalized) * 10000;
            }

            if (OutOfRoutine)
                EndNonRoutineBehaviour();
        }
        #endregion

        if (!fleeBecausePlayerWon)
        {
            if (!GameManager.gameManager.CheckIfPlayerHasControl)
            {
                if (!stoppedBecausePlayerHasNoControl)
                {
                    stoppedBecausePlayerHasNoControl = true;
                    if (targetShip != null)
                        shipMovements.StopShip();
                }
            }
            else
            {
                if (stoppedBecausePlayerHasNoControl)
                {
                    stoppedBecausePlayerHasNoControl = false;
                    if (targetShip != null)
                        shipMovements.StartShip();
                }
            }
        }

        #region Acquiring Target
        if (targetShip == null)
        {
            if (!watchingRoundParameters.IsNull)
                UpdateWatchingRoundBehaviour();
            
            if (stoppedBecausePlayerHasNoControl)
                return;

            targetShip = relatedDetectionZone.GetTargetShip;

            if (targetShip == null)
                return;
            else
            {
                shipMovements.StartShip();
                UpdateEnemyAcquiredDatas();
            }
        }
        #endregion


        if (stoppedBecausePlayerHasNoControl)
            return;

        if (IsStun || BeingSkewered || blind)
            return;

        if (currentReactionTime > 0)
            currentReactionTime -= Time.deltaTime;
        else if (currentReactionTime < 0)
            UpdateEnemyAcquiredDatas();

        #region Attack
        switch (enemyDirectionType)
        {
            case (EnemyType.Fronter):
                if (Vector3.Distance(transform.position, lastPredictedPosition) < (OutOfRoutine ? attackMaxDistance * 2 : attackMaxDistance) && currentCompetenceCooldown == 0 && AttackAngleIsAcceptable && targetShip != null && !PreparingAttack)
                    StartAttackPreparation();
                break;

            case (EnemyType.Flanker):
                if (!OutOfRoutine)
                {
                    if (Vector3.Distance(transform.position, lastPredictedPosition) < (OutOfRoutine ? attackMaxDistance * 2 : attackMaxDistance) && currentCompetenceCooldown == 0 && AttackAngleIsAcceptable && targetShip != null && !PreparingAttack)
                        StartAttackPreparation();
                }
                break;

            case (EnemyType.Backer):
                if (Vector3.Distance(transform.position, lastPredictedPosition) < (OutOfRoutine ? attackMaxDistance * 2 : attackMaxDistance) && currentCompetenceCooldown == 0 && AttackAngleIsAcceptable && targetShip != null && !PreparingAttack)
                    StartAttackPreparation();
                break;

            case (EnemyType.Catapulter):
                Vector3 predictedTargetShipPosition = targetShip.transform.position + lastTargetShipOffsetForShootDuration;

                if (Vector3.Distance(transform.position, predictedTargetShipPosition) < attackMaxDistance && Vector3.Distance(transform.position, predictedTargetShipPosition) > attackMinDistance && currentCompetenceCooldown == 0 && targetShip != null && !PreparingAttack)
                    StartAttackPreparation();
                break;
        }


        if (PreparingAttack)
            UpdateAttackPreparation();
        #endregion

        #region Routine
        ///checks if the enemy is in a "routine" for too much time, and changes his behaviour
        if (!OutOfRoutine)
        {
            bool isOnRoutine = false;

            switch (enemyDirectionType)
            {
                case (EnemyType.Fronter):
                    break;

                case (EnemyType.Flanker):
                    if (Mathf.Abs(GetRotationCoeff()) < maxRoutineDirectionCoeff && Vector3.Distance(transform.position, lastPredictedPosition) < attackMaxDistance * 2f && !AttackAngleIsAcceptable)
                        isOnRoutine = true;
                    break;

                case (EnemyType.Backer):
                    break;

                case (EnemyType.Catapulter):
                    break;
            }

            if (isOnRoutine)
            {
                //Debug.Log("routine");
                if (remainingTimeBeforeExitRoutine > 0)
                    remainingTimeBeforeExitRoutine -= Time.deltaTime;
                else if (remainingTimeBeforeExitRoutine < 0)
                    StartNonRoutineBehaviour();
            }
            else if (remainingTimeBeforeExitRoutine < timeBeforeExitRoutine)
                remainingTimeBeforeExitRoutine += Time.deltaTime;
        }
        else
            UpdateNonRoutineBehaviour();
        #endregion
    }

    public void AutoDetectPlayer()
    {
        relatedDetectionZone.SetTargetShip(GameManager.gameManager.Player);
    }

    #region Routine
    public void StartNonRoutineBehaviour()
    {
        remainingTimeBeforeExitRoutine = 0;
        remainingTimeBeforeBackToRoutine = timeBeforeBackToRoutine;

        Vector3 targetDirection = targetShip.GetShipVelocity.normalized;

        switch (enemyDirectionType)
        {
            case (EnemyType.Fronter):
                break;

            case (EnemyType.Flanker):
                float product = Vector3.Dot(targetDirection.GetRightOrthogonalVectorXZ(), (targetShip.transform.position - transform.position).normalized);

                if (product > 0)
                    outOfRoutineModificationCoeff = -1;
                else
                    outOfRoutineModificationCoeff = 1;

                break;

            case (EnemyType.Backer):
                break;

            case (EnemyType.Catapulter):
                break;
        }
    }

    public void UpdateNonRoutineBehaviour()
    {
        if (remainingTimeBeforeBackToRoutine > 0)
            remainingTimeBeforeBackToRoutine -= Time.deltaTime;
        else if (remainingTimeBeforeBackToRoutine < 0)
            EndNonRoutineBehaviour();

        switch (enemyDirectionType)
        {
            case (EnemyType.Fronter):
                break;

            case (EnemyType.Flanker):
                if (AttackAngleIsAcceptable && !PreparingAttack)
                {
                    Debug.Log("début d'attaque par hors routine");
                    StartAttackPreparation();
                    outOfRoutineModificationCoeff = 0;
                }
                break;

            case (EnemyType.Backer):
                break;

            case (EnemyType.Catapulter):
                break;
        }
    }

    public void EndNonRoutineBehaviour()
    {
        remainingTimeBeforeBackToRoutine = 0;
        remainingTimeBeforeExitRoutine = timeBeforeExitRoutine;
        outOfRoutineModificationCoeff = 0;
    }

    public bool OutOfRoutine
    {
        get
        {
            return remainingTimeBeforeBackToRoutine != 0;
        }
    }
    #endregion

    public void UpdateEnemyAcquiredDatas()
    {
        if (targetShip == null)
            return;

        currentReactionTime = reactionTime;

        if (!shipMovements.GetCurrentRammingParameters.IsPreparing)
            lastPredictedPosition = GetTargetPosition(targetShip.transform.position, targetShip.GetShipSpeed, targetShip.GetShipVelocity.normalized);
        else
            lastPredictedPosition = (GetTargetPosition(targetShip.transform.position, targetShip.GetShipSpeed, targetShip.GetShipVelocity.normalized) * 3 + targetShip.transform.position) / 4;

        /*if (OutOfRoutine)
        {
            Vector3 targetDirection = targetShip.GetShipVelocity.normalized;

            switch (enemyDirectionType)
            {
                case (EnemyType.Fronter):
                    break;

                case (EnemyType.Flanker):
                    float product = Vector3.Dot(targetDirection.GetRightOrthogonalVectorXZ(), (targetShip.transform.position - transform.position).normalized);

                    if (product > 0)
                        lastPredictedPosition += targetDirection.GetLeftOrthogonalVectorXZ().normalized * outOfRoutineOffsetDistance;
                    else
                        lastPredictedPosition += targetDirection.GetRightOrthogonalVectorXZ().normalized * outOfRoutineOffsetDistance;

                    break;

                case (EnemyType.Backer):
                    break;
            }
        }*/


        #region Consider Obstacles 
        Vector3 correctedDirection = GetDirectionAfterConsideringObstacles(lastPredictedPosition);
        lastPredictedPosition = transform.position + correctedDirection * Vector3.Distance(transform.position, lastPredictedPosition);
        #endregion

        if (targetPositionObjectDebugger != null)
        {
            if(enemyDirectionType != EnemyType.Catapulter)
                targetPositionObjectDebugger.position = lastPredictedPosition;
            else if(targetShip != null)
                targetPositionObjectDebugger.position = targetShip.transform.position + lastTargetShipOffsetForShootDuration;
        }
    }
    #endregion

    #region Watching Round
    [Header("Watching Round")]
    [SerializeField] protected EnemyWatchingRoundParameters watchingRoundParameters;

    public virtual void UpdateWatchingRoundBehaviour()
    {
        Vector3 shipPosition = transform.position;
        shipPosition.y = 0;
        Vector3 pointPosition = watchingRoundParameters.GetAimedWatchingPosition;
        pointPosition.y = 0;

        if (currentReactionTime > 0)
            currentReactionTime -= Time.deltaTime;
        else if (currentReactionTime < 0)
        {
            lastPredictedPosition = watchingRoundParameters.GetAimedWatchingPosition;
            CheckForObstacleOnWatchRoundWay();
        }
            

        float distance = Vector3.Distance(pointPosition, shipPosition);
        if (distance < watchingRoundParameters.GetMinDistanceFromPointToPass)
        {
            watchingRoundParameters.GoToNextPoint();
            lastPredictedPosition = watchingRoundParameters.GetAimedWatchingPosition;

            CheckForObstacleOnWatchRoundWay();
        }
    }

    public void CheckForObstacleOnWatchRoundWay()
    {
        Vector3 correctedDirection = GetDirectionAfterConsideringObstacles(lastPredictedPosition);

        if (correctedDirection != (lastPredictedPosition - transform.position).normalized)
        {
            lastPredictedPosition = transform.position + correctedDirection * 50;
            currentReactionTime = reactionTime;
        }
        else
            currentReactionTime = 0;
    }
    #endregion

    #region Detection
    [Header("Detections")]
    [SerializeField] DetectionZone detectionZonePrefab;
    [SerializeField] float detectionRange;
    protected DetectionZone relatedDetectionZone;
    protected PlayerShip targetShip;
    #endregion

    #region Target Position Finding
    [Header("Target Position Finding Parameters")]
    /// <summary>
    /// Coefficiant indiquant si l'ennemi va plutôt viser plus en avant ou en arrière par rapport à la position prévue de la cible. 1 = neutre, inf à 1 = plus vers la cible, sup à 1 = plus vers l'avant
    /// </summary>
    [SerializeField] float positionAimCoeff = 1;
    public Vector3 FindTargetForeseenPosition(Vector3 targetBasePosition, float targetSpeed, Vector3 targetDirection)
    {
        targetSpeed *= 5 / 5.4f;
        targetBasePosition.y = transform.position.y;
        Vector3 targetEvolutivePosition = transform.position;
        float timeToGetToTarget = 0;

        int iterations = 5;

        float selfSpeed = 0;
        if (enemyDirectionType == EnemyType.Catapulter && projectileLifeTimeDependingOnDistance != null)
        {
            float dist = Vector3.Distance(targetBasePosition, transform.position);
            selfSpeed = dist / projectileLifeTimeDependingOnDistance.Evaluate(dist);
        }
        else
            selfSpeed = shipMovements.GetAverageSpeed;

        for (int i = 0; i < iterations; i++)
        {
            float newTime = Vector3.Distance(targetEvolutivePosition, transform.position) / selfSpeed/*shipMovements.GetAverageSpeed*/;

            if (newTime > timeToGetToTarget && timeToGetToTarget != 0)
                break;

            timeToGetToTarget = newTime;

            targetEvolutivePosition = targetBasePosition + targetDirection.normalized * timeToGetToTarget * targetSpeed;

            if (enemyDirectionType == EnemyType.Catapulter && projectileLifeTimeDependingOnDistance != null)
            {
                float dist = Vector3.Distance(targetEvolutivePosition, transform.position);
                selfSpeed = dist / projectileLifeTimeDependingOnDistance.Evaluate(dist);
            }
        }

        /*switch (enemyDirectionType)
        {
            case (EnemyType.Fronter):
                break;

            case (EnemyType.Flanker):
                float product = Vector3.Dot(targetDirection.GetRightOrthogonalVectorXZ(), shipMovements.GetCurrentShipVelocity.normalized);
                break;

            case (EnemyType.Backer):
                break;

            case (EnemyType.Catapulter):
                break;
        }*/

        return targetEvolutivePosition - ((targetEvolutivePosition - targetBasePosition) * (1 - positionAimCoeff));
    }

    public Vector3 GetTargetPosition(Vector3 targetBasePosition, float targetSpeed, Vector3 targetDirection)
    {
        switch (enemyDirectionType)
        {
            case (EnemyType.Fronter):
                return FindTargetForeseenPosition(targetShip.transform.position, targetShip.GetShipSpeed, targetShip.GetShipVelocity.normalized);

            case (EnemyType.Flanker):
                float product = Vector3.Dot(targetDirection.GetRightOrthogonalVectorXZ(), (targetShip.transform.position -transform.position).normalized);
                Vector3 predictedTargetPosition = FindTargetForeseenPosition(targetShip.transform.position, targetShip.GetShipSpeed, targetShip.GetShipVelocity.normalized);

                if(product > 0)
                    predictedTargetPosition += targetDirection.GetLeftOrthogonalVectorXZ().normalized * attackMaxDistance * 0.75f;
                else
                    predictedTargetPosition += targetDirection.GetRightOrthogonalVectorXZ().normalized * attackMaxDistance * 0.75f;

                return predictedTargetPosition;

            case (EnemyType.Backer):
                return FindTargetForeseenPosition(targetShip.transform.position, targetShip.GetShipSpeed, targetShip.GetShipVelocity.normalized);

            case (EnemyType.Catapulter):
                float productCatapulter = Vector3.Dot(targetDirection.GetRightOrthogonalVectorXZ(), (targetShip.transform.position - transform.position).normalized);
                Vector3 predictedTargetPositionCatapulter = FindTargetForeseenPosition(targetShip.transform.position, targetShip.GetShipSpeed, targetShip.GetShipVelocity.normalized);
                lastTargetShipPredictedPosition = predictedTargetPositionCatapulter;

                #region Catapult Shoot Vector
                if(!PreparingAttack)
                    lastTargetShipOffsetForShootDuration = (lastTargetShipPredictedPosition - targetBasePosition) + (lastTargetShipPredictedPosition - targetBasePosition).normalized * 5;
                #endregion

                if (productCatapulter > 0)
                    predictedTargetPositionCatapulter += targetDirection.GetLeftOrthogonalVectorXZ().normalized * attackMaxDistance * 0.75f;
                else
                    predictedTargetPositionCatapulter += targetDirection.GetRightOrthogonalVectorXZ().normalized * attackMaxDistance * 0.75f;

                return predictedTargetPositionCatapulter;
        }
        return targetShip != null ? targetShip.transform.position : Vector3.zero;
    }

    public bool AttackAngleIsAcceptable
    {
        get
        {
            switch (enemyDirectionType)
            {
                case (EnemyType.Fronter):
                    return Vector3.Angle((lastPredictedPosition - transform.position).normalized, shipMovements.GetCurrentShipVelocity.normalized) < attackMaxAngle;

                case (EnemyType.Flanker):
                    float angle = Vector3.Angle((targetShip.transform.position - transform.position).normalized, shipMovements.GetCurrentShipVelocity.normalized);
                    if(OutOfRoutine)
                        return (angle > 90 - (attackMaxAngle / 4) && angle < 90 + (attackMaxAngle / 4));
                    else
                        return (angle > 90 - (attackMaxAngle / 2) && angle < 90 + (attackMaxAngle / 2));

                case (EnemyType.Backer):
                    return Vector3.Angle((lastPredictedPosition - transform.position).normalized, shipMovements.GetCurrentShipVelocity.normalized) < attackMaxAngle;

                case (EnemyType.Catapulter):
                    return true;
            }

            return Vector3.Angle((lastPredictedPosition - transform.position).normalized, shipMovements.GetCurrentShipVelocity.normalized) < attackMaxAngle;
        }
    }
    #endregion

    #region Movements
    /// <summary>
    /// Angle en dessous duquel le bateau cessera de tourner 
    /// </summary>
    [Header("Movements Parameters")]
    [SerializeField] float minAngleToTurn;
    /// <summary>
    /// Angle à partir duquel le bateau sera à sa vitesse de rotation maximale
    /// </summary>
    [SerializeField] float maxAngleToTurn;

    [SerializeField] float maniabilityModifierWhilePreparing;

    /// <summary>
    /// Renvoie la valeur entre -1 et 1 qui servira à déterminer la direction et la force de rotation du bateau (simula la barre du bateau)
    /// </summary>
    /// <returns></returns>
    public float GetRotationCoeff()
    {
        if (OutOfRoutine)
            return outOfRoutineModificationCoeff;

        Vector3 currentDirection = shipMovements.GetCurrentShipVelocity.normalized;
        Vector3 targetDirection = Vector3.zero;

        targetDirection = (lastPredictedPosition - transform.position).normalized;

        float angle = Vector3.Angle(currentDirection, targetDirection);
        float coeff = Mathf.Clamp((angle - minAngleToTurn) / (maxAngleToTurn - minAngleToTurn), 0, 1);

        if (Vector3.Dot(currentDirection.GetRightOrthogonalVectorXZ(), targetDirection) < 0)
            coeff = -coeff;

        return coeff;
    }

    #region Obstacles Dodging
    /// <summary>
    /// Distance sur laquelle le bateau cherche après un obstacle devant lui
    /// </summary>
    [Header("Obstacles Dodging")]
    [SerializeField] float inFrontMinDistanceToDodge;
    /// <summary>
    /// Désigne de combien on tente de tourner la direction cible lorsque l'ennemi cherche un passage sans obstacle
    /// </summary>
    [SerializeField] float dodgingRotationGap = 10;

    public Vector3 GetDirectionAfterConsideringObstacles(Vector3 targetPosition)
    {
        Vector3 modifiedDirection = (targetPosition - transform.position).normalized;

        #region Checking In Front
        bool inFrontModifiedDirection = false;
        /*Vector3 currentDirection = shipMovements.GetCurrentShipVelocity.normalized;
        Ray frontRay = new Ray(transform.position, currentDirection);

        RaycastHit[] frontHits = Physics.RaycastAll(frontRay, inFrontMinDistanceToDodge);*/
        #endregion

        #region Checking On The Way
        if (!inFrontModifiedDirection)
        {
            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            float onWayCheckDistance = Vector3.Distance(transform.position, targetPosition);

            Ray onWayRay = new Ray(transform.position, targetDirection);
            bool onWayClear = true;

            RaycastHit[] onWayHits = Physics.RaycastAll(onWayRay, onWayCheckDistance);
            //Debug.DrawRay(onWayRay.origin + new Vector3(0, 10, 0), onWayRay.direction * onWayCheckDistance, Color.yellow, reactionTime);

            foreach (RaycastHit hit in onWayHits)
            {
                Obstacle hitObstacle = hit.collider.GetComponent<Obstacle>();
                if (hitObstacle != null)
                {
                    float currentPositiveDodgingAngle = 0;
                    float currentNegativeDodgingAngle = 0;

                    onWayClear = false;

                    while (!onWayClear && Mathf.Abs(currentNegativeDodgingAngle) < 180)
                    {
                        bool clear = true;
                        Ray avoidRay = new Ray();
                        avoidRay.origin = transform.position;

                        bool avoidPositively = true;

                        Color traceColor = Color.white;

                        if (currentPositiveDodgingAngle <= Mathf.Abs(currentNegativeDodgingAngle))
                        {
                            //ici, on essaye de tourner d'un côté
                            currentPositiveDodgingAngle += dodgingRotationGap;
                            avoidRay.direction = Quaternion.Euler(0, currentPositiveDodgingAngle, 0) * targetDirection;
                            traceColor = Color.red;

                            avoidPositively = true;
                        }
                        else
                        {
                            //et ici, on tente de tourner de l'autre
                            currentNegativeDodgingAngle -= dodgingRotationGap;
                            avoidRay.direction = Quaternion.Euler(0, currentNegativeDodgingAngle, 0) * targetDirection;
                            traceColor = Color.yellow;

                            avoidPositively = false;
                        }

                        RaycastHit[] possibleDodgeHit = Physics.RaycastAll(avoidRay, onWayCheckDistance);
                        foreach (RaycastHit dodgeHit in possibleDodgeHit)
                        {
                            Obstacle dodgeHitObstacle = dodgeHit.collider.GetComponent<Obstacle>();
                            if (dodgeHitObstacle != null)
                            {
                                //Debug.DrawRay(dodgeHit.point, new Vector3(0, 100, 0), Color.cyan, reactionTime);
                                //Debug.Log(dodgeHitObstacle);

                                clear = false;
                                break;
                            }
                        }

                        //Debug.DrawRay(avoidRay.origin + new Vector3(0, 1, 0), avoidRay.direction * onWayCheckDistance, clear ? Color.green : traceColor, reactionTime);

                        modifiedDirection = Quaternion.Euler(0, avoidPositively ? dodgingRotationGap : -dodgingRotationGap, 0) * avoidRay.direction;

                        onWayClear = clear;
                    }

                    break;
                }
            }
        }
        #endregion
        
        return modifiedDirection;
    }
    #endregion

    #endregion

    #region Attack Management
    [Header("Attacks, Equipments and Competence")]
    [SerializeField] ShipEquipment equipment;
    ShipEquipment equipmentCopy;
    Competence competenceCopy;
    bool spawnedLinkEquipment;
    Projectile projectilePrefab;
    AnimationCurve projectileLifeTimeDependingOnDistance;

    float attackMinDistance;
    /// <summary>
    /// Distance avec la position ciblée par l'attaque à partir de laquelle l'attaque sera considérée comme viable
    /// </summary>
    [SerializeField] float attackMaxDistance = 45;
    /// <summary>
    /// Angle maximal entre la direction actuelle du bateau et sa direction cible pour lancer la compétence
    /// </summary>
    [SerializeField] float attackMaxAngle = 30;
    float currentCompetenceCooldown;
    [SerializeField] float attackImprecision = 15;

    [SerializeField] float attackPreparationTime = 1;
    float currentAttackPreparationTime;

    #region Attack Properties
    public bool PreparingAttack
    {
        get
        {
            return currentAttackPreparationTime != 0;
        }
    }
    #endregion

    public void StartAttackPreparation()
    {
        currentAttackPreparationTime = attackPreparationTime;
        attackPreparationWarningAnim.gameObject.SetActive(true);
    }

    public void UpdateAttackPreparation()
    {
        if (currentAttackPreparationTime > 0)
            currentAttackPreparationTime -= Time.deltaTime;
        else if (currentAttackPreparationTime < 0)
            StartAttack();
    }

    public void InterruptAttackPreparation()
    {
        currentAttackPreparationTime = 0;
        attackPreparationWarningAnim.gameObject.SetActive(false);
        currentCompetenceCooldown = competenceCopy.GetCompetenceCooldown;
        EndNonRoutineBehaviour();
    }

    public void StartAttack()
    {
        currentAttackPreparationTime = 0;
        attackPreparationWarningAnim.gameObject.SetActive(false);

        bool competenceUsed = false;

        if (competenceCopy as CompetenceRamming != null)
            competenceUsed = competenceCopy.UseCompetence(shipMovements);
        else if (competenceCopy as CompetenceShoot != null)
        {
            if (equipmentCopy.GetEquipmentType == EquipmentType.Catapult)
            {
                Vector3 predictedTargetShipPosition = targetShip.transform.position + lastTargetShipOffsetForShootDuration;
                Vector3 aimedPosition = transform.position + (predictedTargetShipPosition - transform.position).normalized * Mathf.Clamp(Vector3.Distance(transform.position, predictedTargetShipPosition), attackMinDistance, attackMaxDistance);
                Vector3 imprecisionVector = new Vector3(Random.Range(-attackImprecision, attackImprecision), 0, Random.Range(-attackImprecision, attackImprecision));
                competenceUsed = competenceCopy.UseCompetence(aimedPosition + imprecisionVector);

                competenceCopy.StartShowPreview();
                competenceCopy.StartLaunchedPreview();
                /*origin.StartShooting(shootParameters, transform.position + currentAimDir * Vector3.Distance(currentAimPos, transform.position) + new Vector3(Random.Range(-shootImprecision, shootImprecision), 0, Random.Range(-shootImprecision, shootImprecision)), false);
                origin.ShowPreparePreview(shootParameters.GetCurrentSalvo);
                origin.StartLaunchedPreview();*/
            }
            else
                competenceUsed = competenceCopy.UseCompetence(Vector3.zero);
        }

        if (competenceUsed)
            currentCompetenceCooldown = competenceCopy.GetCompetenceCooldown;

        EndNonRoutineBehaviour();
    }

    public void UpdateCooldown()
    {

        if (IsStun || BeingSkewered || stoppedBecausePlayerHasNoControl)
            return;

        if (currentCompetenceCooldown > 0)
            currentCompetenceCooldown -= Time.deltaTime;
        else if (currentCompetenceCooldown < 0)
            currentCompetenceCooldown = 0;
    }

    #region Attack Signs and Feedbacks
    [Header("Attack Signs and Feedbacks")]
    [SerializeField] Animator attackPreparationWarningAnim;
    #endregion

    public ProjectilePoolTag GetUsedProjectileTag
    {
        get
        {
            CompetenceShoot shootComp = equipment.GetPrimaryComp as CompetenceShoot;
            if (shootComp != null)
            {
                return shootComp.GetUsedProjectilePoolTag;
            }
            else
                return ProjectilePoolTag.Null;
        }
    }

    #endregion

    #region Life and Death
    [Header("Life and Death")]
    [SerializeField] List<EnemyShipPhaseParameters> enemyPhaseParameters;
    ShipMovementParameters baseMovementParameters;
    int nextPhaseIndex;

    public void CheckForEnemyPhase(IDamageReceiver damageReceiver)
    {
        if (nextPhaseIndex >= enemyPhaseParameters.Count)
            return;

        float currentLifePercent = lifeManager.GetCurrentLifePercentage;
        if (currentLifePercent <= enemyPhaseParameters[nextPhaseIndex].GetLifePercentToAsign)
            AssignNextPhase();
    }

    public void AssignNextPhase()
    {
        if(enemyPhaseParameters[nextPhaseIndex].GetMovementsParametersCoeff != 0)
            shipMovements.AffectMovementValues(baseMovementParameters.GetParametersWithCoeff(enemyPhaseParameters[nextPhaseIndex].GetMovementsParametersCoeff));
        nextPhaseIndex++;
    }

    public override void Die()
    {
        if (!alreadyDead)
        {
            base.Die();

            ArenaManager arenaManager = ArenaManager.arenaManager;

            EnemyLootCrate lootCrate = lootManager.GenerateEnemyLootCrate();
            if (lootCrate != null)
            {
                lootCrate.transform.position = transform.position;
                lootCrate.SetUpFloatingMove();

                if (arenaManager != null)
                    arenaManager.DropManager.AddDropCrate(lootCrate);
            }

            if (arenaManager != null)
                arenaManager.IncreaseNumberOfKilledEnemies();

            shipBody.velocity = Vector3.zero;
            shipBody.constraints = RigidbodyConstraints.FreezeAll;

            if (shipBoxCollider != null)
                shipBoxCollider.enabled = false;
        }

        #region Variables reset
        externalySetedUp = false;

        remainingStunDuration = 0;
        skeweringProjectile = null;
        currentProtectionParameters = default;
        remainingBlindingDuration = 0;

        currentReactionTime = 0;
        lastPredictedPosition = Vector3.zero;

        remainingTimeBeforeExitRoutine = 0;
        remainingTimeBeforeBackToRoutine = 0;
        outOfRoutineModificationCoeff = 0;

        watchingRoundParameters = default;

        targetShip = null;

        nextPhaseIndex = 0;

        relatedDetectionZone.ResetDetectionZone();
        #endregion
    }

    public int GetCurrentLifeAmount
    {
        get
        {
            return lifeManager.GetCurrentLifeAmount;
        }
    }
    #endregion

    #region Other
    [Header("Other References")]
    [SerializeField] EnemyLifeBar enemyLifeBar;
    #endregion

    #region Loot
    [Header("Loot")]
    [SerializeField] EnemyLootManager lootManager;
    public void SetDropParameters(EnemyDropParameters parameters)
    {
        lootManager.SetDropParameters(parameters);
    }
    #endregion

    #region Stun
    public override void SetStun(float stunDuration)
    {
        base.SetStun(stunDuration);

        InterruptAttackPreparation();
    }
    #endregion

    #region Skewering
    public override void SetCurrentSkeweringProjectile(Projectile projectile)
    {
        base.SetCurrentSkeweringProjectile(projectile);

        InterruptAttackPreparation();
    }
    #endregion

    #region Blinding
    public override void Blind()
    {
        base.Blind();

        InterruptAttackPreparation();
    }

    public override void Blind(float duration)
    {
        base.Blind(duration);

        InterruptAttackPreparation();
    }

    public override void Unblind()
    {
        base.Unblind();
    }
    #endregion

    #region Pooling 
    public bool CheckIfReadyToBeReturnedToPool()
    {
        if (shipFeedbacks != null)
            return alreadyDead && shipFeedbacks.ReadyToBeReturnedToPool;
        else
            return alreadyDead;
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnEnemyShip(this);
    }
    #endregion

    public void StartEnemyHighlighting(PlayerShip playerShip, bool important)
    {
        playerShip.StartEnemyHighlighting(this, important);

        if(important)
            relatedShipHitbox.OnDeath += new OnLifeEvent(StopImportantEnemyShipHighlighting);
        else
            relatedShipHitbox.OnDeath += new OnLifeEvent(StopEnemyShipHighlighting);
    }

    public void StopEnemyShipHighlighting(IDamageReceiver receiver)
    {
        PlayerShip player = GameManager.gameManager.Player;
        player.EndEnemyHighlighting(this);
    }

    public void StopImportantEnemyShipHighlighting(IDamageReceiver receiver)
    {
        PlayerShip player = GameManager.gameManager.Player;
        player.EndImportantEnemyHighlighting(this);
    }

    [Header("Debug")]
    [SerializeField] Transform targetPositionObjectDebugger;
}

public enum EnemyType
{
    None,
    /// <summary>
    /// Ceux qui vont chercher à avoir le joueur/sa future position en face d'eux pour tirer devant ou foncer dedans
    /// </summary>
    Fronter,
    /// <summary>
    /// Ceux qui vont chercher à avoir le joueur/sa future position sur les côtés, pour tirer avec des canons latéraux
    /// </summary>
    Flanker,
    /// <summary>
    /// Ceux qui vont chercher à avoir le joueur/sa future position derrière, pour tirer avec des canons arrière ou lacher des mines
    /// </summary>
    Backer,
    Catapulter
}

public enum EnemyState
{
    Stopped,
    Patrolling,
    ChasingPlayer,
    PreparingAttack,
    Attacking
}