using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyLootManager
{
    [SerializeField] EnemyDropParameters dropParameters;
    ArenaDropManager arenaDropManager;

    public void SetDropParameters(EnemyDropParameters parameters)
    {
        dropParameters = parameters;

        if(arenaDropManager == null)
        {
            if (ArenaManager.arenaManager != null)
                arenaDropManager = ArenaManager.arenaManager.DropManager;
        }
    }

    public EnemyLootCrate GenerateEnemyLootCrate()
    {
        if(dropParameters == null)
        {
            Debug.Log("Pas de parametres de drop assignés");
            return null;
        }

        DropContent dropContent = default;
        if (arenaDropManager != null)
            dropContent = dropParameters.GenerateEnemyLootCrateContent(arenaDropManager.HasToDropEquipment, arenaDropManager.CanDropEquipment);
        else
            dropContent = dropParameters.GenerateEnemyLootCrateContent(false, true);

        if (dropContent.IsNull)
        {
            Debug.Log("pas de bol, pas de loot");
            return null;
        }

        EnemyLootCrate lootCrate = GameManager.gameManager.PoolManager.GetLootCrate(LootCratePoolTag.Normal, PoolInteractionType.GetFromPool);

        lootCrate.SetBoxLoot(dropContent.GetGoldAmount, dropContent.GetEquipment);

        return lootCrate;
    }
}

public enum LootType
{
    Gold, Equipment, Null
}
