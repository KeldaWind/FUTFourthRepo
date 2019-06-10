using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArenaDropManager
{
    List<EnemyLootCrate> droppedCrates;
    [SerializeField] int minimumNumberOfDroppedEquipments = 1;
    [SerializeField] int maximumNumberOfDroppedEquipments = 3;
    int currentNumberOfDroppedEquipments;

    public bool HasToDropEquipment
    {
        get
        {
            return currentNumberOfDroppedEquipments < minimumNumberOfDroppedEquipments;
        }
    }

    public bool CanDropEquipment
    {
        get
        {
            return currentNumberOfDroppedEquipments < maximumNumberOfDroppedEquipments;
        }
    }

    public void AddDropCrate(EnemyLootCrate newLootCrate)
    {
        if (droppedCrates == null)
            droppedCrates = new List<EnemyLootCrate>();

        droppedCrates.Add(newLootCrate);

        if (newLootCrate != null)
            if (newLootCrate.GetLootedEquipment != null)
                currentNumberOfDroppedEquipments++;
    }

    bool lootingAll;

    public void RemoveDropCrate(EnemyLootCrate lootCrateToRemove)
    {
        if(!lootingAll && droppedCrates != null)
            droppedCrates.Remove(lootCrateToRemove);
    }

    public void LootAllRemainingCrates()
    {
        PlayerShip playerShip = GameManager.gameManager.Player;

        lootingAll = true;

        if (droppedCrates == null)
        {
            lootingAll = false;
            droppedCrates = new List<EnemyLootCrate>();
            return;
        }

        foreach (EnemyLootCrate lootCrate in droppedCrates)
        {
            lootCrate.LootCrate(playerShip);
        }

        lootingAll = false;

        droppedCrates = new List<EnemyLootCrate>();
    }

    [Header("Upgraded Equipments Probabilities : One Star")]
    [SerializeField] IntProbabilitySystem oneStarLevel2EquipmentsProbabilities;
    [SerializeField] IntProbabilitySystem oneStarLevel3EquipmentsProbabilities;

    [Header("Upgraded Equipments Probabilities : Two Stars")]
    [SerializeField] IntProbabilitySystem twoStarsLevel2EquipmentsProbabilities;
    [SerializeField] IntProbabilitySystem twoStarsLevel3EquipmentsProbabilities;

    [Header("Upgraded Equipments Probabilities : Three Stars")]
    [SerializeField] IntProbabilitySystem threeStarsLevel2EquipmentsProbabilities;
    [SerializeField] IntProbabilitySystem threeStarsLevel3EquipmentsProbabilities;

    public List<ShipEquipment> ChangePlayerLootWithRarity(List<ShipEquipment> baseLoot, int numberOfStars)
    {
        #region Lists Composition
        List<ShipEquipment> nonUpgradedEquipments = new List<ShipEquipment>(baseLoot);
        List<ShipEquipment> modifiedEquipments = new List<ShipEquipment>();
        List<ShipEquipment> upgradableToLevel2Equipments = new List<ShipEquipment>();
        List<ShipEquipment> upgradableToLevel3Equipments = new List<ShipEquipment>();

        foreach(ShipEquipment equip in nonUpgradedEquipments)
        {
            if (equip.Upgradable)
            {
                ShipEquipment level2Equip = equip.GetUpgradedEquipment;
                upgradableToLevel2Equipments.Add(equip);

                if (level2Equip.Upgradable)
                {
                    //ShipEquipment level3Equip = level2Equip.GetUpgradedEquipment;
                    upgradableToLevel3Equipments.Add(equip);
                }
            }
        }
        #endregion

        #region Pick Number of Upgraded Equipments
        int numberOfLevel2Equipments = 0;
        int numberOfLevel3Equipments = 0;

        if(numberOfStars == 1)
        {
            numberOfLevel2Equipments = oneStarLevel2EquipmentsProbabilities.GetRandomElementFromSystem();
            numberOfLevel3Equipments = oneStarLevel3EquipmentsProbabilities.GetRandomElementFromSystem();
        }
        else if (numberOfStars == 2)
        {
            numberOfLevel2Equipments = twoStarsLevel2EquipmentsProbabilities.GetRandomElementFromSystem();
            numberOfLevel3Equipments = twoStarsLevel3EquipmentsProbabilities.GetRandomElementFromSystem();
        }
        else if (numberOfStars == 3)
        {
            numberOfLevel2Equipments = threeStarsLevel2EquipmentsProbabilities.GetRandomElementFromSystem();
            numberOfLevel3Equipments = threeStarsLevel3EquipmentsProbabilities.GetRandomElementFromSystem();
        }
        #endregion

        /*Debug.Log(numberOfLevel2Equipments + " Niv2");
        Debug.Log(numberOfLevel3Equipments + " Niv3");*/

        #region Create new dropped equipments
        for(int i = 0; i < numberOfLevel3Equipments; i++)
        {
            if(upgradableToLevel3Equipments.Count == 0)
            {
                numberOfLevel2Equipments += (numberOfLevel3Equipments - i);
                break; 
            }

            ShipEquipment pickedEquipment = upgradableToLevel3Equipments.GetRandomMemberOfTheList();
            upgradableToLevel3Equipments.Remove(pickedEquipment);
            upgradableToLevel2Equipments.Remove(pickedEquipment);
            nonUpgradedEquipments.Remove(pickedEquipment);
            modifiedEquipments.Add(pickedEquipment.GetUpgradedEquipment.GetUpgradedEquipment);
        }

        for (int i = 0; i < numberOfLevel2Equipments; i++)
        {
            if (upgradableToLevel2Equipments.Count == 0)
                break;

            ShipEquipment pickedEquipment = upgradableToLevel2Equipments.GetRandomMemberOfTheList();
            upgradableToLevel2Equipments.Remove(pickedEquipment);
            nonUpgradedEquipments.Remove(pickedEquipment);
            modifiedEquipments.Add(pickedEquipment.GetUpgradedEquipment);
        }
        #endregion

        foreach (ShipEquipment equip in nonUpgradedEquipments)
            modifiedEquipments.Add(equip);

        return modifiedEquipments;
    }
}
