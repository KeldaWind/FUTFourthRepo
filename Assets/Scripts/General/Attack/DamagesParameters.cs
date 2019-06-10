using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Paramètres de dégats infligés par une attaque
/// </summary>
[System.Serializable]
public struct DamagesParameters
{
    public DamagesParameters(int amount, float recoverTime, RecoveringType recoverType)
    {
        damagesAmount = amount;
        recoveringTime = recoverTime;
        recoveringType = recoverType;
    }

    /// <summary>
    /// Quantité de dégats
    /// </summary>
    [SerializeField] int damagesAmount;
    public int GetDamageAmount
    {
        get
        {
            return damagesAmount;
        }
    }

    /// <summary>
    /// Le temps de recover qu'affecte cette attaque
    /// </summary>
    [SerializeField] float recoveringTime;
    public float GetRecoveringTime
    {
        get
        {
            return recoveringTime;
        }
    }


    /// <summary>
    /// Définit si l'attaque considère ou non le recover actuel de la cible pour infliger des dégâts, et comment le recover est affecté à la cible
    /// </summary>
    [SerializeField] RecoveringType recoveringType;
    public RecoveringType GetRecoveringType { get { return recoveringType; } }
}

public enum RecoveringType
{
    /// <summary>
    /// N'infligera de dégâts que si la cible n'est pas en train de recover, et appliquera le temps de recover tel quel
    /// </summary>
    ConsidersRecover,
    /// <summary>
    /// Infligera des dégâts même si la cible est en train de recover. Ne changera pas le recover de la cible.
    /// </summary>
    IgnoreRecoverDontSet,
    /// <summary>
    /// Infligera des dégâts même si la cible est en train de recover. Le temps de recover n'est affecté que si le temps de recover actuel de la cible est plus court.
    /// </summary>
    IgnoreRecoverSetIfHigher,
    /// <summary>
    /// Infligera des dégâts même si la cible est en train de recover. Le temps de recover s'affecte tel quel, quel que soit le temps de recover actuel de la cible.
    /// </summary>
    IgnoreRecoverOverride,
    /// <summary>
    /// Infligera des dégâts même si la cible est en train de recover. Le temps de recover s'ajoute à celui que possède déjà la cible.
    /// </summary>
    IgnoreRecoverAdd
}
