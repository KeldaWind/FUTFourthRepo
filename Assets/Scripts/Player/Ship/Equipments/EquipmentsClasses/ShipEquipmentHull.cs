using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equipement applicable sur le joueur. Les équipements de type Hull modifient les valeurs de déplacements du bateau.
/// </summary>
[CreateAssetMenu(fileName = "NewEquipment", menuName = "Equipments and Co/Equipment/HullEquipment", order = 1)]
public class ShipEquipmentHull : ShipEquipment
{
    [Header("Movement Parameters")]
    [SerializeField] ShipMovementParameters movementParameters;
    public ShipMovementParameters GetMovementParameters
    {
        get
        {
            return movementParameters;
        }
    }

    [Header("Life and Armor")]
    [SerializeField] int maximumHullLife;
    public int GetShipMaximumLife
    {
        get
        {
            return maximumHullLife;
        }
    }
    [SerializeField] int maximumHullArmor;
    public int GetShipMaximumArmor
    {
        get
        {
            return maximumHullArmor;
        }
    }

    [Header("Rendering and Hitbox")]
    [SerializeField] GameObject hullPrefab;
    public GameObject GetHullPrefab { get { return hullPrefab; } }

    [SerializeField] Vector3 hullHitboxDimensions;
    public Vector3 GetHullHitboxDimensions { get { return hullHitboxDimensions; } }
}
