using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyShipPhaseParameters 
{
    [SerializeField] float lifePercentToAsignValues;
    public float GetLifePercentToAsign
    {
        get
        {
            return lifePercentToAsignValues;
        }
    }

    [SerializeField] float movementParametersCoeff;
    public float GetMovementsParametersCoeff
    {
        get
        {
            return movementParametersCoeff;
        }
    }
}
