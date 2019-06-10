using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllGameEquipments", menuName = "Equipments and Co/AllGameEquipments")]
public class AllGameEquipments : ScriptableObject
{
    [SerializeField] List<ShipEquipment> allGameEquipments;
    public ShipEquipment GetEquipment(int equipmentIndex)
    {
        foreach(ShipEquipment equip in allGameEquipments)
        {
            if (equip != null)
            {
                if (equip.GetEquipmentSaveIndex == equipmentIndex)
                    return equip;
            }
        }

        Debug.LogWarning("Equipement introuvable");
        return null;
    }

    public int GetIndex(ShipEquipment equipToFind)
    {
        foreach (ShipEquipment equip in allGameEquipments)
        {
            if (equip != null)
            {
                if (equip == equipToFind)
                    return equip.GetEquipmentSaveIndex;
            }
        }

        Debug.LogWarning("Equipement introuvable");
        return -1;
    }

    [ContextMenu("ShowAllTakenIndexes")]
    public void ShowAllTakenIndexes()
    {
        List<int> allIndexes = new List<int>();
        foreach (ShipEquipment equip in allGameEquipments)
        {
            if (allIndexes.Contains(equip.GetEquipmentSaveIndex))
            {
                Debug.LogWarning("Attention : l'index " + equip.GetEquipmentSaveIndex + " est présent deux fois dans la liste");
            }
            else
            {
                allIndexes.Add(equip.GetEquipmentSaveIndex);
                Debug.Log(equip.GetEquipmentInformations.GetEquipmentName + " : " + equip.GetEquipmentSaveIndex);
            }
        }
    }
}
