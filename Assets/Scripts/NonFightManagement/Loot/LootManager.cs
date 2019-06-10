using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootManager
{
    [Header("Balancing")]
    [SerializeField] float conservedGoldWhenPlayerLose;

    int totalLootedGold = 0;
    public int GetAllLootedGold { get { return totalLootedGold; } }

    List<ShipEquipment> allLootedEquipments = new List<ShipEquipment>();
    public List<ShipEquipment> GetAllLootedEquipments { get { return allLootedEquipments; } }

    public void SetUp()
    {
        allLootedEquipments = new List<ShipEquipment>();
    }

    public void AddLootedGold(int amount)
    {
        if(MapManager.mapManager == null)
            totalLootedGold += amount;
        else
        {
            if (IntersceneManager.intersceneManager != null)
            {
                IntersceneManager.intersceneManager.GetPlayerDatas.EarnMoney(amount);
                MapManager.mapManager.DocksManager.UpdateGoldText();
                MapManager.mapManager.SavePlayerDatas();
            }
        }
            
    }

    public void AddLootedEquipment(ShipEquipment equipment)
    {
        if (equipment != null)
            allLootedEquipments.Add(equipment);
    }

    #region Earn and Lose
    public void EarnLoot(ArenaDropManager dropManager, int numberOfStars)
    {
        allLootedEquipments = dropManager.ChangePlayerLootWithRarity(allLootedEquipments, numberOfStars);

        if (IntersceneManager.intersceneManager != null)
            IntersceneManager.intersceneManager.GetPlayerDatas.EarnMoney(totalLootedGold);
    }

    public void LoseLoot()
    {
        allLootedEquipments = new List<ShipEquipment>();
        totalLootedGold = (int)(totalLootedGold * conservedGoldWhenPlayerLose);

        if (IntersceneManager.intersceneManager != null)
            IntersceneManager.intersceneManager.GetPlayerDatas.EarnMoney(totalLootedGold);
    }
    #endregion

    #region Debug
    public void DebugPlayerLoot()
    {
        Debug.Log("Gold : " + totalLootedGold);
        foreach (ShipEquipment equip in allLootedEquipments)
        {
            Debug.Log("Equipement : " + equip);
        }
    }
    #endregion
}
