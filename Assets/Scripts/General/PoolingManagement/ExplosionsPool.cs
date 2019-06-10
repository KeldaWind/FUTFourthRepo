using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExplosionsPool : Pool<Explosion>
{
    [SerializeField] ExplosionPoolTag explosionTag;
    public ExplosionPoolTag GetExplosionPoolTag { get { return explosionTag; } }
}

public enum ExplosionPoolTag
{
    Normal
}
