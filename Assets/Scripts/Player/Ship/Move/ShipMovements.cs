using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Module de mouvements permettant de contrôler les mouvements d'un bateau
/// </summary>
[System.Serializable]
public class ShipMovements 
{
    /// <summary>
    /// Bateau relié à ce module de mouvements
    /// </summary>
    Ship relatedShip;

    /// <summary>
    /// Initialisation du module
    /// </summary>
    /// <param name="ship">Bateau relié à ce module de mouvements</param>
    public void SetUp(Ship ship, bool stop)
    {
        stopped = stop;

        relatedShip = ship;

        if (!stopped)
            currentSpeed = minSpeed;
        else
            currentSpeed = 0;

        currentRotation = ship.transform.rotation.eulerAngles.y;

        currentShipSpeedModifiers = new List<ShipSpeedModifier>();
        currentZonesSpeedModifiers = new List<ShipSpeedModifier>();

        currentRammingParameters = default;
        currentRotationSpeed = 0;

        redirecting = false;
        currentStartRedirectionRotation = 0;
        currentTargetRedirectionRotation = 0;
        totalRedirectionTime = 0;
        currentRedirectionTime = 0;

        currentStreamForce = Vector3.zero;
        beingStreamed = false;
    }

    public void AffectMovementValues(ShipMovementParameters values)
    {
        minSpeed = values.minSpeed;
        maxSpeed = values.maxSpeed;
        acceleration = values.acceleration;
        desceleration = values.desceleration;
        if(values.speedCurve.keys.Length > 0)
            speedCurve = values.speedCurve;

        maxRotationSpeed = values.maxRotationSpeed;
        maniability = values.maniability;
        if (values.rotationSpeedCurve.keys.Length > 0)
            rotationSpeedCurve = values.rotationSpeedCurve;
    }

    public void Reset()
    {

    }

    #region Direction
    /// <summary>
    /// Direction actuellement suivie par le bateau
    /// </summary>
    [Header("Balancing : Direction")]    
    Vector3 currentDirection = new Vector3(0, 0, 1);
    /// <summary>
    /// Rotation actuelle du bateau
    /// </summary>
    float currentRotation;
    public float GetCurrentRotation { get { return currentRotation; } }

    float baseShipRotation;

    /// <summary>
    /// Actualisation de la direction actuelle du bateau
    /// </summary>
    public void UpdateCurrentDirection()
    {
        currentDirection = Quaternion.Euler(0, currentRotation, 0) * new Vector3(0, 0, 1);
    }

    public void SetCurrentRotation(float rot)
    {
        currentRotation = rot;
    }
    #endregion

    #region Speed
    /// <summary>
    /// Vitesse minimale du bateau
    /// </summary>
    [Header("Balancing : Speed")]
    [SerializeField] float minSpeed;
    /// <summary>
    /// Vitesse maximale du bateau
    /// </summary>
    [SerializeField] float maxSpeed;
    /// <summary>
    /// Accéleration du bateau (modification par seconde de sa vitesse actuelle vers sa vitesse cible si la vitesse cible est supérieure)
    /// </summary>
    [SerializeField] float acceleration;
    /// <summary>
    /// Descélération du bateau (modification par seconde de sa vitesse actuelle vers sa vitesse cible si la vitesse cible est inférieure)
    /// </summary>
    [SerializeField] float desceleration;
    /// <summary>
    /// Permet de calculer la vitesse cible du bateau en fonction de la vitesse de rotation actuelle du bateau
    /// </summary>
    [SerializeField] AnimationCurve speedCurve;
    /// <summary>
    /// Vitesse actuelle du bateau
    /// </summary>
    float currentSpeed;
    /// <summary>
    /// Vitesse actuelle du bateau
    /// </summary>
    public float GetCurrentSpeed
    {
        get
        {
            return currentSpeed;
        }
    }
    public float GetCurrentSpeedCoeffFromZeroToMax
    {
        get
        {
            return currentSpeed/maxSpeed;
        }
    }

    /// <summary>
    /// Actualisation de la vitesse actuelle du bateau en fonction de sa vitesse cible
    /// </summary>
    public void UpdateCurrentSpeed()
    {
        if (!currentRammingParameters.IsRamming)
        {
            float targetSpeed = Mathf.Lerp(maxSpeed, minSpeed, speedCurve.Evaluate(GetCurrentRotationCoeff)) * currentSpeedAccelerationCoeff;

            if (stopped)
            {
                targetSpeed = 0;
            }

            if (currentSpeed < targetSpeed)
            {
                currentSpeed += acceleration * Time.deltaTime * /*currentSpeedAccelerationCoeff*/currentSpeedAccelerationTargetCoeff;
                if (currentSpeed > targetSpeed)
                    currentSpeed = targetSpeed;
            }
            else if (currentSpeed > targetSpeed)
            {
                currentSpeed -= desceleration * Time.deltaTime * (currentSpeed > maxSpeed ? 3 : targetSpeed < minSpeed ? 2 : 1) * currentSpeedAccelerationCoeff;
                if (currentSpeed < targetSpeed)
                {
                    currentSpeed = targetSpeed;
                    /*if (transformToTakeOnStop != null)
                        relatedShip.SetPositionAndRotation(transformToTakeOnStop);*/
                }
            }
        }
        else
            currentRammingParameters.UpdateRammingParametersAndUpdateCurrentSpeed(ref currentSpeed);
    }

    public float GetAverageSpeed
    {
        get
        {
            return Mathf.Lerp(maxSpeed, minSpeed, speedCurve.Evaluate(1f));
        }
    }
    #endregion

    #region RotationSpeed
    /// <summary>
    /// Vitesse de rotation maximale de ce bateau
    /// </summary>
    [Header("Balancing : Rotation Speed")]
    [SerializeField] float maxRotationSpeed;
    /// <summary>
    /// Maniabilité du bateau (modification par seconde de sa vitesse de rotation actuelle vers la vitesse de rotation cible)
    /// </summary>
    [SerializeField] float maniability;
    /// <summary>
    /// Permet de calculer la vitesse de rotation cible du bateau en fonction de la vitesse de rotation actuelle de la barre
    /// </summary>
    [SerializeField] AnimationCurve rotationSpeedCurve;
    /// <summary>
    /// Vitesse de rotation actuelle
    /// </summary>
    float currentRotationSpeed;

    /// <summary>
    /// Actualisation de la vitesse de rotation actuelle
    /// </summary>
    /// <param name="wheelDeltaRotation"></param>
    public void UpdateCurrentRotationSpeed(float wheelDeltaRotation)
    {
        float targetRotationSpeed = (wheelDeltaRotation > 0 ? 1 : -1) * Mathf.Lerp(0, maxRotationSpeed, rotationSpeedCurve.Evaluate(Mathf.Abs(wheelDeltaRotation))) + GetRotationSpeedStreamModifier;

        if (stopped)
        {
            targetRotationSpeed = 0;
        }

        if (currentRotationSpeed < targetRotationSpeed)
        {
            currentRotationSpeed += maniability * Time.deltaTime * /*currentManiabilityAccelerationCoeff*/currentManiabilityAccelerationTargetCoeff;
            if (currentRotationSpeed > targetRotationSpeed)
                currentRotationSpeed = targetRotationSpeed;
        }
        else if (currentRotationSpeed > targetRotationSpeed)
        {
            currentRotationSpeed -= maniability * Time.deltaTime * currentManiabilityAccelerationCoeff;
            if (currentRotationSpeed < targetRotationSpeed)
                currentRotationSpeed = targetRotationSpeed;
        }

        //currentRotationSpeed += GetRotationSpeedStreamModifier;

        float modifier = 1;
        if (currentRammingParameters.IsRamming)
            modifier = currentRammingParameters.IsPreparing ? currentRammingParameters.GetManiabilityWhilePreparing : currentRammingParameters.GetManiabilityWhileAttacking;

        currentRotation += currentRotationSpeed * modifier * Time.deltaTime;
    }

    /// <summary>
    /// Renvoie une valeur entre 0 et 1 en fonction de la vitesse de rotation actuelle (0 : rotation actuelle nulle ; 1 : vitesse de rotation actuelle maximale)
    /// </summary>
    public float GetCurrentRotationCoeff
    {
        get
        {
            return Mathf.Abs(currentRotationSpeed / maxRotationSpeed);
        }
    }
    public float GetCurrentRotationSignedCoeff
    {
        get
        {
            return (currentRotationSpeed / maxRotationSpeed);
        }
    }
    #endregion

    #region Global
    /// <summary>
    /// Actualisation des valeurs de déplacement
    /// </summary>
    /// <param name="wheelDeltaRotation"></param>
    public void UpdateMovementValues(float wheelDeltaRotation)
    {
        if (Time.timeScale == 0)
            return;

        UpdateCurrentRotationSpeed(wheelDeltaRotation);
        if (redirecting)
            UpdateRedirection();

        UpdateCurrentSpeed();

        UpdateCurrentDirection();

        UpdateCurrentGlobalAcceleration();

        #region Stream
        if (beingStreamed)
        {
            if (streamInfluence < 1)
                streamInfluence = Mathf.Lerp(streamInfluence, 1, streamCoeff);

            if (streamInfluence > 0.99f)
                streamInfluence = 1;
        }
        else
        {
            if (streamInfluence > 0)
                streamInfluence = Mathf.Lerp(streamInfluence, 0, streamCoeff);

            if (streamInfluence < 0.01f)
                streamInfluence = 0;
        }
        #endregion

        if(stopped && transformToTakeOnStop)
        {
            relatedShip.transform.position = Vector3.Lerp(relatedShip.transform.position, transformToTakeOnStop.position, 0.02f);
            currentRotation = Mathf.Lerp(currentRotation, transformToTakeOnStop.rotation.eulerAngles.y, 0.02f);
        }

        #region Speed Modification
        currentSpeedModificationCoeff = UpdateSpeedModifiersAndGetSpeedModificationValue();
        #endregion

        #region SpecialParameters
        if (!currentSpecialShipMoveParameters.IsNull)
            UpdateSpecialMoveParameter();
        #endregion
    }

    /// <summary>
    /// Renvoie la vélocité affectée par le système de déplacement
    /// </summary>
    public Vector3 GetCurrentShipVelocity
    {
        get
        {
            if (Time.timeScale != 0)
                return (currentDirection * currentSpeed * currentSpeedModificationCoeff * Time.deltaTime / Time.timeScale * 50);
            else
                return Vector3.zero;
        }
    }
    #endregion

    #region Stop & Go
    bool stopped;
    public bool Stopped
    {
        get
        {
            return stopped;
        }
    }

    public void StopShip()
    {
        if (stopped)
            return;

        stopped = true;
        currentSpeed = minSpeed;
        currentRammingParameters.InterruptRamming();
    }

    public void InstantStopShip()
    {
        StopShip();
        currentSpeed = 0;
    }

    public void StartShip()
    {
        if (!stopped)
            return;

        stopped = false;
        transformToTakeOnStop = null;
    }

    Transform transformToTakeOnStop;
    public void SetTransformToTakeOnStop(Transform tr)
    {
        transformToTakeOnStop = tr;
        currentRotation %= 360;
        if (currentRotation > 180)
            currentRotation -= 360;
    }
    #endregion

    #region Redirecting
    [Header("Redirection")]
    [SerializeField] AnimationCurve redirectionCurve;
    bool redirecting;
    float currentStartRedirectionRotation;
    float currentTargetRedirectionRotation;

    float totalRedirectionTime;
    float currentRedirectionTime;

    public void SetRedirection(Vector3 redirection, float redirectionCoeff, float redirectionTime)
    {
        redirecting = true;

        float redirectionTotalAngle = Mathf.Abs(Vector3.Angle(redirection, currentDirection) * redirectionCoeff);

        Vector3 rightVect = currentDirection.GetRightOrthogonalVectorXZ();
        if (Vector3.Dot(rightVect, redirection) < 0)
            redirectionTotalAngle = -redirectionTotalAngle;

        currentStartRedirectionRotation = currentRotation;
        currentTargetRedirectionRotation = currentRotation + redirectionTotalAngle;
        
        totalRedirectionTime = redirectionTime;
        currentRedirectionTime = redirectionTime;

        currentRotationSpeed = 0;
    }

    public void UpdateRedirection()
    {
        if (currentRedirectionTime > 0)
            currentRedirectionTime -= Time.deltaTime;
        else if (currentRedirectionTime < 0)
        {
            currentRedirectionTime = 0;
            redirecting = false;
        }

        float coeff = 1 - currentRedirectionTime/totalRedirectionTime;

        currentRotation = Mathf.Lerp(currentStartRedirectionRotation, currentTargetRedirectionRotation, redirectionCurve.Evaluate(coeff));
    }
    #endregion

    #region Ramming
    RammingParameters currentRammingParameters;
    public RammingParameters GetCurrentRammingParameters
    {
        get
        {
            return currentRammingParameters;
        }
    }

    public void StartRamming(RammingParameters rammingParameters)
    {
        currentRammingParameters = rammingParameters;
        currentRammingParameters.StartRamming();

        if (currentRammingParameters.IsTurnAroundCompetence)
            currentRotation += 180;

        relatedShip.PlayRammingFeedback();
        currentRammingParameters.OnRammingEnd = relatedShip.StopRammingFeedback;
    }

    public void InterruptRamming()
    {
        currentRammingParameters.InterruptRamming();
        currentSpeed = minSpeed;
    }
    #endregion

    #region Stream Management
    [Header("Stream Management")]
    [SerializeField] float streamResistance = 2;
    [SerializeField] float streamCoeff = 0.3f;
    Vector3 currentStreamForce;
    bool beingStreamed;
    float streamInfluence;

    public void StartStreamForce(Vector3 streamForce)
    {
        beingStreamed = true;
        currentStreamForce = streamForce;
    }

    public void UpdateStreamForce(Vector3 streamForce)
    {
        currentStreamForce = streamForce;
    }

    public void StopStreamForce(Vector3 streamForce)
    {
        beingStreamed = false;
        currentStreamForce = streamForce;
    }

    public float GetRotationSpeedStreamModifier
    {
        get
        {
            float angle = Vector3.Angle(currentStreamForce, currentDirection);
            angle = (angle < 90 ? angle : 180 - angle);

            float sign = Mathf.Sign(Vector3.Dot(currentDirection.GetRightOrthogonalVectorXZ(), currentStreamForce));

            return Mathf.Clamp(streamInfluence * (angle * sign / streamResistance), -maxRotationSpeed, maxRotationSpeed);
        }
    }

    public Vector3 GetCurrentStreamForce
    {
        get
        {
            return streamInfluence * currentStreamForce;
        }
    }
    #endregion

    #region Modifiers
    [SerializeField] List<ShipSpeedModifier> currentShipSpeedModifiers;
    [SerializeField] List<ShipSpeedModifier> currentZonesSpeedModifiers;

    float currentSpeedModificationCoeff;

    public void StartNewSpeedModifier(ShipSpeedModifier shipSpeedModifier)
    {
        ShipSpeedModifier newModifier = new ShipSpeedModifier(shipSpeedModifier);
        newModifier.StartModificationParameters();
        currentShipSpeedModifiers.Add(newModifier);

        relatedShip.PlaySlowingFeedback();
    }

    public void StartNewSpeedModifier(ShipSpeedModifier shipSpeedModifier, SlowingZone zone)
    {
        ShipSpeedModifier newModifier = new ShipSpeedModifier(shipSpeedModifier);
        newModifier.StartModificationParameters();
        newModifier.SetRelatedZone(zone);
        currentZonesSpeedModifiers.Add(newModifier);

        relatedShip.PlaySlowingFeedback();
    }

    public float UpdateSpeedModifiersAndGetSpeedModificationValue()
    {
        bool beingSlowed = currentShipSpeedModifiers.Count > 0 || currentZonesSpeedModifiers.Count > 0;

        float modificationCoeff = 1;

        float totalCoeff = 0;
        int counter = 0;
        List<ShipSpeedModifier> modifiersToRemove = new List<ShipSpeedModifier>();

        #region Duration Modifiers
        foreach (ShipSpeedModifier shipSpeedModifier in currentShipSpeedModifiers)
        {
            shipSpeedModifier.UpdateModificationParameters();

            if (!shipSpeedModifier.Ended)
            {
                /*totalCoeff += shipSpeedModifier.GetCurrentSpeedModification;
                counter++;*/
                modificationCoeff *= shipSpeedModifier.GetCurrentSpeedModification;
            }
            else
                modifiersToRemove.Add(shipSpeedModifier);
        }

        foreach(ShipSpeedModifier modifierToRemove in modifiersToRemove)
        {
            currentShipSpeedModifiers.Remove(modifierToRemove);
        }
        #endregion

        #region Zone Modifiers
        foreach (ShipSpeedModifier shipSpeedModifier in currentZonesSpeedModifiers)
        {
            shipSpeedModifier.UpdateModificationParameters();

            /*totalCoeff += shipSpeedModifier.GetCurrentSpeedModification;
            counter++;*/
            modificationCoeff *= shipSpeedModifier.GetCurrentSpeedModification;
        }
        #endregion

        /*if (counter == 0)
            return 1;
        else 
            return totalCoeff/(float)counter;*/

        if (beingSlowed && currentShipSpeedModifiers.Count == 0 && currentZonesSpeedModifiers.Count == 0)
            relatedShip.StopSlowingFeedback();

        return modificationCoeff;
    }

    public void GetOutSlowingZone(SlowingZone slowingZone)
    {
        ShipSpeedModifier modiferToRemove = null;

        foreach (ShipSpeedModifier modifier in currentZonesSpeedModifiers)
        {
            if(modifier.GetRelatedSlowingZone == slowingZone)
            {
                modiferToRemove = modifier;
            }
        }

        if (modiferToRemove != null)
        {
            currentZonesSpeedModifiers.Remove(modiferToRemove);
            modiferToRemove.SetRelatedZone(null);
            currentShipSpeedModifiers.Add(modiferToRemove);
        }
    }
    #endregion

    #region Special Moves
    [Header("Special Moves")]
    SpecialShipMoveParameters currentSpecialShipMoveParameters;
    public bool ShipIsAnchored { get { return currentSpecialShipMoveParameters.ShipIsAnchored; } }

    public void StartNewSpecialMove(SpecialShipMoveParameters newParams)
    {
        if(!newParams.IsNull)
        {
            currentSpecialShipMoveParameters = newParams;
            currentSpecialShipMoveParameters.StartParameter();
            if (currentSpecialShipMoveParameters.ShipBoost)
                SetGlobalAcceleration(currentSpecialShipMoveParameters.GetSpeedBoost, currentSpecialShipMoveParameters.GetManiabilityBoost, currentSpecialShipMoveParameters.GetDuration);
        }
    }

    public void UpdateSpecialMoveParameter()
    {
        currentSpecialShipMoveParameters.UpdateParameter();
    }

    #region Global Acceleration
    //Acceleration de la vitesse de déplacement et de la maniabilité, avec un coefficient 
    float currentSpeedAccelerationCoeff = 1;
    float currentManiabilityAccelerationCoeff = 1;
    float remainingGlobalAccelerationTime;
    float currentSpeedAccelerationTargetCoeff = 1;
    float currentManiabilityAccelerationTargetCoeff = 1;
    float globalAccelerationChangeCoeff = 0.1f;

    public void SetGlobalAcceleration(float speedAccelerationCoeff, float maniabilityAccelerationCoeff, float accelerationTime)
    {
        currentSpeedAccelerationTargetCoeff = speedAccelerationCoeff;
        currentManiabilityAccelerationTargetCoeff = maniabilityAccelerationCoeff;
        remainingGlobalAccelerationTime = accelerationTime;
    }

    public void UpdateCurrentGlobalAcceleration()
    {
        currentSpeedAccelerationCoeff = Mathf.Lerp(currentSpeedAccelerationCoeff, currentSpeedAccelerationTargetCoeff, globalAccelerationChangeCoeff);
        currentManiabilityAccelerationCoeff = Mathf.Lerp(currentManiabilityAccelerationCoeff, currentManiabilityAccelerationTargetCoeff, globalAccelerationChangeCoeff);

        if (remainingGlobalAccelerationTime > 0)
            remainingGlobalAccelerationTime -= Time.deltaTime;
        else if (remainingGlobalAccelerationTime < 0)
            EndGlobalAcceleration();
    }

    public void EndGlobalAcceleration()
    {
        remainingGlobalAccelerationTime = 0;
        currentSpeedAccelerationTargetCoeff = 1;
        currentManiabilityAccelerationTargetCoeff = 1;
    }
    #endregion
    #endregion

    public float rotationEnCours()
    {
        return currentRotationSpeed;
    }

    public ShipMovementParameters GetCurrentShipMovementParameters
    {
        get
        {
            ShipMovementParameters newMoveParameters = new ShipMovementParameters();

            newMoveParameters.minSpeed = minSpeed;
            newMoveParameters.maxSpeed = maxSpeed;
            newMoveParameters.acceleration = acceleration;
            newMoveParameters.desceleration = desceleration;

            newMoveParameters.maxRotationSpeed = maxRotationSpeed;
            newMoveParameters.maniability = maniability;

            return newMoveParameters;
        }
    }
}

[System.Serializable]
public struct ShipMovementParameters
{
    [Header("Speed")]
    public float minSpeed;
    public float maxSpeed;
    public float acceleration;
    public float desceleration;
    public AnimationCurve speedCurve;

    [Header("Rotation")]
    public float maxRotationSpeed;
    public float maniability;
    public AnimationCurve rotationSpeedCurve;

    public ShipMovementParameters GetParametersWithCoeff(float coeff)
    {
        ShipMovementParameters newMoveParameters = new ShipMovementParameters();

        newMoveParameters.minSpeed = minSpeed * coeff;
        newMoveParameters.maxSpeed = maxSpeed * coeff;
        newMoveParameters.acceleration = acceleration * coeff;
        newMoveParameters.desceleration = desceleration * coeff;

        newMoveParameters.maxRotationSpeed = maxRotationSpeed * coeff;
        newMoveParameters.maniability = maniability * coeff;

        return newMoveParameters;
    }
}

[System.Serializable]
public class ShipSpeedModifier
{
    public ShipSpeedModifier(ShipSpeedModifier modifier)
    {
        if (modifier == null)
            return;

        modificationSpeedCoeff = modifier.modificationSpeedCoeff;
        modificationDuration = modifier.modificationDuration;
        modificationFadeInCoeff = modifier.modificationFadeInCoeff;
        modificationFadeOutCoeff = modifier.modificationFadeOutCoeff;
    }

    [SerializeField] float modificationSpeedCoeff = 1;
    [SerializeField] float modificationDuration = 0;
    public float GetModificationDuration { get { return modificationDuration; } }
    [SerializeField] float modificationFadeInCoeff = 0.2f;
    [SerializeField] float modificationFadeOutCoeff = 0.2f;

    SlowingZone relatedSlowingZone;
    public SlowingZone GetRelatedSlowingZone { get { return relatedSlowingZone; } }
    public void SetRelatedZone(SlowingZone zone)
    {
        relatedSlowingZone = zone;
    }

    float currentSpeedModification;
    public float GetCurrentSpeedModification { get { return currentSpeedModification; } }

    bool ended;
    public bool Ended { get { return ended; } }

    public void StartModificationParameters()
    {
        currentSpeedModification = 1;
    }

    public void SetAlreadyFullModif()
    {
        currentSpeedModification = modificationSpeedCoeff;
    }

    public void UpdateModificationParameters()
    {
        if (relatedSlowingZone == null)
        {
            if (modificationDuration > 0)
            {
                modificationDuration -= Time.deltaTime;
                currentSpeedModification = Mathf.Lerp(currentSpeedModification, modificationSpeedCoeff, modificationFadeInCoeff * Time.timeScale);
            }
            else if (modificationDuration < 0)
                modificationDuration = 0;
            else
            {
                currentSpeedModification = Mathf.Lerp(currentSpeedModification, 1, modificationFadeOutCoeff * Time.timeScale);
                if (Mathf.Abs(currentSpeedModification - 1) < 0.01f && !ended)
                    ended = true;
            }
        }
        else
            currentSpeedModification = Mathf.Lerp(currentSpeedModification, modificationSpeedCoeff, modificationFadeInCoeff * Time.timeScale);
    }

    public bool IsNull
    {
        get
        {
            return modificationDuration == 0 || modificationSpeedCoeff == 1;
        }
    }
}
