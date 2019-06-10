using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Module à placer sur tout objet capable d'être repoussé par certaines sources
/// </summary>
[System.Serializable]
public class KnockbackManager 
{
    /// <summary>
    /// The ship related to this manager
    /// </summary>
    Ship relatedShip;

    /// <summary>
    /// Initialisation du gestionnaire de knockback
    /// </summary>
    public void SetUp(Ship ship)
    {
        relatedShip = ship;
        currentKnockbackParameters = default;
    }

    public void Reset()
    {

    }

    KnockbackParameters currentKnockbackParameters;

    /// <summary>
    /// Applique le knockback renseigné comme étant le knockback actuel
    /// </summary>
    /// <param name="knockbackParameters"></param>
    public void SetCurrentKnockbackParameters(KnockbackParameters knockbackParameters, Vector3 knockbackDirection, Vector3 redirection)
    {
        if (currentRecoveringTime != 0)
            return;

        if (relatedShip.IsKnockbackProtected && !knockbackParameters.IgnoresProtection)
            return;

        currentKnockbackParameters = knockbackParameters;
        currentKnockbackParameters.Initialize(knockbackDirection, redirection);

        if(redirection != Vector3.zero)
            relatedShip.ShipMvt.SetRedirection(redirection, knockbackParameters.GetRedirectionCoeff, knockbackParameters.GetRedirectionTime);

        currentRecoveringTime = recoveringTime;
    }

    #region KnockbackForce
    public Vector3 UpdateAndGetCurrentKnockbackForce()
    {
        UpdateRecovering();

        currentKnockbackParameters.UpdateKnockbackValues();

        return currentKnockbackParameters.GetCurrentKnockbackForceVector();
    }

    public Vector3 GetCurrentKnockbackForce
    {
        get
        {
            return currentKnockbackParameters.GetCurrentKnockbackForceVector();
        }
    }
    #endregion

    #region Recovering
    [SerializeField] float recoveringTime;
    float currentRecoveringTime;

    public void UpdateRecovering()
    {
        if (currentRecoveringTime > 0)
            currentRecoveringTime -= Time.deltaTime;
        else if (currentRecoveringTime < 0)
            currentRecoveringTime = 0;
    }

    public bool Recovering
    {
        get
        {
            return currentRecoveringTime != 0;
        }
    }
    #endregion
}
