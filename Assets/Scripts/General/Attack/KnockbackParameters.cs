using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Paramètres de knockback infligés par une attaque
/// </summary>
[System.Serializable]
public struct KnockbackParameters
{
    public KnockbackParameters(float force, float duration, float attenuationCoeff, bool ignoresProtec, float redirCoeff, float redirDuration)
    {
        knockbackForce = force;
        knockbackDuration = duration;
        knockbackAttenuationCoeff = attenuationCoeff;
        ignoresProtection = ignoresProtec;

        redirectionCoeff = redirCoeff;
        redirectionTime = redirDuration;

        currentKnockbackForce = 0;
        currentKnockbackDuration = 0;
        knockbackDirection = Vector3.zero;
        redirection = Vector3.zero;
    }

    /// <summary>
    /// La force de base de ce knockback
    /// </summary>
    [Header("Constant Parameters")]
    [SerializeField] float knockbackForce;
    /// <summary>
    /// La durée de base de ce knockback
    /// </summary>
    [SerializeField] float knockbackDuration;
    /// <summary>
    /// Coefficient entre 0 et 1 définissant à quelle vitesse la knockback s'atténue (0 : aucune atténuation, déconseillé ; 1 : atténuiation totale immédiate)
    /// </summary>
    [SerializeField] float knockbackAttenuationCoeff;
    [SerializeField] bool ignoresProtection;
    public bool IgnoresProtection { get { return ignoresProtection; } }

    float currentKnockbackForce;
    float currentKnockbackDuration;

    /// <summary>
    /// La direction actuelle du knockback
    /// </summary>
    [Header("Situational Parameters")]
    Vector3 knockbackDirection;

    /// <summary>
    /// Définit la direction dans laquelle l'objet va être repoussé
    /// </summary>
    /// <param name="dir">Direction dans laquelle l'objet va être repoussé</param>
    public void SetKnockbackDirection(Vector3 dir)
    {
        knockbackDirection = dir;
    }

    /// <summary>
    /// Définit à quel point ce knockback va rediriger le bateau dans la direction rensignée
    /// </summary>
    [Header("Redirection Parameters")]
    [SerializeField] float redirectionCoeff;
    public float GetRedirectionCoeff
    {
        get
        {
            return redirectionCoeff;
        }
    }
    /// <summary>
    /// Vitesse de rotation du bateau qui va devoir être redirigé
    /// </summary>
    [SerializeField] float redirectionTime;
    public float GetRedirectionTime
    {
        get
        {
            return redirectionTime;
        }
    }
    /// <summary>
    /// Direction in which the boat has to be redirected
    /// </summary>
    Vector3 redirection;
    public Vector3 GetRedirection
    {
        get
        {
            return redirection;
        }
    }

    /// <summary>
    /// Définit la direction dans laquelle l'objet va partiellement être redirigé
    /// </summary>
    /// <param name="dir">Direction dans laquelle l'objet va partiellement être redirigé</param>
    public void SetRedirection(Vector3 dir)
    {
        redirection = dir;
    }


    public void Initialize(Vector3 knbkDir, Vector3 redir)
    {
        currentKnockbackForce = knockbackForce;
        currentKnockbackDuration = knockbackDuration;
        SetKnockbackDirection(knbkDir);
        SetRedirection(redir);
    }

    public void UpdateKnockbackValues()
    {
        if (currentKnockbackDuration > 0)
            currentKnockbackDuration -= Time.deltaTime;
        else if (currentKnockbackDuration < 0)
        {
            currentKnockbackDuration = 0;
        }
        else if(currentKnockbackDuration == 0)
        {
            currentKnockbackForce = Mathf.Lerp(currentKnockbackForce, 0, knockbackAttenuationCoeff);
        }

        if(currentKnockbackForce < 0.01f)
        {
            currentKnockbackForce = 0;
            knockbackDirection = Vector3.zero;
        }
    }

    public Vector3 GetCurrentKnockbackForceVector()
    {
        if (Time.timeScale != 0)
            return knockbackDirection * currentKnockbackForce * 50 * Time.deltaTime / Time.timeScale;
        else
            return Vector3.zero;
    } 
}
