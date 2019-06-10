using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllGameEquipmentsManager : MonoBehaviour
{
    public static AllGameEquipmentsManager manager;
    [SerializeField] AllGameEquipments allGameEquipments;
    public AllGameEquipments GetAllGameEquipments { get { return allGameEquipments; } }

    public void SetUp()
    {
        manager = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<int> EquipmentsToIndexes(List<ShipEquipment> equipments)
    {
        List<int> allIndexes = new List<int>();

        foreach (ShipEquipment equip in equipments)
        {
            int index = allGameEquipments.GetIndex(equip);

            if (index > 0)
                allIndexes.Add(index);
        }

        return allIndexes;
    }

    public List<ShipEquipment> IndexesToEquipments(List<int> indexes)
    {
        List<ShipEquipment> allEquipments = new List<ShipEquipment>();

        foreach (int index in indexes)
        {
            ShipEquipment equip = allGameEquipments.GetEquipment(index);

            if (equip != null)
                allEquipments.Add(equip);
        }

        return allEquipments;
    }
}
