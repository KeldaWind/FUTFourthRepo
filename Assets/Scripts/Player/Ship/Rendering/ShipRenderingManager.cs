using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Module permettant de gérer le rendering d'un bateau
/// </summary>
[System.Serializable]
public class ShipRenderingManager 
{
    /// <summary>
    /// Parent du renderer du bateau
    /// </summary>
    [SerializeField] Transform rendererParent;
    /// <summary>
    /// Rotation normale du bateai
    /// </summary>
    Vector3 normalRendererRotation;

    /// <summary>
    /// Initialisation du gestionnaire
    /// </summary>
    public void SetUp(float baseRotation, LifeManager lifeManager)
    {
        normalRendererRotation = rendererParent.localRotation.eulerAngles - new Vector3(0, baseRotation, 0);
        rendererParent.gameObject.SetActive(true);

        lifeFeedbacksManager.SetUp(lifeManager);

        rotationPerDamagesTakenCurve.preWrapMode = WrapMode.PingPong;
        rotationPerDamagesTakenCurve.postWrapMode = WrapMode.PingPong;

        if(ecumeDevantD != null)
            ecumeDevantDBaseSpeed = ecumeDevantD.emissionRate;
        if (ecumeDevantG != null)
            ecumeDevantGBaseSpeed = ecumeDevantG.emissionRate;
        if (ecumeArriere != null)
            ecumeArriereBaseSpeed = ecumeArriere.emissionRate;
    }

    [Header("VariablesPierre")]
    [SerializeField] Transform rotationParent;
    [SerializeField] AnimationCurve curveTangage;
    [SerializeField] float maxTangageAngle;
    [SerializeField] TrailRenderer trailMilieu;
    [SerializeField] TrailRenderer trailGauche;
    [SerializeField] TrailRenderer trailDroite;

    [Header("ParticuleBato")]
    [SerializeField] ParticleSystem ecumeDroite;
    [SerializeField] ParticleSystem ecumeGauche;
    [Header("ParticulesVitesse")]
    [SerializeField] ParticleSystem ecumeDevantD;
    float ecumeDevantDBaseSpeed;
    [SerializeField] ParticleSystem ecumeDevantG;
    float ecumeDevantGBaseSpeed;
    [SerializeField] ParticleSystem ecumeArriere;
    float ecumeArriereBaseSpeed;
    [SerializeField] AnimationCurve evolutionParticleNumber;

    /// <summary>
    /// Tourne le renderer en direction du vecteur indiqué
    /// </summary>
    /// <param name="lookDirection">Direction vers laquelle le bateau doit regarder</param>
    public void TurnRendererTowardVector(Vector3 lookDirection)
    {
        float rotY = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;

        if (rendererParent != null)
            rendererParent.localRotation = Quaternion.Euler(normalRendererRotation + new Vector3(0, rotY, 0));
    }

    public void UpdateRendering(float currentRotationCoeff, float currentSpeedCoeff, bool stopped)
    {
        if(currentMaxRotationPerDamage != 0)
            UpdateDamageRotation();

        Tangage(currentRotationCoeff);
        ParticleSpeedUpdate(currentSpeedCoeff);

        lifeFeedbacksManager.UpdateLifeFeedbacksManagement();

        if (stopped)
        {

        }
        else
        {

        }
    }

    float currentTangageCoeff;
    void Tangage(float currentShipRotationCoeff)
    {
        //float rota = curveTangage.Evaluate(currentShipRotation);

        currentTangageCoeff = Mathf.Lerp(currentTangageCoeff, currentShipRotationCoeff, 0.1f);
        #region Rotation
        if (rotationParent != null)
        {
            float movementRotation = curveTangage.Evaluate(/*currentShipRotationCoeff*/currentTangageCoeff) * maxTangageAngle;
            if (/*currentShipRotationCoeff*/currentTangageCoeff > 0)
                movementRotation = -movementRotation;

            float damageRotation = GetCurrentDamagesRotation;
            float globalRotation = movementRotation + damageRotation;

            rotationParent.localRotation = Quaternion.Euler(0, 0, globalRotation);
        }
        #endregion

        if (/*currentShipRotationCoeff*/ currentTangageCoeff > 0)
        {
            if (ecumeDroite != null)
            {
                if (/*currentShipRotationCoeff*/currentTangageCoeff > 0.5)
                    ecumeDroite.Play();
                else
                    ecumeDroite.Stop();
            }

        }
        else if(/*currentShipRotationCoeff*/currentTangageCoeff < 0)
        {
            if (ecumeGauche != null)
            {
                if (/*currentShipRotationCoeff*/currentTangageCoeff < -0.5)
                    ecumeGauche.Play();
                else
                    ecumeGauche.Stop();
            }
        }
    }
    void ParticleSpeedUpdate(float currentShipSpeedCoeff)
    {
        if (ecumeDevantD != null)
            ecumeDevantD.emissionRate = evolutionParticleNumber.Evaluate(currentShipSpeedCoeff) * ecumeDevantDBaseSpeed;
        if (ecumeDevantG != null)
            ecumeDevantG.emissionRate = evolutionParticleNumber.Evaluate(currentShipSpeedCoeff) * ecumeDevantGBaseSpeed;
        if (ecumeArriere != null)
            ecumeArriere.emissionRate = evolutionParticleNumber.Evaluate(currentShipSpeedCoeff) * ecumeArriereBaseSpeed;
    }

    public void HideShip()
    {
        rendererParent.gameObject.SetActive(false);
    }

    [Header("Life Feedbacks")]
    [SerializeField] ShipLifeFeedbacksManager lifeFeedbacksManager;

    [Header("Damages Feedback")]
    [SerializeField] float rotationForcePerTakenDamage = 10;
    [SerializeField] float maxRotationFromDamages = 30;
    [SerializeField] float rotationFromDamageDesceleration = 15;
    [SerializeField] float rotationFromDamageSpeed = 3;
    [SerializeField] AnimationCurve rotationPerDamagesTakenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    float currentMaxRotationPerDamage;
    float damageCurveCounter;

    public void StartDamageAnimation(int damages, DamageSourceRelativePosition damageSourceRelativePosition)
    {
        currentMaxRotationPerDamage = Mathf.Clamp(rotationForcePerTakenDamage * damages, 0, maxRotationFromDamages);

        if(damageSourceRelativePosition == DamageSourceRelativePosition.Right)
            damageCurveCounter = 0.5f;
        else
            damageCurveCounter = 1.5f;
    }

    public void UpdateDamageRotation()
    {
        if (currentMaxRotationPerDamage > 0)
        {
            currentMaxRotationPerDamage -= rotationFromDamageDesceleration * Time.deltaTime;
            damageCurveCounter += Time.deltaTime * rotationFromDamageSpeed;
        }
        if (currentMaxRotationPerDamage < 0)
            currentMaxRotationPerDamage = 0;
    }

    public float GetCurrentDamagesRotation
    {
        get
        {
            if (currentMaxRotationPerDamage == 0)
                return 0;

            float rotCoeff = rotationPerDamagesTakenCurve.Evaluate(damageCurveCounter);
            rotCoeff = (rotCoeff * 2) - 1;
            return rotCoeff * currentMaxRotationPerDamage;
        }
    }

    #region Hull
    [Header("Hull")]
    [SerializeField] ShipHullRenderer currentHullObject;
    public void InstantiateHullRenderer(ShipHullRenderer hullPrefab, WeaponInformationType canonType)
    {
        if (currentHullObject != null)
            ClearLastHullRenderer();

        currentHullObject = Object.Instantiate(hullPrefab, rotationParent);
        currentHullObject.transform.localPosition = Vector3.zero;
        currentHullObject.transform.localRotation = Quaternion.identity;
        currentHullObject.name = "Currently Equiped Hull";

        hullPrefab.CheckObjectsToShow(canonType);
    }

    public void ClearLastHullRenderer()
    {
        Object.Destroy(currentHullObject.gameObject);
        currentHullObject = null;
    }
    #endregion
}
public enum DamageSourceRelativePosition { Right, Left}

[System.Serializable]
public class ShipLifeFeedbacksManager
{
    LifeManager shipLifeManager;

    public void SetUp(LifeManager lifeManager)
    {
        shipLifeManager = lifeManager;

        lifeManager.OnLifeChange += UpdateLifeAmount;

        if (smokeParticleSystem != null)
        {
            smokeParticlesMainModule = smokeParticleSystem.main;
            smokeParticlesEmissionModule = smokeParticleSystem.emission;
        }

        if(fireParticleSystem != null)
        {
            fireParticlesMainModule = fireParticleSystem.main;
            fireParticlesEmissionModule = fireParticleSystem.emission;
        }
    }

    public void UpdateLifeAmount(IDamageReceiver shipReceiver)
    {
        if (currentStepIndex >= allShipLifeFeedbacksSteps.Length)
            return;

        float currentLifePercentage = shipLifeManager.GetCurrentLifePercentage;
        
        if(currentLifePercentage <= allShipLifeFeedbacksSteps[currentStepIndex].stepLifePercentage)
        {
            ApplyNewFeedbackStep(allShipLifeFeedbacksSteps[currentStepIndex]);
            currentStepIndex++;
        }
    }

    public void ApplyNewFeedbackStep(ShipLifeFeedbacksStep step)
    {
        //smokeParticlesMainModule.startColor = step.smokeColor;
        currentFeedbackStep = step;
    }

    ShipLifeFeedbacksStep currentFeedbackStep;

    public void UpdateLifeFeedbacksManagement()
    {
        if (currentFeedbackStep.stepLifePercentage == 0)
            return;

        if (smokeParticleSystem != null)
        {
            smokeParticlesMainModule.startColor = Color.Lerp(smokeParticlesMainModule.startColor.color, currentFeedbackStep.smokeColor, transitionSpeedCoeff);
            smokeParticlesMainModule.startSize = Mathf.Lerp(smokeParticlesMainModule.startSize.constant, currentFeedbackStep.smokeSize, transitionSpeedCoeff);
            smokeParticlesEmissionModule.rateOverTime = Mathf.Lerp(smokeParticlesEmissionModule.rateOverTime.constant, currentFeedbackStep.smokeEmissionSpeed, transitionSpeedCoeff);
        }

        if(fireParticleSystem != null)
        {
            fireParticlesMainModule.startSize = Mathf.Lerp(fireParticlesMainModule.startSize.constant, currentFeedbackStep.fireSize, transitionSpeedCoeff);
            fireParticlesEmissionModule.rateOverTime = Mathf.Lerp(fireParticlesEmissionModule.rateOverTime.constant, currentFeedbackStep.fireEmissionSpeed, transitionSpeedCoeff);
        }
    }

    [Header("Steps")]
    [SerializeField] ShipLifeFeedbacksStep[] allShipLifeFeedbacksSteps;
    [SerializeField] float transitionSpeedCoeff = 0.01f;
    int currentStepIndex;

    [Header("Particle System")]
    [SerializeField] ParticleSystem smokeParticleSystem;
    ParticleSystem.MainModule smokeParticlesMainModule;
    ParticleSystem.EmissionModule smokeParticlesEmissionModule;

    [SerializeField] ParticleSystem fireParticleSystem;
    ParticleSystem.MainModule fireParticlesMainModule;
    ParticleSystem.EmissionModule fireParticlesEmissionModule;
}

[System.Serializable]
public struct ShipLifeFeedbacksStep
{
    public float stepLifePercentage;
    [Header("Smoke")]
    public Color smokeColor;
    public float smokeEmissionSpeed;
    public float smokeSize;
    [Header("Fire")]
    public float fireEmissionSpeed;
    public float fireSize;
}