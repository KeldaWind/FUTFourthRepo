using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootCratePool : Pool<EnemyLootCrate>
{
    [SerializeField] LootCratePoolTag lootCrateTag;
    public LootCratePoolTag GetLootCratePoolTag { get { return lootCrateTag; } }
}

public enum LootCratePoolTag
{
    Normal
}