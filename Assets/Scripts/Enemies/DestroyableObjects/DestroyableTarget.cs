using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTarget : MonoBehaviour, IDamageReceiver
{
    bool destroyable;

    [SerializeField] LifeManager lifeManager;
    [SerializeField] AttackTag damageTag;
    [SerializeField] Collider objectCollider;

    [Header("Feedbacks")]
    [SerializeField] Renderer objectRenderer;
    [SerializeField] GameObject[] objectRenderers;
    [SerializeField] ParticleSystem particlesToPlay;
    [SerializeField] Transform[] woodProjectionPositions;
    [SerializeField] AudioSource targetAudioSource;
    [SerializeField] Sound soundToPlayOnDestroy;

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


        if (objectCollider != null)
            objectCollider.enabled = false;
        else if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;


        if (particlesToPlay != null)
            particlesToPlay.Play();

        foreach(Transform tr in woodProjectionPositions)
        {
            FeedbackObject woodProjection = GameManager.gameManager.PoolManager.GetFeedbackObject(FeedbackObjectPoolTag.WoodDestruction, PoolInteractionType.GetFromPool);
            if (woodProjection != null)
            {
                woodProjection.transform.position = tr.position;
                woodProjection.StartFeedback(2, 0.2f);
            }
        }

        if(particlesToPlay == null && woodProjectionPositions.Length == 0)
        {
            FeedbackObject woodProjection = GameManager.gameManager.PoolManager.GetFeedbackObject(FeedbackObjectPoolTag.WoodDestruction, PoolInteractionType.GetFromPool);
            if (woodProjection != null)
            {
                woodProjection.transform.position = transform.position;
                woodProjection.StartFeedback(2, 0.2f);
            }
        }


        if (targetAudioSource != null)
            targetAudioSource.PlaySound(soundToPlayOnDestroy);


        if (objectRenderer != null)
            objectRenderer.enabled = false;

        foreach (GameObject obj in objectRenderers)
            obj.SetActive(false);

        if(objectRenderer == null && objectRenderers.Length == 0)
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
