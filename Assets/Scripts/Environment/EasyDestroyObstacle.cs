using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyDestroyObstacle : MonoBehaviour, IDamageReceiver
{
    [SerializeField] FeedbackObjectPoolTag onDestroyParticleType;
    [SerializeField] float particlesSize = 1;
    [SerializeField] float soundVolume = 0.1f;
    [SerializeField] Transform[] particlesPositions;

    public AttackTag GetDamageTag { get { return AttackTag.Environment; } }

    public bool IsDamageable { get { return true; } }

    public event OnLifeEvent OnDeath;

    public void Die()
    {
        foreach (Transform pos in particlesPositions)
        {
            FeedbackObject onDestroyFeedbackObject = GameManager.gameManager.PoolManager.GetFeedbackObject(onDestroyParticleType, PoolInteractionType.GetFromPool);
            onDestroyFeedbackObject.gameObject.SetActive(true);
            onDestroyFeedbackObject.transform.position = pos.position;
            onDestroyFeedbackObject.StartFeedback(transform.localScale.x * particlesSize, soundVolume);
        }
        Destroy(gameObject);
    }

    public void ReceiveDamage(IDamageSource damageSource, DamagesParameters damagesParameters, ProjectileSpecialParameters projSpecialParameters)
    {
        Die();
    }

    public void UpdateLifeBar(int lifeAmount)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ShipHitbox>() != null)
        {
            Die();
        }
    }
}
