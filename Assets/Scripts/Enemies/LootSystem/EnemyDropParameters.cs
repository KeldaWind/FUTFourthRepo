using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyDropParameters", menuName = "Enemies/DropParameters")]
public class EnemyDropParameters : ScriptableObject
{
    [Header("Gold, Equipment or Nothing")]
    [SerializeField] int goldLootProbability;
    [SerializeField] int equipmentLootProbability;
    [SerializeField] int nullLootProbability;

    [Header("Gold")]
    [SerializeField] IntProbabilitySystem goldLootSystem;
    [SerializeField] int goldLootRandomAmplitude;

    [Header("Equipments : Type Probabilities")]
    [SerializeField] int hullEquipmentProbability;
    [SerializeField] int weaponEquipmentProbability;

    [Header("Equipments : Equipments Lists")]
    [SerializeField] EquipmentProbabilitySystem hullLootSystem;
    [SerializeField] EquipmentProbabilitySystem weaponLootSystem;

    public DropContent GenerateEnemyLootCrateContent(bool hasToDropEquipment, bool canDropEquipment)
    {
        LootType lootType = LootType.Null;
        #region Select loot type
        if (hasToDropEquipment)
        {
            lootType = LootType.Equipment;
        }
        else
        {
            int[] lootTypeProbas = new int[3];
            lootTypeProbas[0] = goldLootProbability;
            lootTypeProbas[1] = equipmentLootProbability;
            lootTypeProbas[2] = nullLootProbability;
            lootType = (LootType)Probabilities.GetRandomIndexFromWeights(lootTypeProbas);
        }
        #endregion

        if (lootType == LootType.Null)
            return new DropContent();



        int lootedGoldAmount = 0;
        ShipEquipment lootedEquipment = null;

        EquipmentType lootedEquipmentType = default;
        switch (lootType)
        {
            case (LootType.Gold):
                lootedGoldAmount = goldLootSystem.GetRandomElementFromSystem();
                break;

            case (LootType.Equipment):

                if (!canDropEquipment)
                {
                    lootedGoldAmount = goldLootSystem.GetRandomElementFromSystem();
                    break;
                }

                #region Select equipment type
                int[] lootEquipmentTypeProba = new int[2];
                lootEquipmentTypeProba[0] = hullEquipmentProbability;
                lootEquipmentTypeProba[1] = weaponEquipmentProbability;
                int equipmentType = Probabilities.GetRandomIndexFromWeights(lootEquipmentTypeProba);
                #endregion

                //loot une coque
                if (equipmentType == 0)
                {
                    if (hullLootSystem.GetAllItems.Count > 0)
                    {
                        lootedEquipmentType = EquipmentType.Hull;
                        lootedEquipment = hullLootSystem.GetRandomElementFromSystem();
                    }
                    else
                    {
                        lootedEquipmentType = EquipmentType.Canon;
                        lootedEquipment = weaponLootSystem.GetRandomElementFromSystem();
                    }
                }
                //loot une arme
                else if (equipmentType == 1)
                {
                    if (weaponLootSystem.GetAllItems.Count > 0)
                    {
                        lootedEquipmentType = EquipmentType.Canon;
                        lootedEquipment = weaponLootSystem.GetRandomElementFromSystem();
                    }
                    else
                    {
                        lootedEquipmentType = EquipmentType.Hull;
                        lootedEquipment = hullLootSystem.GetRandomElementFromSystem();
                    }
                }

                break;
        }

        if (lootedGoldAmount != 0)
        {
            int randomInt = Random.Range(-Mathf.Abs(goldLootRandomAmplitude) / 2, 1 + Mathf.Abs(goldLootRandomAmplitude) / 2);
            lootedGoldAmount += randomInt;
        }

        #region Remove Dropped Equipment from possibilities
        if(lootedEquipment != null)
        {
            Debug.Log("objet looté : " + lootedEquipment);
            if (lootedEquipmentType == EquipmentType.Hull)
                hullLootSystem.RemoveEquipmentFromSystem(lootedEquipment);
            else if (lootedEquipmentType == EquipmentType.Canon)
                weaponLootSystem.RemoveEquipmentFromSystem(lootedEquipment);
        }
        #endregion

        return new DropContent(lootedGoldAmount, lootedEquipment);
    }

    [ContextMenu("ShowAllLootProbabilities")]
    public void ShowAllLootProbabilities()
    {
        #region Gold
        float goldTrueProbability = (float)goldLootProbability / (goldLootProbability + equipmentLootProbability + nullLootProbability);

        foreach (IntWithProbabilityWeight goldPossibleAmount in goldLootSystem.GetAllItems)
        {
            float specificItemProbability = (float)goldPossibleAmount.GetWeight / (float)goldLootSystem.GetTotalWeight;

            Debug.Log(goldPossibleAmount.GetItem + " Gold : " + (specificItemProbability * goldTrueProbability * 100) + "% de chance");
        }
        #endregion

        #region Equipment
        float equipmentTrueProba = (float)equipmentLootProbability / (goldLootProbability + equipmentLootProbability + nullLootProbability);
        float hullTrueProba = (float)hullEquipmentProbability / (hullEquipmentProbability + weaponEquipmentProbability);
        float weaponTrueProba = (float)weaponEquipmentProbability / (hullEquipmentProbability + weaponEquipmentProbability);

        foreach (ShipEquipmentWithProbabilityWeight possibleHull in hullLootSystem.GetAllItems)
        {
            float specificItemProbability = (float)possibleHull.GetWeight / (float)hullLootSystem.GetTotalWeight;

            Debug.Log("Coque " + possibleHull.GetItem.name + " : " + (specificItemProbability * equipmentTrueProba * hullTrueProba * 100) + "% de chance");
        }

        foreach (ShipEquipmentWithProbabilityWeight possibleWeapon in weaponLootSystem.GetAllItems)
        {
            float specificItemProbability = (float)possibleWeapon.GetWeight / (float)weaponLootSystem.GetTotalWeight;

            Debug.Log("Arme " + possibleWeapon.GetItem.name + " : " + (specificItemProbability * equipmentTrueProba * weaponTrueProba * 100) + "% de chance");
        }
        #endregion
    }
}

public struct DropContent
{
    public bool IsNull { get { return goldAmount == 0 && equipment == null; } }

    public DropContent(int gold, ShipEquipment equip)
    {
        goldAmount = gold;
        equipment = equip;
    }

    int goldAmount;
    public int GetGoldAmount { get { return goldAmount; } }

    ShipEquipment equipment;
    public ShipEquipment GetEquipment { get { return equipment; } }
}
