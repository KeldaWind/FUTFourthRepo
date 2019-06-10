using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectilePool : Pool<Projectile>
{
    [SerializeField] ProjectilePoolTag projectileTag;
    public ProjectilePoolTag GetProjectilePoolTag
    {
        get
        {
            return projectileTag;
        }
    }
}

public enum ProjectilePoolTag
{
    Canon, Catapult, Mine, TonneauSlow,TonneauExplosion, Piercing, skewering, slowCanon, Null
}
