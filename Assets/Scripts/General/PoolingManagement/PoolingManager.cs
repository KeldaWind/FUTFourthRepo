using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolingManager
{
    #region General
    public void SetUpPooling()
    {
        //GenerateProjectilePoolDisctionary();
        GeneratePreviewPoolDisctionary();
        //GenerateEnemyShipsPoolDisctionary();
        GenerateLootCratePoolDisctionary();
        GenerateLootPopUpPoolDisctionary();
        GenerateExplosionsPoolDisctionary();
        GenerateProtectionSpheresPoolDisctionary();
        GenerateSpecialEffectZonePoolDisctionary();
        GenerateFeedbackObjectsPoolDisctionary();
    }
    #endregion

    #region Projectiles
    [Header("Projectiles")]
    [SerializeField] List<ProjectilePool> projectilePools;
    Dictionary<ProjectilePoolTag, ProjectilePool> projectilePoolsDisctionary;

    public void GenerateProjectilePoolDisctionary()
    {
        projectilePoolsDisctionary = new Dictionary<ProjectilePoolTag, ProjectilePool>();

        foreach(ProjectilePool pool in projectilePools)
        {
            projectilePoolsDisctionary.Add(pool.GetProjectilePoolTag, pool);
            pool.SetUpPool();
        }
    }

    public void GenerateProjectilePoolDisctionary(List<ProjectilePoolTag> allProjectilesTags)
    {
        projectilePoolsDisctionary = new Dictionary<ProjectilePoolTag, ProjectilePool>();

        foreach (ProjectilePool pool in projectilePools)
        {
            if (allProjectilesTags.Contains(pool.GetProjectilePoolTag))
            {
                projectilePoolsDisctionary.Add(pool.GetProjectilePoolTag, pool);
                pool.SetUpPool();
            }
        }
    }

    public Projectile GetProjectile(ProjectilePoolTag tag, PoolInteractionType interactionType)
    {
        if (projectilePoolsDisctionary == null)
            return null;

        if (!projectilePoolsDisctionary.ContainsKey(tag))
            return null;

        switch (interactionType)
        {
            case (PoolInteractionType.GetFromPool):
                return projectilePoolsDisctionary[tag].GetObjectFromPool();

            case (PoolInteractionType.PeekFromPool):
                return projectilePoolsDisctionary[tag].PeekObjectFromPool();

            case (PoolInteractionType.PeekPrefab):
                return projectilePoolsDisctionary[tag].PeekPrefab();
        }

        return null;
    }

    public void ReturnProjectile(Projectile projectile)
    {
        if (!projectilePoolsDisctionary.ContainsKey(projectile.GetProjectilePoolTag))
            return;

        projectilePoolsDisctionary[projectile.GetProjectilePoolTag].AddObjectInPool(projectile);
    }
    #endregion

    #region Previews
    [Header("Previews")]
    [SerializeField] List<PreviewPool> previewPools;
    Dictionary<PreviewPoolTag, PreviewPool> previewPoolsDisctionary;

    public void GeneratePreviewPoolDisctionary()
    {
        previewPoolsDisctionary = new Dictionary<PreviewPoolTag, PreviewPool>();

        foreach (PreviewPool pool in previewPools)
        {
            previewPoolsDisctionary.Add(pool.GetPreviewPoolTag, pool);
            pool.SetUpPool();
        }
    }

    public Preview GetPreview(PreviewPoolTag tag, PoolInteractionType interactionType)
    {
        if (!previewPoolsDisctionary.ContainsKey(tag))
            return null;

        switch (interactionType)
        {
            case (PoolInteractionType.GetFromPool):
                return previewPoolsDisctionary[tag].GetObjectFromPool();

            case (PoolInteractionType.PeekFromPool):
                return previewPoolsDisctionary[tag].PeekObjectFromPool();

            case (PoolInteractionType.PeekPrefab):
                return previewPoolsDisctionary[tag].PeekPrefab();
        }

        return null;
    }

    public void ReturnPreview(Preview preview)
    {
        if (!previewPoolsDisctionary.ContainsKey(preview.GetPreviewType))
            return;

        preview.transform.parent = previewPoolsDisctionary[preview.GetPreviewType].GetPoolParent;
        previewPoolsDisctionary[preview.GetPreviewType].AddObjectInPool(preview);
    }
    #endregion

    #region Enemy SHips
    [Header("Enemy Ships")]
    [SerializeField] List<EnemyShipsPool> enemyShipsPool;
    Dictionary<EnemyShipPoolTag, EnemyShipsPool> enemyShipsPoolsDisctionary;

    public void GenerateEnemyShipsPoolDisctionary()
    {
        enemyShipsPoolsDisctionary = new Dictionary<EnemyShipPoolTag, EnemyShipsPool>();

        foreach (EnemyShipsPool pool in enemyShipsPool)
        {
            enemyShipsPoolsDisctionary.Add(pool.GetEnemyPoolTag, pool);
            pool.SetUpPool();
        }
    }

    public void GenerateEnemyShipsPoolDisctionary(List<EnemyShipPoolTag> allEnemiesTags)
    {
        enemyShipsPoolsDisctionary = new Dictionary<EnemyShipPoolTag, EnemyShipsPool>();

        foreach(EnemyShipsPool pool in enemyShipsPool)
        {
            if (allEnemiesTags.Contains(pool.GetEnemyPoolTag))
            {
                enemyShipsPoolsDisctionary.Add(pool.GetEnemyPoolTag, pool);
                pool.SetUpPool();
            }
        }
    }

    public EnemyShip GetEnemyShip(EnemyShipPoolTag tag, PoolInteractionType interactionType)
    {
        if (enemyShipsPoolsDisctionary == null)
        {
            if(interactionType == PoolInteractionType.PeekPrefab)
            {
                foreach(EnemyShipsPool enemyPool in enemyShipsPool)
                {
                    if (enemyPool.GetEnemyPoolTag == tag)
                    {
                        return enemyPool.PeekPrefab();
                    }
                }
            }

            return null;
        }

        if (!enemyShipsPoolsDisctionary.ContainsKey(tag))
            return null;

        switch (interactionType)
        {
            case (PoolInteractionType.GetFromPool):
                return enemyShipsPoolsDisctionary[tag].GetObjectFromPool();

            case (PoolInteractionType.PeekFromPool):
                return enemyShipsPoolsDisctionary[tag].PeekObjectFromPool();

            case (PoolInteractionType.PeekPrefab):
                return enemyShipsPoolsDisctionary[tag].PeekPrefab();
        }

        return null;
    }

    public void ReturnEnemyShip(EnemyShip enemyShip)
    {
        if (!enemyShipsPoolsDisctionary.ContainsKey(enemyShip.GetEnemyPoolTag))
            return;

        enemyShipsPoolsDisctionary[enemyShip.GetEnemyPoolTag].AddObjectInPool(enemyShip);
    }
    #endregion

    #region Loot Crates
    [Header("Loot Crates")]
    [SerializeField] List<LootCratePool> lootCratePools;
    Dictionary<LootCratePoolTag, LootCratePool> lootCratePoolsDictionary;

    public void GenerateLootCratePoolDisctionary()
    {
        lootCratePoolsDictionary = new Dictionary<LootCratePoolTag, LootCratePool>();

        foreach (LootCratePool pool in lootCratePools)
        {
            lootCratePoolsDictionary.Add(pool.GetLootCratePoolTag, pool);
            pool.SetUpPool();
        }
    }

    public EnemyLootCrate GetLootCrate(LootCratePoolTag tag, PoolInteractionType interactionType)
    {
        if (!lootCratePoolsDictionary.ContainsKey(tag))
            return null;

        switch (interactionType)
        {
            case (PoolInteractionType.GetFromPool):
                return lootCratePoolsDictionary[tag].GetObjectFromPool();

            case (PoolInteractionType.PeekFromPool):
                return lootCratePoolsDictionary[tag].PeekObjectFromPool();

            case (PoolInteractionType.PeekPrefab):
                return lootCratePoolsDictionary[tag].PeekPrefab();
        }

        return null;
    }

    public void ReturnLootCrate(EnemyLootCrate lootCrate)
    {
        if (!lootCratePoolsDictionary.ContainsKey(lootCrate.GetLootCratePoolTag))
            return;

        lootCratePoolsDictionary[lootCrate.GetLootCratePoolTag].AddObjectInPool(lootCrate);
    }
    #endregion

    #region Loot Pop Ups
    [Header("Loot Pop Up")]
    [SerializeField] List<LootPopUpPool> lootPopUpPools;
    Dictionary<LootPopUpPoolTag, LootPopUpPool> lootPopUpPoolsDictionary;

    public void GenerateLootPopUpPoolDisctionary()
    {
        lootPopUpPoolsDictionary = new Dictionary<LootPopUpPoolTag, LootPopUpPool>();

        foreach (LootPopUpPool pool in lootPopUpPools)
        {
            lootPopUpPoolsDictionary.Add(pool.GetLootPopUpTag, pool);
            pool.SetUpPool();
        }
    }

    public LootPopUpObject GetLootPopUp(LootPopUpPoolTag tag, PoolInteractionType interactionType)
    {
        if (!lootPopUpPoolsDictionary.ContainsKey(tag))
            return null;

        switch (interactionType)
        {
            case (PoolInteractionType.GetFromPool):
                return lootPopUpPoolsDictionary[tag].GetObjectFromPool();

            case (PoolInteractionType.PeekFromPool):
                return lootPopUpPoolsDictionary[tag].PeekObjectFromPool();

            case (PoolInteractionType.PeekPrefab):
                return lootPopUpPoolsDictionary[tag].PeekPrefab();
        }

        return null;
    }

    public void ReturnLootPopUp(LootPopUpObject lootPopUp)
    {
        if (!lootPopUpPoolsDictionary.ContainsKey(lootPopUp.GetPopUpPoolTag))
            return;

        lootPopUpPoolsDictionary[lootPopUp.GetPopUpPoolTag].AddObjectInPool(lootPopUp);
    }
    #endregion

    #region Explosions
    [Header("Explosions")]
    [SerializeField] List<ExplosionsPool> explosionsPools;
    Dictionary<ExplosionPoolTag, ExplosionsPool> explosionsPoolsDictionary;

    public void GenerateExplosionsPoolDisctionary()
    {
        explosionsPoolsDictionary = new Dictionary<ExplosionPoolTag, ExplosionsPool>();

        foreach (ExplosionsPool pool in explosionsPools)
        {
            explosionsPoolsDictionary.Add(pool.GetExplosionPoolTag, pool);
            pool.SetUpPool();
        }
    }

    public Explosion GetExplosion(ExplosionPoolTag tag, PoolInteractionType interactionType)
    {
        if (!explosionsPoolsDictionary.ContainsKey(tag))
            return null;

        switch (interactionType)
        {
            case (PoolInteractionType.GetFromPool):
                return explosionsPoolsDictionary[tag].GetObjectFromPool();

            case (PoolInteractionType.PeekFromPool):
                return explosionsPoolsDictionary[tag].PeekObjectFromPool();

            case (PoolInteractionType.PeekPrefab):
                return explosionsPoolsDictionary[tag].PeekPrefab();
        }

        return null;
    }

    public void ReturnExplosion(Explosion explosion)
    {
        if (!explosionsPoolsDictionary.ContainsKey(explosion.GetExplosionPoolTag))
            return;

        explosionsPoolsDictionary[explosion.GetExplosionPoolTag].AddObjectInPool(explosion);
    }
    #endregion

    #region Protection Spheres
    [Header("Protection Spheres")]
    [SerializeField] List<ProtectionSpherePool> protectionSpheresPools;
    Dictionary<ProtectionSpherePoolTag, ProtectionSpherePool> protectionSpheresPoolsDictionary;

    public void GenerateProtectionSpheresPoolDisctionary()
    {
        protectionSpheresPoolsDictionary = new Dictionary<ProtectionSpherePoolTag, ProtectionSpherePool>();

        foreach (ProtectionSpherePool pool in protectionSpheresPools)
        {
            protectionSpheresPoolsDictionary.Add(pool.GetProtectionSpherePoolTag, pool);
            pool.SetUpPool();
        }
    }

    public ProtectionSphere GetProtectionSphere(ProtectionSpherePoolTag tag, PoolInteractionType interactionType)
    {
        if (!protectionSpheresPoolsDictionary.ContainsKey(tag))
            return null;

        switch (interactionType)
        {
            case (PoolInteractionType.GetFromPool):
                return protectionSpheresPoolsDictionary[tag].GetObjectFromPool();

            case (PoolInteractionType.PeekFromPool):
                return protectionSpheresPoolsDictionary[tag].PeekObjectFromPool();

            case (PoolInteractionType.PeekPrefab):
                return protectionSpheresPoolsDictionary[tag].PeekPrefab();
        }

        return null;
    }

    public void ReturnProtectionSphere(ProtectionSphere protectionSphere)
    {
        if (!protectionSpheresPoolsDictionary.ContainsKey(protectionSphere.GetProtectionSpherePoolTag))
            return;

        protectionSphere.transform.parent = protectionSpheresPoolsDictionary[protectionSphere.GetProtectionSpherePoolTag].GetPoolParent;
        protectionSpheresPoolsDictionary[protectionSphere.GetProtectionSpherePoolTag].AddObjectInPool(protectionSphere);
    }
    #endregion

    #region Special Effect Zones
    [Header("Special Effect Zones")]
    [SerializeField] List<SpecialEffectZonePool> specialEffectZonePools;
    Dictionary<SpecialEffectZonePoolTag, SpecialEffectZonePool> specialEffectZonePoolsDictionary;

    public void GenerateSpecialEffectZonePoolDisctionary()
    {
        specialEffectZonePoolsDictionary = new Dictionary<SpecialEffectZonePoolTag, SpecialEffectZonePool>();

        foreach (SpecialEffectZonePool pool in specialEffectZonePools)
        {
            specialEffectZonePoolsDictionary.Add(pool.GetSpecialEffectZonePoolTag, pool);
            pool.SetUpPool();
        }
    }

    public SpecialEffectZone GetSpecialEffectZone(SpecialEffectZonePoolTag tag, PoolInteractionType interactionType)
    {
        if (!specialEffectZonePoolsDictionary.ContainsKey(tag))
            return null;

        switch (interactionType)
        {
            case (PoolInteractionType.GetFromPool):
                return specialEffectZonePoolsDictionary[tag].GetObjectFromPool();

            case (PoolInteractionType.PeekFromPool):
                return specialEffectZonePoolsDictionary[tag].PeekObjectFromPool();

            case (PoolInteractionType.PeekPrefab):
                return specialEffectZonePoolsDictionary[tag].PeekPrefab();
        }

        return null;
    }

    public void ReturnSpecialEffectZone(SpecialEffectZone specialEffectZone)
    {
        if (!specialEffectZonePoolsDictionary.ContainsKey(specialEffectZone.GetSpecialEffectZoneType))
            return;

        specialEffectZonePoolsDictionary[specialEffectZone.GetSpecialEffectZoneType].AddObjectInPool(specialEffectZone);
    }
    #endregion

    #region Feedback Objects
    [Header("Feedback Objects")]
    [SerializeField] List<FeedbackObjectPool> feedbackObjectsPool;
    Dictionary<FeedbackObjectPoolTag, FeedbackObjectPool> feedbackObjectsPoolsDisctionary;

    public void GenerateFeedbackObjectsPoolDisctionary()
    {
        feedbackObjectsPoolsDisctionary = new Dictionary<FeedbackObjectPoolTag, FeedbackObjectPool>();

        foreach (FeedbackObjectPool pool in feedbackObjectsPool)
        {
            feedbackObjectsPoolsDisctionary.Add(pool.GetFeedbackObjectTag, pool);
            pool.SetUpPool();
        }
    }

    public FeedbackObject GetFeedbackObject(FeedbackObjectPoolTag tag, PoolInteractionType interactionType)
    {
        if (!feedbackObjectsPoolsDisctionary.ContainsKey(tag))
            return null;

        switch (interactionType)
        {
            case (PoolInteractionType.GetFromPool):
                return feedbackObjectsPoolsDisctionary[tag].GetObjectFromPool();

            case (PoolInteractionType.PeekFromPool):
                return feedbackObjectsPoolsDisctionary[tag].PeekObjectFromPool();

            case (PoolInteractionType.PeekPrefab):
                return feedbackObjectsPoolsDisctionary[tag].PeekPrefab();
        }

        return null;
    }

    public void ReturnFeedbackObject(FeedbackObject fdbckObj)
    {
        if (!feedbackObjectsPoolsDisctionary.ContainsKey(fdbckObj.GetFeedbackPoolTag))
            return;

        feedbackObjectsPoolsDisctionary[fdbckObj.GetFeedbackPoolTag].AddObjectInPool(fdbckObj);
    }
    #endregion

}

public enum PoolInteractionType
{
    GetFromPool, PeekFromPool, PeekPrefab
}
