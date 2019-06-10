using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RammingParameters
{
    #region General
    public bool IsRamming
    {
        get
        {
            return currentPreparationDuration != 0 || currentAttackDuration != 0;
        }
    }

    public void StartRamming()
    {
        currentPreparationDuration = preparationDuration;
        currentAttackDuration = attackDuration;
    }

    public void UpdateRammingParametersAndUpdateCurrentSpeed(ref float shipSpeed)
    {
        if(currentPreparationDuration > 0)
        {
            currentPreparationDuration -= Time.deltaTime;

            shipSpeed -= preparationDesceleration * Time.deltaTime / Time.timeScale;
            if (shipSpeed < preparationTargetSpeed)
                shipSpeed = preparationTargetSpeed;
        }
        else if (currentPreparationDuration < 0)
        {
            currentPreparationDuration = 0;
        }
        else
        {
            if (currentAttackDuration > 0)
            {
                currentAttackDuration -= Time.deltaTime;

                shipSpeed += attackAcceleration * Time.deltaTime / Time.timeScale;
                if (shipSpeed > attackTargetSpeed)
                    shipSpeed = attackTargetSpeed;
            }
            else if (currentAttackDuration < 0)
            {
                currentAttackDuration = 0;
            }
        }
    }

    public void InterruptRamming()
    {
        currentPreparationDuration = 0;
        currentAttackDuration = 0;
    }
    #endregion

    #region Preparation
    [Header("Preparation")]
    [SerializeField] float preparationDuration;
    float currentPreparationDuration;
    [SerializeField] float preparationTargetSpeed;
    [SerializeField] float preparationDesceleration;
    [SerializeField] float maniabilityWhilePreparing;
    public float GetManiabilityWhilePreparing
    {
        get
        {
            return maniabilityWhilePreparing;
        }
    }

    public bool IsPreparing
    {
        get
        {
            return currentPreparationDuration != 0;
        }
    }
    #endregion

    #region Attack
    [Header("Attack")]
    [SerializeField] float attackDuration; public float AttackDuration { get { return attackDuration; } }
    float currentAttackDuration;
    [SerializeField] float attackTargetSpeed; public float AttackSpeed { get { return attackTargetSpeed; } }
    [SerializeField] float attackAcceleration;
    [SerializeField] float maniabilityWhileAttacking;
    public float GetManiabilityWhileAttacking
    {
        get
        {
            return maniabilityWhileAttacking;
        }
    }

    public bool IsAttacking
    {
        get
        {
            return currentPreparationDuration == 0 && currentAttackDuration != 0;
        }
    }
    #endregion

    #region Recovering
    /*[Header("Recovering")]
    [SerializeField] float recoveringDuration;*/
    #endregion

    #region Damages
    [Header("Damages")] 
    [SerializeField] DamagesParameters damagesParameters;
    public DamagesParameters GetDamagesParameters
    {
        get
        {
            return damagesParameters;
        }
    }
    #endregion

    #region Knockback
    [Header("Knockback")]
    [SerializeField] KnockbackParameters inflictedKnockbackParameters;
    public KnockbackParameters GetInflictedKnockbackParameters
    {
        get
        {
            return inflictedKnockbackParameters;
        }
    }

    [SerializeField] KnockbackParameters sustainedKnockbackParametersOnImpact;
    public KnockbackParameters GetSustainedKnockbackParametersOnImpact
    {
        get
        {
            return sustainedKnockbackParametersOnImpact;
        }
    }
    #endregion

    #region SlowMo
    [Header("Feedbacks")]
    [SerializeField] ScreenshakeParameters screenshakeParameters;
    public ScreenshakeParameters GetScreenshakeParameters
    {
        get
        {
            return screenshakeParameters;
        }
    }

    [SerializeField] SlowMoParameters slowMoParameters;
    public SlowMoParameters GetSlowMoParameters
    {
        get
        {
            return slowMoParameters;
        }
    }
    #endregion

    #region
    [Header("Special")]
    [SerializeField] bool turnAround;
    public bool IsTurnAroundCompetence { get { return turnAround; } }
    #endregion
}
