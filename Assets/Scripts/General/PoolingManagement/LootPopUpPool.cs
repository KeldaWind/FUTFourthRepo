using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class LootPopUpPool : Pool<LootPopUpObject>
{
    [SerializeField] LootPopUpPoolTag lootPopUpTag;
    public LootPopUpPoolTag GetLootPopUpTag { get { return lootPopUpTag; } }
}

public enum LootPopUpPoolTag
{
    Normal
}