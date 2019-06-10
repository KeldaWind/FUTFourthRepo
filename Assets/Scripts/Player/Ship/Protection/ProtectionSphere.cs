using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectionSphere : MonoBehaviour, IDamageReceiver
{
    private void Update()
    {
        UpdateSphere();
    }

    #region Management
    public void SetUpSphere(float duration, float radius, Transform parent, AttackTag attackTag)
    {
        gameObject.SetActive(true);
        protectionTag = attackTag;
        remainingLifeTime = duration;
        targetRadius = radius;
        transform.localScale = Vector3.zero;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
    }

    public void UpdateSphere()
    {
        if (remainingLifeTime > 0)
        {
            remainingLifeTime -= Time.deltaTime;
            transform.localScale = Mathf.Lerp(transform.localScale.x, targetRadius, expansionCoeff) * Vector3.one;
        }
        else if (remainingLifeTime < 0)
            remainingLifeTime = 0;
        else
        {
            transform.localScale = Mathf.Lerp(transform.localScale.x, 0, expansionCoeff) * Vector3.one;
            if (transform.localScale.x < 0.1f)
                SetOff();
        }

    }

    public void SetOff()
    {
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnProtectionSphere(this);
    }
    #endregion

    #region Pooling
    [Header("Pooling")]
    [SerializeField] ProtectionSpherePoolTag poolTag;
    public ProtectionSpherePoolTag GetProtectionSpherePoolTag { get { return poolTag; } }
    #endregion

    #region Sphere Parameters
    AttackTag protectionTag;
    float remainingLifeTime;
    float targetRadius;
    float expansionCoeff = 0.3f;
    #endregion

    #region Rendering
    [Header("Rendering")]
    [SerializeField] Renderer sphereRenderer;
    #endregion

    public AttackTag GetDamageTag { get { return protectionTag; } }

    public bool IsDamageable { get { return true; } }

    #region Useless
    public event OnLifeEvent OnDeath;

    public void Die()
    {
        
    }

    public void ReceiveDamage(IDamageSource damageSource, DamagesParameters damagesParameters, ProjectileSpecialParameters projSpecialParameters)
    {
        
    }

    public void UpdateLifeBar(int lifeAmount)
    {
    }
    #endregion
}
