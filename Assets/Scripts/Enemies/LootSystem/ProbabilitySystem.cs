using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProbabilitySystem<T>
{
    [SerializeField] List<ItemWithProbabilityWeight<T>> allItems;
    [SerializeField] ShipEquipmentWithProbabilityWeight truc;

    public T GetRandomElementFromSystem()
    {
        int selectionInt = Random.Range(0, GetTotalWeight);
        int probaCounter = 0;

        foreach (ItemWithProbabilityWeight<T> itemWithProba in allItems)
        {
            probaCounter += itemWithProba.GetWeight;
            if (selectionInt < probaCounter)
                return itemWithProba.GetItem;
        }

        return default;
    }

    public int GetTotalWeight
    {
        get
        {
            int total = 0;

            foreach(ItemWithProbabilityWeight<T> itemWithProba in allItems)
            {
                total += itemWithProba.GetWeight;
            }

            return total;
        }
    } 
}

[System.Serializable]
public class IntProbabilitySystem
{
    [SerializeField] List<IntWithProbabilityWeight> allItems;
    public List<IntWithProbabilityWeight> GetAllItems { get { return allItems; } }

    public int GetRandomElementFromSystem()
    {
        int selectionInt = Random.Range(0, GetTotalWeight);
        int probaCounter = 0;

        foreach (IntWithProbabilityWeight intWithProba in allItems)
        {
            probaCounter += intWithProba.GetWeight;
            if (selectionInt < probaCounter)
                return intWithProba.GetItem;
        }

        return 0;
    }

    public int GetTotalWeight
    {
        get
        {
            int total = 0;

            foreach (IntWithProbabilityWeight intWithProba in allItems)
            {
                total += intWithProba.GetWeight;
            }

            return total;
        }
    }
}

[System.Serializable]
public class EquipmentProbabilitySystem
{
    [SerializeField] List<ShipEquipmentWithProbabilityWeight> allItems;
    public List<ShipEquipmentWithProbabilityWeight> GetAllItems { get { return allItems; } }

    public ShipEquipment GetRandomElementFromSystem()
    {
        int selectionInt = Random.Range(0, GetTotalWeight);
        int probaCounter = 0;

        foreach (ShipEquipmentWithProbabilityWeight equipWithProba in allItems)
        {
            probaCounter += equipWithProba.GetWeight;
            if (selectionInt < probaCounter)
                return equipWithProba.GetItem;
        }

        return null;
    }

    public int GetTotalWeight
    {
        get
        {
            int total = 0;

            foreach (ShipEquipmentWithProbabilityWeight equipWithProba in allItems)
            {
                total += equipWithProba.GetWeight;
            }

            return total;
        }
    }

    public void RemoveEquipmentFromSystem(ShipEquipment shipEquipment)
    {
        ShipEquipmentWithProbabilityWeight elementToRemove = null;

        foreach(ShipEquipmentWithProbabilityWeight item in allItems)
        {
            if (item.GetItem == shipEquipment)
            {
                elementToRemove = item;
                break;
            }
        }

        if (elementToRemove != null)
            allItems.Remove(elementToRemove);
    }
}

[System.Serializable]
public class EnemiesProbabilitySystem
{
    [SerializeField] List<EnemyShipWithProbabilityWeight> allItems;
    public List<EnemyShipWithProbabilityWeight> GetAllItems { get { return allItems; } }

    public EnemyShipPoolTag GetRandomElementFromSystem()
    {
        int selectionInt = Random.Range(0, GetTotalWeight);
        int probaCounter = 0;

        foreach (EnemyShipWithProbabilityWeight enemyWithProba in allItems)
        {
            probaCounter += enemyWithProba.GetWeight;
            if (selectionInt < probaCounter)
                return enemyWithProba.GetItem;
        }

        return default;
    }

    public int GetTotalWeight
    {
        get
        {
            int total = 0;

            foreach (EnemyShipWithProbabilityWeight enemyWithProba in allItems)
            {
                total += enemyWithProba.GetWeight;
            }

            return total;
        }
    }
}
