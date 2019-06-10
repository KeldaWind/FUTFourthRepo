using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemWithProbabilityWeight<T>
{
    [SerializeField] T item;
    public T GetItem { get { return item; } }

    [SerializeField] int weight;
    public int GetWeight { get { return weight; } }
}

[System.Serializable] public class IntWithProbabilityWeight : ItemWithProbabilityWeight<int> { }
[System.Serializable] public class ShipEquipmentWithProbabilityWeight : ItemWithProbabilityWeight<ShipEquipment> { }
[System.Serializable] public class EnemyShipWithProbabilityWeight : ItemWithProbabilityWeight<EnemyShipPoolTag> { }