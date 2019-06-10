using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTarget : MonoBehaviour, IDamageReceiver
{
    bool destroyable;

    [SerializeField] LifeManager lifeManager;
    [SerializeField] AttackTag damageTag;

    private void Start()
    {
        lifeManager.SetUp(this as IDamageReceiver);
    }

    public AttackTag GetDamageTag
    {
        get
        {
            return damageTag;
        }
    }

    public bool IsDamageable
    {
        get
        {
            return destroyable;
        }
    }

    public event OnLifeEvent OnDeath;

    public void SetDestroyable(bool destroy)
    {
        destroyable = destroy;
    }

    public void Die()
    {
        OnDeath?.Invoke(this);

        ArenaManager arenaManager = ArenaManager.arenaManager;

        EnemyLootCrate lootCrate = lootManager.GenerateEnemyLootCrate();
        if (lootCrate != null)
        {
            lootCrate.transform.position = transform.position;
            lootCrate.SetUpFloatingMove();

            if (arenaManager != null)
                arenaManager.DropManager.AddDropCrate(lootCrate);
        }

        Destroy(gameObject);
    }

    public void ReceiveDamage(IDamageSource damageSource, DamagesParameters damagesParameters, ProjectileSpecialParameters projSpecialParameters)
    {
        lifeManager.Damage(damagesParameters.GetDamageAmount, damagesParameters.GetRecoveringTime, damagesParameters.GetRecoveringType);
    }

    private void Update()
    {
        lifeManager.UpdateLifeManagement();
    }

    public void UpdateLifeBar(int lifeAmount)
    {

    }

    #region Drop
    [Header("Dropping")]
    [SerializeField] EnemyLootManager lootManager;
    #endregion
}
