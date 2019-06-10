using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LifeManager 
{
    IDamageReceiver relatedDamageReceiver;

    EnemyLifeBar lifeBar;

    public void SetUp(IDamageReceiver damageReceiver)
    {
        currentLife = maxLife;
        relatedDamageReceiver = damageReceiver;
    }

    public void SetUpLifeBar(EnemyLifeBar enemyLifeBar)
    {
        if (enemyLifeBar != null)
        {
            lifeBar = enemyLifeBar;
            lifeBar.SetUp(maxLife);
        }
    }

    public void UpdateLifeManagement()
    {
        if (Recovering)
            UpdateRecovering();
    }

    #region Life
    /// <summary>
    /// The maximum life amount of this object, also its base life
    /// </summary>
    [Header("Life")]
    [SerializeField] int maxLife;
    /// <summary>
    /// The remaining amount of life of this object
    /// </summary>
    int currentLife;
    public int GetCurrentLifeAmount
    {
        get
        {
            return currentLife;
        }
    }
    public float GetCurrentLifePercentage { get { return (float)currentLife / (float)maxLife; } }

    public void Damage(int amount, float recoveringTime, RecoveringType recoveringType)
    {
        /*if (!Recovering)
        {*/
        if (recoveringType == RecoveringType.ConsidersRecover && Recovering)
            return;
        else
        {
            if (relatedDamageReceiver.GetDamageTag == AttackTag.Player)
            {
                if (ArenaManager.arenaManager != null)
                    ArenaManager.arenaManager.ScoreMng.IncreaseTakenDamages(amount);
            }

            if (currentArmorAmount > 0)
            {
                currentArmorAmount -= amount;
                if (currentArmorAmount < 0)
                {
                    amount = Mathf.Abs(currentArmorAmount);
                    currentArmorAmount = 0;
                }
                else
                    amount = 0;
            }

            currentLife -= Mathf.Abs(amount);

            #region Recover
            switch (recoveringType)
            {
                case (RecoveringType.ConsidersRecover):
                    currentRecoveringTime = recoveringTime;
                    break;

                case (RecoveringType.IgnoreRecoverDontSet):
                    break;

                case (RecoveringType.IgnoreRecoverSetIfHigher):
                    if(recoveringTime > currentRecoveringTime)
                        currentRecoveringTime = recoveringTime;
                    break;

                case (RecoveringType.IgnoreRecoverOverride):
                    currentRecoveringTime = recoveringTime;
                    break;

                case (RecoveringType.IgnoreRecoverAdd):
                    currentRecoveringTime += recoveringTime;
                    break;
            }
            #endregion

            relatedDamageReceiver.UpdateLifeBar(currentLife + currentArmorAmount);

            if (currentLife <= 0)
            {
                currentLife = 0;
                relatedDamageReceiver.Die();
            }
            else
            {
                if (OnLifeChange != null)
                    OnLifeChange(relatedDamageReceiver);
            }
        }
        //}
    }

    public void Heal(int amount)
    {
        currentLife += Mathf.Abs(amount);
        if (currentLife > maxLife)
            currentLife = maxLife;

        relatedDamageReceiver.UpdateLifeBar(currentLife);

        if (OnLifeChange != null)
            OnLifeChange(relatedDamageReceiver);
    }

    public void UpdateLifeRendering()
    {
        if(lifeBar != null)
            lifeBar.UpdateLifeBar(currentLife);
    }

    public OnLifeEvent OnLifeChange;
    #endregion

    #region
    [Header("Armor")]
    int currentArmorAmount;
    public int GetCurrentArmorAmount { get { return currentArmorAmount; } }
    int maxArmorAmount;

    public void SetUpArmorAndLife(int mxLife, int currentArmor, int maxArmor)
    {
        maxLife = mxLife;
        currentLife = maxLife;
        currentArmorAmount = currentArmor;
        maxArmorAmount = maxArmor;
    }
    #endregion

    #region Recovering
    [Header("Recovering")]
    float currentRecoveringTime;

    public bool Recovering
    {
        get
        {
            return currentRecoveringTime != 0;
        }
    }

    public void UpdateRecovering()
    {
        if (currentRecoveringTime > 0)
            currentRecoveringTime -= Time.deltaTime;
        else if (currentRecoveringTime < 0)
            currentRecoveringTime = 0;
    }
    #endregion
}
